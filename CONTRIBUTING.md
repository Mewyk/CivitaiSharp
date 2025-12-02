# Contributing to CivitaiSharp

Thank you for your interest in contributing to CivitaiSharp. This document provides guidelines and information for contributors.

---

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Coding Standards](#coding-standards)
- [Pull Request Process](#pull-request-process)
- [Testing Guidelines](#testing-guidelines)
- [Reporting Issues](#reporting-issues)

---

## Code of Conduct

- Be respectful and inclusive in all interactions
- Focus on constructive feedback
- Accept responsibility for mistakes and learn from them
- Prioritize the community's best interests

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- A C# IDE (Visual Studio 2022, VS Code with C# extension, or JetBrains Rider)
- Git

### Fork and Clone

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```shell
   git clone https://github.com/YOUR_USERNAME/CivitaiSharp.git
   cd CivitaiSharp
   ```
3. Add the upstream remote:
   ```shell
   git remote add upstream https://github.com/Mewyk/CivitaiSharp.git
   ```

---

## Development Setup

### Build the Project

```shell
dotnet restore
dotnet build
```

### Run Tests

```shell
dotnet test
```

### Run Tests with Coverage

```shell
dotnet tool install -g dotnet-coverage
dotnet-coverage collect -f cobertura -o coverage.cobertura.xml dotnet test
```

---

## Project Structure

```
CivitaiSharp/
    Core/                         # Public API wrapper (CivitaiSharp.Core)
    Sdk/                          # Generator/Orchestration API wrapper (CivitaiSharp.Sdk)
    Tools/                        # Download and parsing utilities (CivitaiSharp.Tools)
    Tests/
        CivitaiSharp.Core.Tests/  # Core library tests
        CivitaiSharp.Tools.Tests/ # Tools library tests
    Documentation/                # DocFX documentation site
    Sampler/                      # Sample application for testing
    Resources/                    # Project assets
    README.md
    CONTRIBUTING.md
    LICENSE
```

---

## Coding Standards

### General Principles

- Follow existing code patterns and conventions in the codebase
- Keep code simple and readable
- Prefer clarity over cleverness
- Write self-documenting code with meaningful names

### C# Conventions

- Use primary constructors for all new types
- Use full words for naming (no abbreviations like `Ctx` for `Context`)
- Follow the least-exposure rule: `private` > `internal` > `protected` > `public`
- Use nullable reference types (`#nullable enable`)
- Use records for immutable data types
- Use `[JsonPropertyName]` attributes for all JSON-serialized properties

### Code Formatting

#### File Organization

- Use file-scoped namespaces (`namespace X;` not `namespace X { }`)
- Order using directives: System namespaces first, then third-party, then project namespaces
- Separate using groups with blank lines

#### Naming Conventions

| Element | Convention | Example |
|---------|------------|--------|
| Namespaces | PascalCase | `CivitaiSharp.Core.Models` |
| Classes/Records | PascalCase | `ModelBuilder`, `ApiResult` |
| Interfaces | IPascalCase | `IApiClient`, `IPagedHttpClient` |
| Methods | PascalCase | `ExecuteAsync`, `GetByIdAsync` |
| Properties | PascalCase | `IsSuccess`, `ErrorInfo` |
| Private fields | _camelCase | `_httpClient`, `_filters` |
| Parameters | camelCase | `cancellationToken`, `resultsLimit` |
| Constants | PascalCase | `DefaultBaseUrl`, `MaxTimeoutSeconds` |
| Enum values | PascalCase | `ModelType.Checkpoint`, `ErrorCode.NotFound` |

#### DTO Patterns

**Response DTOs (Core library)**: Use primary constructor records with `[JsonPropertyName]` attributes:

```csharp
public sealed record Model(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description);
```

**Request DTOs (Sdk library)**: Use classes with init-only properties:

```csharp
public sealed class TextToImageJobRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("params")]
    public required ImageJobParams Params { get; init; }
}
```

#### Bracing and Spacing

- Always use braces for control statements, even single-line blocks
- Use a single blank line to separate logical sections
- No trailing whitespace
- Use 4 spaces for indentation (no tabs)
- Opening braces on same line for methods and control statements

#### Collections

- Use `IReadOnlyList<T>` for immutable collections in public APIs
- Use `ImmutableDictionary<K,V>` for builder state
- Prefer collection expressions `[]` over `Array.Empty<T>()`

### XML Documentation

- All public types, methods, and properties must have XML documentation
- Use `<summary>` for brief descriptions
- Use `<remarks>` for additional context and implementation details
- Use `<param>` for all method parameters
- Use `<returns>` for return value descriptions
- Use `<exception>` for documented exceptions

Example:
```csharp
/// <summary>
/// Submits a text-to-image generation job to the API.
/// </summary>
/// <param name="request">The job request containing generation parameters.</param>
/// <param name="cancellationToken">Token to cancel the operation.</param>
/// <returns>A result containing the job response or an error.</returns>
/// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
public Task<ApiResult<JobResponse>> SubmitAsync(
    TextToImageJobRequest request,
    CancellationToken cancellationToken = default);
```

### Error Handling

- Use `ArgumentNullException.ThrowIfNull()` for null checks
- Use `ArgumentException.ThrowIfNullOrWhiteSpace()` for string validation
- Return `ApiResult<T>` for expected failures (API errors, validation errors)
- Throw exceptions only for unexpected/programming errors

### Async Code

- All async methods must end with `Async` suffix
- Always accept and pass through `CancellationToken`
- Use `ConfigureAwait(false)` in library code
- Never use fire-and-forget patterns

---

## Pull Request Process

### Before Submitting

1. Create a feature branch from `main`:
   ```shell
   git checkout main
   git pull upstream main
   git checkout -b feature/your-feature-name
   ```

2. Make your changes following the coding standards

3. Write or update tests for your changes

4. Ensure all tests pass:
   ```shell
   dotnet test
   ```

5. Ensure the build succeeds with no warnings:
   ```shell
   dotnet build --warnaserror
   ```

### Submitting a Pull Request

1. Push your branch to your fork:
   ```shell
   git push origin feature/your-feature-name
   ```

2. Open a pull request against the `main` branch

3. Fill in the pull request template with:
   - A clear description of the changes
   - The motivation and context
   - Any breaking changes
   - Related issue numbers

### Pull Request Checklist

- [ ] Code follows the project's coding standards
- [ ] All public APIs have XML documentation
- [ ] Tests have been added or updated
- [ ] All tests pass
- [ ] No build warnings
- [ ] Commit messages are clear and descriptive

---

## Testing Guidelines

### Test Structure

- Test projects follow the naming convention `[ProjectName].Tests`
- Test classes mirror the classes they test: `ApiClient` -> `ApiClientTests`
- Test method names describe behavior: `WhenValidRequest_ThenReturnsSuccess`

### Writing Tests

- Follow the Arrange-Act-Assert (AAA) pattern
- One behavior per test
- Use clear, specific assertions
- Avoid mocking code within the solution; mock only external dependencies
- Tests should run in any order and in parallel

Example:
```csharp
[Fact]
public async Task WhenValidModelId_ThenReturnsModel()
{
    // Arrange
    var builder = new ModelBuilder(httpClient)
        .WhereModelId(12345);

    // Act
    var result = await builder.ExecuteAsync();

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(12345, result.Value.Items[0].Id);
}
```

### Test Frameworks

The project uses xUnit for testing. Key packages:
- `xunit`
- `xunit.runner.visualstudio`
- `Microsoft.NET.Test.Sdk`

---

## Reporting Issues

### Bug Reports

When reporting a bug, please include:

1. **Description**: A clear description of the bug
2. **Steps to Reproduce**: Minimal steps to reproduce the issue
3. **Expected Behavior**: What you expected to happen
4. **Actual Behavior**: What actually happened
5. **Environment**: 
   - OS and version
   - .NET version
   - CivitaiSharp version
6. **Code Sample**: Minimal code that demonstrates the issue

### Feature Requests

When requesting a feature, please include:

1. **Use Case**: Describe the problem you're trying to solve
2. **Proposed Solution**: Your idea for the feature
3. **Alternatives Considered**: Other solutions you've thought about
4. **Additional Context**: Any other relevant information

---

## Questions?

If you have questions about contributing, please open a discussion on GitHub or reach out through the project's communication channels.

---

## License

By contributing to CivitaiSharp, you agree that your contributions will be licensed under the MIT License.
