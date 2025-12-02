namespace CivitaiSharp.Sdk.Tests;

using CivitaiSharp.Sdk.Extensions;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Shared test fixture that ensures SDK-specific enum mappings are registered
/// with the Core's ApiStringRegistry before any SDK tests run.
/// </summary>
/// <remarks>
/// This fixture uses the public DI registration path to initialize the SDK,
/// which triggers enum registry initialization as a consumer would.
/// </remarks>
public sealed class SdkTestFixture
{
    /// <summary>
    /// Initializes the SDK test fixture by registering SDK services,
    /// which triggers enum mappings registration.
    /// </summary>
    public SdkTestFixture()
    {
        // Use the public DI registration to trigger initialization
        // This mimics what a real consumer would do
        var services = new ServiceCollection();
        services.AddCivitaiSdk(options =>
        {
            options.ApiToken = "test-token-for-initialization";
        });
        
        // Build and dispose - we just need the initialization side effect
        services.BuildServiceProvider().Dispose();
    }
}
