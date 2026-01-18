using System;
using System.Threading;
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        ConfigureAuthentication(builder.Services);

        using var host = builder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        await host.StartAsync(cts.Token);
        await host.StopAsync(cts.Token);

        Console.WriteLine("ApiQuirks samples are intended for documentation snippets.");
        return 0;
    }

    public static void ConfigureAuthentication(IServiceCollection services)
    {
        #region configure-auth
        services.AddCivitaiApi(options =>
        {
            options.ApiKey = "your-api-key-here";
        });
        #endregion
    }

    public static async Task CommercialUseExampleAsync(IApiClient apiClient)
    {
        #region commercial-use
        // Single value typically returns 0 results
        var singleValueResult = await apiClient.Models
            .WhereCommercialUse(CommercialUsePermission.None)
            .ExecuteAsync();

        // Multiple values work correctly
        var multipleValueResult = await apiClient.Models
            .WhereCommercialUse(
                CommercialUsePermission.Image,
                CommercialUsePermission.Sell)
            .ExecuteAsync();
        #endregion
    }

    public static void ConfigureTimeout(IServiceCollection services)
    {
        #region timeout-configuration
        services.AddCivitaiApi(options =>
        {
            options.TimeoutSeconds = 60; // Increase for problematic endpoints
        });
        #endregion
    }

    public static async Task SortValuesExampleAsync(IApiClient apiClient)
    {
        #region sort-values
        // "Highest Rated" → URL-encoded as "Highest%20Rated"
        var result = await apiClient.Models
            .OrderBy(ModelSort.HighestRated)
            .ExecuteAsync();
        #endregion
    }

    public static async Task FilterCompositionExampleAsync(IApiClient apiClient)
    {
        #region filter-composition
        var builder = apiClient.Models
            .WhereType(ModelType.Lora)
            .OrderBy(ModelSort.HighestRated);

        // Test base filter
        var baseResult = await builder.ExecuteAsync();

        // Add more filters after confirming base works
        var refinedResult = await builder
            .WherePeriod(TimePeriod.Month)
            .ExecuteAsync(resultsLimit: 10);
        #endregion
    }

    public static async Task ErrorHandlingExampleAsync(IApiClient apiClient)
    {
        #region error-handling
        var result = await apiClient.Creators
            .WhereName("artist")
            .ExecuteAsync();

        if (result is Result<PagedResult<Creator>>.Failure failure)
        {
            if (failure.Error.Code == ErrorCode.Timeout)
            {
                // Retry with longer timeout or different approach
            }
            else if (failure.Error.Code == ErrorCode.ServerError)
            {
                // Log and handle server errors
            }
        }
        #endregion
    }

    public static async Task VerifyResultsExampleAsync(IApiClient apiClient)
    {
        #region verify-results
        var result = await apiClient.Models
            .WhereCommercialUse(CommercialUsePermission.Image)
            .ExecuteAsync();

        if (result is Result<PagedResult<Model>>.Success success)
        {
            if (success.Data.Items.Count == 0)
            {
                // May indicate the single-value quirk
                // Retry with multiple values
            }
        }
        #endregion
    }
}
