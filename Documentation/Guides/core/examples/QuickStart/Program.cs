using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// See Common/Program.cs for setup patterns: #CoreBasicSetup, #CoreSetupWithApiKey

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();
await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();

#region query

// Build a query for LoRA models tagged with "anime"
var result = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WhereTag("anime")
    .ExecuteAsync(resultsLimit: 10);
#endregion

#region Handling
if (result is Result<PagedResult<Model>>.Success success)
{
    Console.WriteLine($"Found {success.Data.Items.Count} models:");
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"  - {model.Name} (ID: {model.Id})");
    }
}
else if (result is Result<PagedResult<Model>>.Failure failure)
{
    Console.WriteLine($"Error: {failure.Error.Code} - {failure.Error.Message}");
}
#endregion

await host.StopAsync();
