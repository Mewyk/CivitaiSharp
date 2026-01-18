using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

#region options-configuration
builder.Services.AddCivitaiApi(options =>
{
    options.ApiKey = "your-api-key";
});
#endregion

#region configuration-file
builder.Services.AddCivitaiApi(builder.Configuration);
#endregion

#region environment-variable
builder.Services.AddCivitaiApi(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("CIVITAI_API_KEY");
});
#endregion
