using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

#region Registration
builder.Services.AddCivitaiApi(options =>
{
    options.Key = "your-api-key";
});
#endregion

using var host = builder.Build();
await host.StartAsync();

IApiClient client = host.Services.GetRequiredService<IApiClient>();

#region BasicUsage
var result = await client.Models
    .WhereType(ModelType.Lora)
    .WhereTag("anime")
    .ExecuteAsync(resultsLimit: 10);

if (result is Result<PagedResult<Model>>.Success success)
{
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"{model.Id}: {model.Name}");
    }
}
#endregion

await host.StopAsync();
