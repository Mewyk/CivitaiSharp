using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



#region AutomaticConfiguration
// API token is automatically loaded from appsettings.json CivitaiSdk section
var autoBuilder = Host.CreateApplicationBuilder(args);
autoBuilder.Services.AddCivitaiSdk(autoBuilder.Configuration);
var autoHost = autoBuilder.Build();
#endregion

#region ConfigurationWithOverrides
// Load from appsettings.json but override specific options
var overrideBuilder = Host.CreateApplicationBuilder(args);
overrideBuilder.Services.AddCivitaiSdk(overrideBuilder.Configuration);
overrideBuilder.Services.Configure<SdkClientOptions>(options =>
{
    options.TimeoutSeconds = 1200;  // Override timeout to 20 minutes
});
var overrideHost = overrideBuilder.Build();
#endregion

await autoHost.StartAsync();

var sdkClient = autoHost.Services.GetRequiredService<ISdkClient>();
Console.WriteLine("SDK configuration example completed.");

await autoHost.StopAsync();
