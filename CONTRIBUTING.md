# Contributing to CivitaiSharp

Thank you for your interest in contributing to CivitaiSharp! This document provides guidelines and best practices for contributors.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Code Standards](#code-standards)
- [Testing](#testing)
- [Documentation](#documentation)
- [Pull Requests](#pull-requests)
- [Reporting Issues](#reporting-issues)

## Code of Conduct

We are committed to providing a welcoming and inclusive experience for everyone. Please:

- Be respectful and constructive in all interactions
- Focus feedback on the code, not the person
- Accept and learn from constructive criticism
- Prioritize the community's best interests

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- A C# IDE (Visual Studio 2022, VS Code with C# DevKit, or JetBrains Rider)
- Git for version control

### Setting Up Your Environment

1. **Fork the repository** on GitHub

2. **Clone your fork locally**:
   ```shell
   git clone https://github.com/YOUR_USERNAME/CivitaiSharp.git
   cd CivitaiSharp
   ```

3. **Add the upstream remote**:
   ```shell
   git remote add upstream https://github.com/Mewyk/CivitaiSharp.git
   ```

4. **Build the solution**:
   ```shell
   dotnet restore
   dotnet build
   ```

5. **Run the tests** to verify your setup:
   ```shell
   dotnet test
   ```

### Project Structure

```
CivitaiSharp/
  Core/           CivitaiSharp.Core - Public API client library
  Sdk/            CivitaiSharp.Sdk - High-level SDK for image generation
  Tools/          CivitaiSharp.Tools - File utilities (hashing, downloads)
  Tests/
    CivitaiSharp.Core.Tests/
    CivitaiSharp.Sdk.Tests/
    CivitaiSharp.Tools.Tests/
  Documentation/  DocFX documentation site
  Sampler/        Sample application for manual testing
  Resources/      Project assets (logos, scripts)
```

## Development Workflow

### Branch Strategy

- Create feature branches from `development`
- Use descriptive branch names: `feature/add-model-filtering`, `fix/pagination-cursor`
- Keep branches focused on a single feature or fix

### Making Changes

1. **Sync with upstream**:
   ```shell
   git fetch upstream
   git checkout development
   git merge upstream/development
   ```

2. **Create a feature branch**:
   ```shell
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes** following the code standards below

4. **Commit with clear messages**:
   ```shell
   git commit -m "Add model type filtering to ModelBuilder"
   ```

5. **Push and create a pull request**:
   ```shell
   git push origin feature/your-feature-name
   ```

## Code Standards

### General Principles

- Follow existing patterns in the codebase
- Keep code simple and readable
- Write self-documenting code with meaningful names
- Use full words for naming (avoid abbreviations like `Ctx` for `Context`)

### C# Conventions

#### Naming

| Element | Convention | Example |
|---------|------------|---------|
| Namespaces | PascalCase | `CivitaiSharp.Core.Models` |
| Classes/Records | PascalCase | `ModelBuilder` |
| Interfaces | IPascalCase | `IApiClient` |
| Methods | PascalCase | `ExecuteAsync` |
| Properties | PascalCase | `IsSuccess` |
| Private fields | _camelCase | `_httpClient` |
| Parameters | camelCase | `cancellationToken` |
| Constants | PascalCase | `DefaultTimeout` |

#### Code Style

- Use file-scoped namespaces
- Order usings: System, then third-party, then project namespaces
- Use 4 spaces for indentation
- Always use braces for control statements
- Use `var` when the type is obvious from context

#### Response DTOs (Core Library)

Use primary constructor records:

```csharp
public sealed record Model(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] ModelType Type);
```

#### Request DTOs (SDK Library)

Use classes with init-only properties:

```csharp
public sealed class TextToImageJobRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("params")]
    public ImageJobParams? Params { get; init; }
}
```

### Error Handling

- Use `ArgumentNullException.ThrowIfNull()` for null checks
- Use `ArgumentException.ThrowIfNullOrWhiteSpace()` for string validation
- Return `Result<T>` for expected failures
- Throw exceptions only for programming errors

### Async Code

- All async methods end with `Async` suffix
- Always accept and propagate `CancellationToken`
- Use `ConfigureAwait(false)` in library code
- Never use fire-and-forget patterns

### XML Documentation

All public APIs require XML documentation:

```csharp
/// <summary>
/// Filters models by the specified model type.
/// </summary>
/// <param name="type">The model type to filter by.</param>
/// <returns>A new builder instance with the filter applied.</returns>
public ModelBuilder WhereType(ModelType type)
```

## Testing

### Test Structure

- Test projects: `[ProjectName].Tests`
- Test classes mirror source: `ModelBuilder` to `ModelBuilderTests`
- Test names describe behavior: `WhenValidModelId_ReturnsModel`

### Writing Tests

Follow the Arrange-Act-Assert pattern:

```csharp
[Fact]
public async Task WhenValidModelId_ReturnsModel()
{
    // Arrange
    var client = CreateTestClient();

    // Act
    var result = await client.Models.WhereModelId(12345).ExecuteAsync();

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(12345, result.Value.Id);
}
```

### Test Guidelines

- One behavior per test
- Avoid branching logic in tests
- Tests should run independently and in parallel
- Mock only external dependencies, not internal code
- Use specific assertions that match the test name

### Running Tests

```shell
# Run all tests
dotnet test

# Run with coverage
dotnet tool install -g dotnet-coverage
dotnet-coverage collect -f cobertura -o coverage.xml dotnet test

# Run specific test project
dotnet test Tests/CivitaiSharp.Core.Tests
```

## Documentation

### DocFX Site

The documentation site uses DocFX with the modern template.

**Structure**:
```
Documentation/
  Api/          Auto-generated API reference
  Guides/       Tutorials and how-to guides
  toc.yml       Main navigation
  docfx.json    DocFX configuration
```

**Building Documentation**:
```shell
cd Documentation
docfx build
docfx serve _site
```

### Writing Guides

- Place guides in `Documentation/Guides/`
- Update `Documentation/Guides/toc.yml` for navigation
- Use clear headings and code examples
- Link to related API documentation using `[text](xref:Namespace.Type)`

## Pull Requests

### Before Submitting

1. Ensure your code follows the standards above
2. Add or update tests for your changes
3. Update documentation if needed
4. Verify all tests pass: `dotnet test`
5. Verify no build warnings: `dotnet build --warnaserror`

### PR Description

Include:
- A clear description of the changes
- The motivation and context
- Any breaking changes
- Related issue numbers (e.g., "Fixes #123")

### Review Process

1. Automated checks must pass
2. Code review by maintainers
3. Address feedback promptly
4. Squash commits on merge

## Reporting Issues

### Bug Reports

Include:
- Clear description of the bug
- Steps to reproduce
- Expected vs actual behavior
- Environment (OS, .NET version, package version)
- Minimal code sample if applicable

### Feature Requests

Include:
- The problem you are trying to solve
- Your proposed solution
- Alternative approaches considered
- Additional context

## Questions

Open a GitHub Discussion for questions about:
- Usage and best practices
- Feature ideas and feedback
- General project direction

---

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE.md).
