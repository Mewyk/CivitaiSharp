using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#region ActionDelegate
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCivitaiApi(options =>
{
    options.ApiKey = "your-api-key";
    options.TimeoutSeconds = 60;
});

var host = builder.Build();
#endregion

await host.StartAsync();

#region IConfiguration
// Configuration is read from the "CivitaiApi" section by default
builder.Services.AddCivitaiApi(builder.Configuration);

// Or specify a custom section name
builder.Services.AddCivitaiApi(
    builder.Configuration,
    sectionName: "MyCivitaiSettings");
#endregion

var apiClient = host.Services.GetRequiredService<IApiClient>();
Console.WriteLine("Configuration example completed.");

await host.StopAsync();
