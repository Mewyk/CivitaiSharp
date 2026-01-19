using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();

await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();

#region PatternMatching
var result = await apiClient.Models
    .WhereType(ModelType.Lora)
    .FirstOrDefaultAsync();

// Pattern matching is the recommended approach
switch (result)
{
    case Result<Model?>.Success { Data: { } model }:
        Console.WriteLine($"Found: {model.Name}");
        break;
    case Result<Model?>.Success { Data: null }:
        Console.WriteLine("No model found");
        break;
    case Result<Model?>.Failure failure:
        Console.WriteLine($"Error: {failure.Error.Message}");
        break;
}
#endregion

#region TryGet
var tryResult = await apiClient.Models.GetByIdAsync(123456);

// Using TryGet methods
if (tryResult.TryGetValue(out var foundModel))
{
    Console.WriteLine($"Model: {foundModel.Name}");
}
else if (tryResult.TryGetError(out var tryError))
{
    Console.WriteLine($"Failed: {tryError.Message}");
    if (tryError.InnerException is not null)
    {
        Console.WriteLine($"Cause: {tryError.InnerException.Message}");
    }
}
#endregion

#region SpecificErrors
var errorResult = await apiClient.Models.GetByIdAsync(999999999);

if (errorResult is Result<Model>.Failure { Error: var err })
{
    switch (err.Code)
    {
        case ErrorCode.NotFound:
            Console.WriteLine("The model does not exist.");
            break;
        case ErrorCode.RateLimited:
            Console.WriteLine("Rate limited. Please wait before retrying.");
            if (err.RetryAfter.HasValue)
            {
                Console.WriteLine($"Retry after: {err.RetryAfter.Value.TotalSeconds} seconds");
            }
            break;
        case ErrorCode.Unauthorized:
            Console.WriteLine("Authentication required. Please provide an API key.");
            break;
        case ErrorCode.Timeout:
            Console.WriteLine("Request timed out. Try again later.");
            break;
        default:
            Console.WriteLine($"Unexpected error: {err.Code} - {err.Message}");
            break;
    }
}
#endregion

#region ChainingOperations
var modelsForSelect = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync(resultsLimit: 10);

var modelNames = modelsForSelect.Select(paged => paged.Items.Select(m => m.Name).ToList());

if (modelNames.IsSuccess && modelNames.ValueOrDefault is { } names)
{
    Console.WriteLine($"Model names: {string.Join(", ", names)}");
}
#endregion

#region RateLimiting
var rateLimitResult = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync();

if (rateLimitResult is Result<PagedResult<Model>>.Failure { Error: { Code: ErrorCode.RateLimited } rateLimitError })
{
    if (rateLimitError.RetryAfter.HasValue)
    {
        Console.WriteLine($"Rate limited. Retry after: {rateLimitError.RetryAfter.Value.TotalSeconds} seconds");
        await Task.Delay(rateLimitError.RetryAfter.Value);
        // Retry the request
        var retryResult = await apiClient.Models.WhereType(ModelType.Lora).ExecuteAsync();
        Console.WriteLine($"Retry result: {(retryResult.IsSuccess ? "Success" : "Failure")}");
    }
}
#endregion

#region OnSuccessOnFailure
var onSuccessResult = await apiClient.Models
    .WhereName("example")
    .FirstOrDefaultAsync();

onSuccessResult
    .OnSuccess(model => Console.WriteLine($"Found: {model?.Name}"))
    .OnFailure(error => Console.WriteLine($"Query failed: {error.Message}"));
#endregion

await host.StopAsync();
