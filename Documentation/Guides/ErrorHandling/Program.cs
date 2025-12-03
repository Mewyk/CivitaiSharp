using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();
var apiClient = host.Services.GetRequiredService<IApiClient>();

// #region pattern-matching
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
// #endregion pattern-matching

// #region properties
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
// #endregion properties

// #region tryget
var tryResult = await apiClient.Models.GetByIdAsync(123456);

// Using TryGet methods
if (tryResult.TryGetValue(out var model2))
{
    Console.WriteLine($"Model: {model2.Name}");
}
else if (tryResult.TryGetError(out var error))
{
    Console.WriteLine($"Failed: {error.Message}");
    if (error.InnerException is not null)
    {
        Console.WriteLine($"Cause: {error.InnerException.Message}");
    }
}
// #endregion tryget

// #region match
var matchResult = await apiClient.Models
    .WhereName("example")
    .FirstOrDefaultAsync();

// Using the Match method for exhaustive handling
var message = matchResult.Match(
    onSuccess: m => m is not null ? $"Found: {m.Name}" : "Not found",
    onFailure: e => $"Error: {e.Message}"
);
Console.WriteLine(message);
// #endregion match

// #region specific-errors
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
// #endregion specific-errors
