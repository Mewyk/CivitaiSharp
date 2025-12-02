<p align="center">
  <img src="Resources/Logo/CivitaiSharp.png" alt="Logo de CivitaiSharp - Imagen temporal generada por IA" width="120"/>
</p>

<h1 align="center" style="border-bottom: none;">CivitaiSharp</h1>

<p align="center">
  Una biblioteca cliente .NET 10 moderna, liviana y compatible con AOT para todo lo relacionado con Civitai.com
</p>

<p align="center">
  <!-- Estado -->
  <img src="https://img.shields.io/badge/Estado-Alpha-4682B4?style=flat&logo=beaker" alt="Estado: Alpha"/>
  <!-- Licencia -->
  <a href="https://opensource.org/licenses/MIT">
    <img src="https://img.shields.io/badge/Licencia-MIT-4682B4?style=flat&logo=book" alt="Licencia MIT"/>
  </a>
  <!-- .NET -->
  <a href="https://dotnet.microsoft.com/">
    <img src="https://img.shields.io/badge/.NET-10.0-4682B4?style=flat&logo=dotnet" alt=".NET 10"/>
  </a>
  <!-- GitHub -->
  <a href="https://github.com/Mewyk/CivitaiSharp">
    <img src="https://img.shields.io/badge/GitHub-Mewyk%2FCivitaiSharp-4682B4?style=flat&logo=github" alt="GitHub"/>
  </a>
  <!-- NuGet (dinámico) -->
  <a href="https://www.nuget.org/packages/CivitaiSharp.Core">
    <img src="https://img.shields.io/nuget/v/CivitaiSharp.Core?label=NuGet%20Core&logo=nuget" alt="NuGet Core"/>
  </a>
</p>

<p align="center">
  <a href="README.md">English</a> |
  <strong>Español (Argentina)</strong> |
  <a href="README.ja.md">日本語</a>
</p>

<p align="center">
<strong>
  CivitaiSharp está actualmente en Alpha: las APIs, funcionalidades y estabilidad están sujetas a cambios.
</strong>
</p>

## Tabla de Contenidos
1. [Paquetes y Calendario de Lanzamiento](#1-paquetes-y-calendario-de-lanzamiento)
2. [Instalación](#2-instalación)
3. [Inicio Rápido](#3-inicio-rápido)
4. [Configuración](#4-configuración)
5. [Ejemplos de API](#5-ejemplos-de-api)
6. [Características](#6-características)
7. [Documentación](#7-documentación)
8. [Particularidades Conocidas de la API](#8-particularidades-conocidas-de-la-api)
9. [Versionado](#9-versionado)
10. [Licencia](#10-licencia)
11. [Contribuciones](#11-contribuciones)

---

## 1. Paquetes y Calendario de Lanzamiento
| Paquete | Estado | Descripcion |
|---------|--------|-------------|
| **CivitaiSharp.Core** | Alpha | Cliente API publico para modelos, imagenes, etiquetas y creadores |
| **CivitaiSharp.Sdk** | Alpha | Cliente API de Generacion/Orquestacion para trabajos de generacion de imagenes |
| **CivitaiSharp.Tools** | Planificado | Utilidades para descargas, hash y analisis de HTML |

> **Nota:** Tanto los paquetes Core como Sdk estan actualmente en Alpha. Las APIs pueden cambiar entre versiones menores.

> **Advertencia:** CivitaiSharp.Sdk no esta completamente probado y no debe usarse en entornos de produccion. Uselo bajo su propio riesgo.

---

## 2. Instalación
Instalar vía NuGet:

```shell
dotnet add package CivitaiSharp.Core
```

---

## 3. Inicio Rápido
### Ejemplo Mínimo
La forma más sencilla de comenzar con CivitaiSharp.Core:

```csharp
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCivitaiApi(_ => { });

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IApiClient>();

var result = await client.Models.ExecuteAsync();
if (result.IsSuccess)
{
    foreach (var model in result.Value.Items)
        Console.WriteLine(model.Name);
}
```

### Ejemplo Completo con Configuración

```csharp
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCivitaiApi(options =>
{
    options.TimeoutSeconds = 120;
    options.StrictJsonParsing = true;
});

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IApiClient>();

var result = await client.Models
    .WhereType(ModelType.Checkpoint)
    .WhereNsfw(false)
    .OrderBy(ModelSort.MostDownloaded)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (result.IsSuccess)
{
    foreach (var model in result.Value.Items)
        Console.WriteLine($"{model.Name} por {model.Creator?.Username}");
}
else
{
    Console.WriteLine($"Error: {result.ErrorInfo.Message}");
}
```

---

## 4. Configuración
### Usando appsettings.json
CivitaiSharp.Core lee la configuración de la sección `CivitaiApi` por defecto.

<details>
<summary><strong>Configuración Mínima (appsettings.json)</strong></summary>

```json
{
  "CivitaiApi": {
  }
}
```

Todas las configuraciones tienen valores predeterminados sensatos, por lo que una sección vacía es válida.

</details>

<details>
<summary><strong>Configuración Completa (appsettings.json)</strong></summary>

```json
{
  "CivitaiApi": {
    "BaseUrl": "https://civitai.com",
    "ApiVersion": "v1",
    "ApiKey": null,
    "TimeoutSeconds": 30,
    "StrictJsonParsing": false
  }
}
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `BaseUrl` | `string` | `https://civitai.com` | URL base para la API de Civitai |
| `ApiVersion` | `string` | `v1` | Segmento de ruta de la versión de API |
| `ApiKey` | `string?` | `null` | Clave API opcional para solicitudes autenticadas |
| `TimeoutSeconds` | `int` | `30` | Tiempo de espera de solicitud HTTP (1-300 segundos) |
| `StrictJsonParsing` | `bool` | `false` | Lanzar excepción en propiedades JSON no mapeadas |

> **Nota de Autenticación:** La biblioteca Core puede consultar endpoints públicos (modelos, imágenes, tags, creadores) sin clave API. Una clave API solo es necesaria para funcionalidades autenticadas como favoritos, modelos ocultos y límites de tasa más altos. Esto es diferente a CivitaiSharp.Sdk que **siempre requiere un token de API** para todas las operaciones.

</details>

<details>
<summary><strong>Configuración con IConfiguration</strong></summary>

```csharp
using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();
services.AddCivitaiApi(configuration);
// O con un nombre de sección personalizado:
// services.AddCivitaiApi(configuration, "MiSeccionPersonalizada");

await using var provider = services.BuildServiceProvider();
```

</details>

---

## 5. Ejemplos de API
CivitaiSharp.Core proporciona constructores fluidos para cada endpoint. Cada constructor es inmutable y seguro para hilos.

<details>
<summary><strong>Endpoint de Modelos</strong></summary>

```csharp
// Obtener todos los modelos (consulta predeterminada)
var result = await client.Models.ExecuteAsync();

// Obtener un modelo específico por ID
var result = await client.Models.GetByIdAsync(12345);
if (result.IsSuccess)
    Console.WriteLine($"Modelo: {result.Value.Name}");

// Obtener el primer modelo coincidente (recuperación eficiente de un solo elemento)
var result = await client.Models
    .WhereName("SDXL")
    .FirstOrDefaultAsync();
if (result is { IsSuccess: true, Value: not null })
    Console.WriteLine($"Encontrado: {result.Value.Name}");

// Buscar por nombre
var result = await client.Models
    .WhereName("SDXL")
    .ExecuteAsync();

// Filtrar por tipo y ordenar
var result = await client.Models
    .WhereType(ModelType.Checkpoint)
    .OrderBy(ModelSort.MostDownloaded)
    .WithResultsLimit(25)
    .ExecuteAsync();

// Filtrar por etiqueta
var result = await client.Models
    .WhereTag("anime")
    .WhereNsfw(false)
    .ExecuteAsync();

// Filtrar por creador
var result = await client.Models
    .WhereUsername("Mewyk")
    .OrderBy(ModelSort.Newest)
    .ExecuteAsync();

// Filtrar por modelo base (valor string, ej., "SDXL 1.0", "SD 1.5", "Flux.1 D")
var result = await client.Models
    .WhereBaseModel("SDXL 1.0")
    .WhereType(ModelType.Lora)
    .ExecuteAsync();

// Filtrar por múltiples modelos base
var result = await client.Models
    .WhereBaseModels("SDXL 1.0", "Pony")
    .ExecuteAsync();

// Filtrar por IDs de modelo específicos (ignorado si también se proporciona consulta)
var result = await client.Models
    .WhereIds(12345, 67890, 11111)
    .ExecuteAsync();
```

</details>

<details>
<summary><strong>Endpoint de Imágenes</strong></summary>

```csharp
// Obtener todas las imágenes (consulta predeterminada)
var result = await client.Images.ExecuteAsync();

// Obtener la primera imagen coincidente
var result = await client.Images
    .WhereModelId(12345)
    .FirstOrDefaultAsync();
if (result is { IsSuccess: true, Value: not null })
    Console.WriteLine($"URL de imagen: {result.Value.Url}");

// Filtrar por ID de modelo
var result = await client.Images
    .WhereModelId(12345)
    .ExecuteAsync();

// Filtrar por ID de versión de modelo
var result = await client.Images
    .WhereModelVersionId(67890)
    .OrderBy(ImageSort.Newest)
    .ExecuteAsync();

// Filtrar por nombre de usuario
var result = await client.Images
    .WhereUsername("Mewyk")
    .WhereNsfwLevel(ImageNsfwLevel.None)
    .WithResultsLimit(50)
    .ExecuteAsync();

// Filtrar por ID de publicación
var result = await client.Images
    .WherePostId(11111)
    .ExecuteAsync();
```

</details>

<details>
<summary><strong>Endpoint de Etiquetas</strong></summary>

```csharp
// Obtener todas las etiquetas (consulta predeterminada)
var result = await client.Tags.ExecuteAsync();

// Buscar etiquetas por nombre
var result = await client.Tags
    .WhereName("portrait")
    .WithResultsLimit(100)
    .ExecuteAsync();
```

</details>

<details>
<summary><strong>Endpoint de Creadores</strong></summary>

```csharp
// Obtener todos los creadores (consulta predeterminada)
var result = await client.Creators.ExecuteAsync();

// Buscar creadores por nombre
var result = await client.Creators
    .WhereName("Mewyk")
    .WithResultsLimit(20)
    .ExecuteAsync();

// Paginación basada en páginas (los creadores usan páginas, no cursores)
var result = await client.Creators
    .WithPageIndex(2)
    .WithResultsLimit(50)
    .ExecuteAsync();
```

</details>

<details>
<summary><strong>Paginación</strong></summary>

```csharp
// Paginación basada en cursor (Modelos, Imágenes, Etiquetas)
string? cursor = null;
var allModels = new List<Model>();

do
{
    var result = await client.Models
        .WhereType(ModelType.Checkpoint)
        .WithResultsLimit(100)
        .ExecuteAsync(cursor: cursor);

    if (!result.IsSuccess)
        break;

    allModels.AddRange(result.Value.Items);
    cursor = result.Value.Metadata?.NextCursor;
    
} while (cursor is not null);

// Paginación basada en páginas (solo Creadores)
var page1 = await client.Creators.WithPageIndex(1).ExecuteAsync();
var page2 = await client.Creators.WithPageIndex(2).ExecuteAsync();
```

</details>

<details>
<summary><strong>Manejo de Errores</strong></summary>

```csharp
var result = await client.Models.ExecuteAsync();

// Coincidencia de patrones
var message = result switch
{
    { IsSuccess: true, Value: var page } => $"Encontrados {page.Items.Count} modelos",
    { IsSuccess: false, ErrorInfo: var error } => error.Code switch
    {
        ErrorCode.RateLimited => "Demasiadas solicitudes, por favor reduce la velocidad",
        ErrorCode.Unauthorized => "Clave API invalida o faltante",
        ErrorCode.NotFound => "Recurso no encontrado",
        _ => $"Error: {error.Code} - {error.Message}"
    }
};

// Enfoque tradicional
if (result.IsSuccess)
{
    foreach (var model in result.Value.Items)
        Console.WriteLine(model.Name);
}
else
{
    Console.WriteLine($"Fallo: {result.ErrorInfo.Message}");
}
```

</details>

---

## 6. Características
- **.NET 10 Moderno** - Construido con tipos de referencia nullable, records y constructores primarios
- **Constructores de Consulta Fluidos** - Constructores inmutables y seguros para tipos para construir solicitudes API
- **Patrón Result** - Manejo explícito de éxito/falla con unión discriminada
- **Resiliencia Integrada** - Políticas de reintento, circuit breakers, limitación de tasa y tiempos de espera
- **Inyección de Dependencias** - Soporte de primera clase para `IHttpClientFactory` y Microsoft DI
- **Descargas en Streaming** - Manejo de respuestas eficiente en memoria con `ResponseHeadersRead`
- **Contrato JSON Explícito** - Todas las propiedades del modelo usan `[JsonPropertyName]` para seguridad de tipos

---

## 7. Documentación
- [Referencia de API](https://CivitaiSharp.Mewyk.com/Docs/api/)
- [Guía de Inicio](https://CivitaiSharp.Mewyk.com/Guides/introduction.html)

---

## 8. Particularidades Conocidas de la API
CivitaiSharp interactua con la API de Civitai.com, que tiene varias particularidades conocidas. Algunas se mitigan automaticamente; otras estan documentadas y bajo investigacion.

| Problema | Descripcion |
|----------|-------------|
| Conteo de modelos en endpoint de etiquetas | Caracteristica documentada, pero las respuestas nunca incluyen este campo |
| Parametro `limit=0` | Documentado para devolver todos los resultados para varios endpoints, pero devuelve un error |
| Errores semanticos con HTTP 200 | Errores, No Encontrado y otros, todos devuelven HTTP 200 |
| Inconsistencias de endpoints | Limitacion intermitente, interrupciones no reportadas, limites de tasa no documentados |
| Estructuras de metadatos variables | El formato de metadatos varia ampliamente entre respuestas |
| Problemas de existencia de usuarios | Durante interrupciones parciales, usuarios existentes pueden aparecer como inexistentes |
| **Falta de fiabilidad del endpoint de creadores** | Ver detalles a continuacion |

### Problemas de Fiabilidad del Endpoint de Creadores

El endpoint `/api/v1/creators` experimenta problemas de fiabilidad intermitentes:

- **Errores HTTP 500**: El endpoint frecuentemente devuelve errores del servidor, especialmente bajo carga
- **Tiempos de respuesta lentos**: Las solicitudes pueden tomar significativamente mas tiempo que otros endpoints (10-30+ segundos)
- **Fallos por tiempo de espera**: Los tiempos de respuesta largos pueden exceder los umbrales de timeout del cliente

**Recomendaciones:**

1. **Implementar tiempos de espera generosos**: Configurar timeouts de 60-120 segundos para solicitudes al endpoint de Creadores
2. **Usar logica de reintento**: El manejador de resiliencia integrado reintentara en errores 500, pero el exito no esta garantizado
3. **Manejar fallos graciosamente**: Tu aplicacion debe degradarse graciosamente cuando los datos de Creadores no esten disponibles
4. **Cachear resultados agresivamente**: Cuando las solicitudes tengan exito, cachear los resultados para reducir la carga de la API

```csharp
// Ejemplo: Manejando la falta de fiabilidad del endpoint de Creadores
var result = await client.Creators.WithResultsLimit(10).ExecuteAsync();

if (!result.IsSuccess)
{
    // Registrar el error pero continuar con funcionalidad degradada
    logger.LogWarning("Datos de creadores no disponibles: {Error}", result.ErrorInfo.Message);
    return GetCachedCreators(); // Fallback a datos cacheados
}
```

Se estan rastreando particularidades adicionales y se abordaran en futuras versiones.

---

## 9. Versionado
CivitaiSharp sigue el versionado semántico **MAYOR.MENOR.PARCHE**:

| Componente | Descripción |
|------------|-------------|
| **MAYOR** | Cambios significativos e incompatibles de API |
| **MENOR** | Nuevas funcionalidades; puede incluir cambios incompatibles limitados que probablemente no afecten a la mayoría de los usuarios |
| **PARCHE** | Correcciones de errores y mejoras compatibles hacia atrás |

Las versiones preliminares usan el formato: `MAYOR.MENOR.PARCHE-alpha.N`

> **Nota**: Mientras esté en Alpha (0.x.x), las APIs pueden cambiar entre versiones menores. La estabilidad está garantizada a partir de v1.0.0.

---

## 10. Licencia
Este repositorio se publica bajo la [Licencia MIT](LICENSE).

---

## 11. Contribuciones
Las contribuciones son bienvenidas. Por favor lee [CONTRIBUTING.md](CONTRIBUTING.md) para las pautas.

---

## Aviso Legal

CivitaiSharp es un proyecto de codigo abierto independiente y no esta afiliado, patrocinado, respaldado ni oficialmente asociado con Civitai.com o Civitai, Inc. El nombre Civitai y cualquier marca comercial relacionada son propiedad de sus respectivos duenos. El uso de estos nombres es solo con fines de identificacion y no implica ningun respaldo o asociacion.

---

<p align="center">
  <a href="https://github.com/Mewyk/CivitaiSharp">GitHub</a> |
  <a href="https://CivitaiSharp.Mewyk.com">Documentacion</a> |
  <a href="https://civitai.com">Civitai</a>
</p>
