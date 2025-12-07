namespace CivitaiSharp.Tools.Tests;

/// <summary>
/// Shared test fixture for Tools tests.
/// </summary>
/// <remarks>
/// Tools no longer depends on SDK types. The AirBuilder has been moved to the SDK project.
/// This fixture remains available for any shared test initialization needs.
/// </remarks>
public sealed class ToolsTestFixture
{
    /// <summary>
    /// Initializes the Tools test fixture.
    /// </summary>
    public ToolsTestFixture()
    {
        // Tools tests no longer require SDK initialization
        // since AirBuilder has been moved to the SDK project
    }
}
