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

#region Properties
var queryResult = await apiClient.Models
    .WhereType(ModelType.Checkpoint)
    .ExecuteAsync();

// Using IsSuccess/IsFailure properties
if (queryResult.IsSuccess)
{
    var models = queryResult.ValueOrDefault!.Items;
    Console.WriteLine($"Found {models.Count} models");
}
else if (queryResult.IsFailure)
{
    var error = queryResult.ErrorOrDefault!;
    Console.WriteLine($"Error {error.Code}: {error.Message}");
}
#endregion

#region TryGet
var tryResult = await apiClient.Models.GetByIdAsync(123456);

// Using TryGet methods
if (tryResult.TryGetValue(out var model))
{
    Console.WriteLine($"Model: {model.Name}");
}
else if (tryResult.TryGetError(out var error))
{
    Console.WriteLine($"Failed: {error.Message}");
    if (error.InnerException is not null)
    {
        Console.WriteLine($"Cause: {error.InnerException.Message}");
    }
}
#endregion

#region Match
var matchResult = await apiClient.Models
    .WhereName("example")
    .FirstOrDefaultAsync();

// Using the Match method for exhaustive handling
var message = matchResult.Match(
    onSuccess: m => m is not null ? $"Found: {m.Name}" : "Not found",
    onFailure: e => $"Error: {e.Message}"
);
Console.WriteLine(message);
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

#region ChainingSelect
var modelsForSelect = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync(resultsLimit: 10);

var modelNames = modelsForSelect.Select(paged => paged.Items.Select(m => m.Name).ToList());

if (modelNames.IsSuccess && modelNames.ValueOrDefault is { } names)
{
    Console.WriteLine($"Model names: {string.Join(", ", names)}");
}
#endregion

#region ChainingSelectMany
var firstModelResult = await apiClient.Models
    .FirstOrDefaultAsync();

var modelDetails = await firstModelResult.SelectManyAsync(async model =>
    model is not null
        ? await apiClient.Images.WhereModelId(model.Id).FirstOrDefaultAsync()
        : new Result<Image?>.Success(null));

if (modelDetails is Result<Image?>.Success { Data: { } image })
{
    Console.WriteLine($"Found image {image.Id} for model");
}
#endregion

#region RateLimiting
var rateLimitResult = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync();

if (rateLimitResult is Result<PagedResult<Model>>.Failure { Error: { Code: ErrorCode.RateLimited } error })
{
    if (error.RetryAfter.HasValue)
    {
        Console.WriteLine($"Rate limited. Retry after: {error.RetryAfter.Value.TotalSeconds} seconds");
        await Task.Delay(error.RetryAfter.Value);
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
