using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// #region action-delegate
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCivitaiApi(options =>
{
    options.BaseUrl = "https://civitai.com";
    options.ApiVersion = "v1";
    options.ApiKey = "your-api-key";
    options.TimeoutSeconds = 60;
});

var host = builder.Build();
// #endregion action-delegate

// #region iconfiguration
var builderWithConfig = Host.CreateApplicationBuilder(args);

// Configuration is read from the "CivitaiApi" section by default
builderWithConfig.Services.AddCivitaiApi(builderWithConfig.Configuration);

// Or specify a custom section name
builderWithConfig.Services.AddCivitaiApi(
    builderWithConfig.Configuration,
    sectionName: "MyCivitaiSettings");

var hostWithConfig = builderWithConfig.Build();
// #endregion iconfiguration

var apiClient = host.Services.GetRequiredService<IApiClient>();
Console.WriteLine("Configuration example completed.");
