using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// See Common/Program.cs for setup patterns: #CoreBasicSetup, #CoreSetupWithApiKey, #CoreSetupFromConfiguration

var builder = Host.CreateApplicationBuilder(args);

#region OptionsConfiguration
builder.Services.AddCivitaiApi(options =>
{
    options.ApiKey = "your-api-key";
});
#endregion

#region ConfigurationFile
// builder.Services.AddCivitaiApi(builder.Configuration);
#endregion

#region EnvironmentVariable
// builder.Services.AddCivitaiApi(options =>
// {
//     options.ApiKey = Environment.GetEnvironmentVariable("CIVITAI_API_KEY");
// });
#endregion

using var host = builder.Build();
await host.StartAsync();

Console.WriteLine("API key configuration examples complete.");

await host.StopAsync();
