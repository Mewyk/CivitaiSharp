using System;
using System.Threading;
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// See Common/Program.cs for setup patterns: #CoreBasicSetup, #ResultPatternMatching

var builder = Host.CreateApplicationBuilder(args);

#region ConfigureAuth
builder.Services.AddCivitaiApi(options =>
{
    options.ApiKey = "your-api-key-here";
});
#endregion

var host = builder.Build();
await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();

#region CommercialUse
// Single value typically returns 0 results
var singleValueResult = await apiClient.Models
    .WhereCommercialUse(CommercialUsePermission.None)
    .ExecuteAsync();

// Multiple values work correctly
var multipleValueResult = await apiClient.Models
    .WhereCommercialUse([
        CommercialUsePermission.Image,
        CommercialUsePermission.Sell])
    .ExecuteAsync();
#endregion

#region SortValues
// "Highest Rated" → URL-encoded as "Highest%20Rated"
var sortResult = await apiClient.Models
    .OrderBy(ModelSort.HighestRated)
    .ExecuteAsync();
#endregion

#region FilterComposition
var baseBuilder = apiClient.Models
    .WhereType(ModelType.Lora)
    .OrderBy(ModelSort.HighestRated);

// Test base filter
var baseResult = await baseBuilder.ExecuteAsync();

// Add more filters after confirming base works
var refinedResult = await baseBuilder
    .WherePeriod(TimePeriod.Month)
    .ExecuteAsync(resultsLimit: 10);
#endregion

#region ErrorHandling
var errorResult = await apiClient.Creators
    .WhereName("artist")
    .ExecuteAsync();

if (errorResult is Result<PagedResult<Creator>>.Failure failure)
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

#region VerifyResults
var verifyResult = await apiClient.Models
    .WhereCommercialUse(CommercialUsePermission.Image)
    .ExecuteAsync();

if (verifyResult is Result<PagedResult<Model>>.Success success)
{
    if (success.Data.Items.Count == 0)
    {
        // May indicate the single-value quirk
        // Retry with multiple values
    }
}
#endregion

#region TimeoutConfiguration
builder.Services.AddCivitaiApi(options =>
{
    options.TimeoutSeconds = 60; // Increase for problematic endpoints
});
#endregion

await host.StopAsync();
