using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#region SdkConfiguration
var builder = Host.CreateApplicationBuilder(args);

// Note: Unlike CivitaiSharp.Core, the SDK always requires authentication.
// All Generator API operations require a valid API token.
builder.Services.AddCivitaiSdk(options =>
{
    options.ApiToken = "your-api-token";  // Required - SDK cannot operate without a token
    options.TimeoutSeconds = 600;  // 10 minutes for long-running jobs
});

var host = builder.Build();
#endregion

await host.StartAsync();

var sdkClient = host.Services.GetRequiredService<ISdkClient>();
Console.WriteLine("SDK configuration example completed.");

await host.StopAsync();
