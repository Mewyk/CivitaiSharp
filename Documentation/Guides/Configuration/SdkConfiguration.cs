// #region sdk-configuration
using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guides;

public static class SdkConfiguration
{
    public static void ConfigureSdk(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Note: Unlike CivitaiSharp.Core, the SDK always requires authentication.
        // All Generator API operations require a valid API token.
        builder.Services.AddCivitaiSdk(options =>
        {
            options.ApiToken = "your-api-token";  // Required - SDK cannot operate without a token
            options.BaseUrl = "https://orchestration.civitai.com";
            options.ApiVersion = "v1";
            options.TimeoutSeconds = 600;  // 10 minutes for long-running jobs
        });

        var host = builder.Build();

        var sdkClient = host.Services.GetRequiredService<ICivitaiSdkClient>();
        Console.WriteLine("SDK configuration example completed.");
    }
}
// #endregion sdk-configuration
