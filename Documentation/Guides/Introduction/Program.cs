using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

#region registration
builder.Services.AddCivitaiApi(options =>
{
    options.ApiKey = "your-api-key"; // Optional - public endpoints work without a key
});
#endregion

using var host = builder.Build();

IApiClient client = host.Services.GetRequiredService<IApiClient>();

#region request-builders
var baseQuery = client.Models.WhereType(ModelType.Lora);

// These create separate queries, baseQuery is unchanged
var animeQuery = baseQuery.WhereTag("anime");
var realisticQuery = baseQuery.WhereTag("realistic");
#endregion

public sealed class MyService(IApiClient apiClient)
{
    #region basic-usage
    public async Task QueryModelsAsync()
    {
        var result = await apiClient.Models
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
    }
    #endregion
}
