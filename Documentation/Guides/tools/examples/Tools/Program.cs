using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Tools.Downloads;
using CivitaiSharp.Tools.Extensions;
using CivitaiSharp.Tools.Hashing;
using CivitaiSharp.Tools.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Register Core API client
builder.Services.AddCivitaiApi();

// Register Tools services with configuration
builder.Services.AddCivitaiDownloads(options =>
{
    options.Images.BaseDirectory = @"C:\Downloads\Images";
    options.Images.PathPattern = "{Username}/{Id}.{Extension}";
    
    options.Models.BaseDirectory = @"C:\Downloads\Models";
    options.Models.PathPattern = "{ModelType}/{ModelName}/{FileName}";
    options.Models.VerifyHash = true;
    options.Models.HashAlgorithm = HashAlgorithm.Sha256;
});

var host = builder.Build();

await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();
var hashingService = host.Services.GetRequiredService<IFileHashingService>();
var downloadService = host.Services.GetRequiredService<IDownloadService>();

#region HashFile
// Compute SHA256 hash of a file
var hashResult = await hashingService.ComputeHashAsync(
    @"C:\Models\model.safetensors",
    HashAlgorithm.Sha256);

if (hashResult is Result<HashedFile>.Success hashingSuccess)
{
    Console.WriteLine($"File: {hashingSuccess.Data.FilePath}");
    Console.WriteLine($"Hash: {hashingSuccess.Data.Hash}");
    Console.WriteLine($"Algorithm: {hashingSuccess.Data.Algorithm}");
    Console.WriteLine($"Size: {hashingSuccess.Data.FileSize:N0} bytes");
    Console.WriteLine($"Time: {hashingSuccess.Data.ComputationTime.TotalMilliseconds:F2}ms");
}
else if (hashResult is Result<HashedFile>.Failure hashingFailure)
{
    Console.WriteLine($"Error: {hashingFailure.Error.Message}");
}
#endregion

#region HashStream
// Compute hash from a stream
await using var stream = File.OpenRead(@"C:\Models\model.safetensors");

var streamHashResult = await hashingService.ComputeHashAsync(stream, HashAlgorithm.Blake3);

if (streamHashResult is Result<HashedFile>.Success streamHashSuccess)
{
    Console.WriteLine($"BLAKE3 Hash: {streamHashSuccess.Data.Hash}");
}
#endregion

#region VerifyDownload
// Verify a downloaded model against Civitai's hash
var modelResult = await apiClient.Models.GetByIdAsync(123456);

if (modelResult is Result<Model>.Success modelSuccess)
{
    var version = modelSuccess.Data.ModelVersions?.FirstOrDefault();
    var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
    
    if (file?.Hashes?.Sha256 is { } civitaiHash)
    {
        var verificationResult = await hashingService.ComputeHashAsync(
            @"C:\Models\downloaded_model.safetensors",
            HashAlgorithm.Sha256);
        
        if (verificationResult is Result<HashedFile>.Success verificationSuccess)
        {
            var isValid = string.Equals(
                verificationSuccess.Data.Hash,
                civitaiHash,
                StringComparison.OrdinalIgnoreCase);
            
            Console.WriteLine(isValid
                ? "File integrity verified!"
                : $"Hash mismatch! Expected: {civitaiHash}, Got: {verificationSuccess.Data.Hash}");
        }
    }
}
#endregion

#region MultipleHashes
// Compute multiple hash types for comparison
var filePath = @"C:\Models\model.safetensors";

var sha256Task = hashingService.ComputeHashAsync(filePath, HashAlgorithm.Sha256);
var blake3Task = hashingService.ComputeHashAsync(filePath, HashAlgorithm.Blake3);
var crc32Task = hashingService.ComputeHashAsync(filePath, HashAlgorithm.Crc32);

await Task.WhenAll(sha256Task, blake3Task, crc32Task);

Console.WriteLine("Hash comparison:");
if (sha256Task.Result is Result<HashedFile>.Success sha256Success)
    Console.WriteLine($"  SHA256: {sha256Success.Data.Hash}");
if (blake3Task.Result is Result<HashedFile>.Success blake3Success)
    Console.WriteLine($"  BLAKE3: {blake3Success.Data.Hash}");
if (crc32Task.Result is Result<HashedFile>.Success crc32Success)
    Console.WriteLine($"  CRC32:  {crc32Success.Data.Hash}");
#endregion

#region DownloadImage
// Download an image using configured options
var imageResult = await apiClient.Images
    .ExecuteAsync(resultsLimit: 1);

if (imageResult is Result<PagedResult<Image>>.Success imageSuccess)
{
    var image = imageSuccess.Data.Items.FirstOrDefault();
    if (image is not null)
    {
        var downloadResult = await downloadService.DownloadAsync(image);
        
        if (downloadResult is Result<DownloadedFile>.Success downloadSuccess)
        {
            Console.WriteLine($"Downloaded to: {downloadSuccess.Data.FilePath}");
            Console.WriteLine($"Size: {downloadSuccess.Data.SizeBytes:N0} bytes");
        }
    }
}
#endregion

#region DownloadImageCustom
// Download an image to a custom directory
var imageCustomResult = await apiClient.Images
    .WhereUsername("Mewyk")
    .ExecuteAsync(resultsLimit: 1);

if (imageCustomResult is Result<PagedResult<Image>>.Success customPathSuccess)
{
    var image = customPathSuccess.Data.Items.FirstOrDefault();
    if (image is not null)
    {
        var downloadResult = await downloadService.DownloadAsync(
            image,
            @"D:\CustomImages");
        
        if (downloadResult is Result<DownloadedFile>.Success downloadSuccess)
        {
            Console.WriteLine($"Downloaded to custom path: {downloadSuccess.Data.FilePath}");
        }
    }
}
#endregion

#region DownloadModel
// Download a model file (without version context)
var modelFileResult = await apiClient.Models.GetByIdAsync(4201);

if (modelFileResult is Result<Model>.Success modelFileSuccess)
{
    var version = modelFileSuccess.Data.ModelVersions?.FirstOrDefault();
    var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
    
    if (file is not null)
    {
        var downloadResult = await downloadService.DownloadAsync(file);
        
        if (downloadResult is Result<DownloadedFile>.Success downloadSuccess)
        {
            Console.WriteLine($"Downloaded: {downloadSuccess.Data.FilePath}");
            Console.WriteLine($"Verified: {downloadSuccess.Data.IsVerified}");
        }
    }
}
#endregion

#region DownloadModelVersion
// Download a model file with version context for better path organization
var modelVersionResult = await apiClient.Models.GetByIdAsync(4201);

if (modelVersionResult is Result<Model>.Success modelVersionSuccess)
{
    var version = modelVersionSuccess.Data.ModelVersions?.FirstOrDefault();
    var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
    
    if (file is not null && version is not null)
    {
        // Providing version enables additional path tokens like {ModelName}, {BaseModel}, etc.
        var downloadResult = await downloadService.DownloadAsync(file, version);
        
        if (downloadResult is Result<DownloadedFile>.Success downloadSuccess)
        {
            Console.WriteLine($"Downloaded: {downloadSuccess.Data.FilePath}");
            Console.WriteLine($"Size: {downloadSuccess.Data.SizeBytes:N0} bytes");
            Console.WriteLine($"Verified: {downloadSuccess.Data.IsVerified}");
            if (downloadSuccess.Data.ComputedHash is not null)
            {
                Console.WriteLine($"Hash: {downloadSuccess.Data.ComputedHash}");
            }
        }
        else if (downloadResult is Result<DownloadedFile>.Failure downloadFailure)
        {
            Console.WriteLine($"Download failed: {downloadFailure.Error.Message}");
        }
    }
}
#endregion

#region DownloadUrl
// Download from a raw URL with hash verification
var rawUrl = "https://civitai.com/api/download/models/130072";
var destinationPath = @"C:\Downloads\model.safetensors";
var expectedHash = "abc123def456...";

var urlResult = await downloadService.DownloadAsync(
    rawUrl,
    destinationPath,
    expectedHash,
    HashAlgorithm.Sha256);

if (urlResult is Result<DownloadedFile>.Success urlDownloadSuccess)
{
    Console.WriteLine($"Downloaded and verified: {urlDownloadSuccess.Data.FilePath}");
}
#endregion

#region HtmlMarkdown
// Convert HTML description to Markdown
var htmlModelResult = await apiClient.Models.GetByIdAsync(4201);

if (htmlModelResult is Result<Model>.Success htmlModelSuccess)
{
    var model = htmlModelSuccess.Data;
    
    // Using the static parser directly
    var markdown = HtmlParser.ToMarkdown(model.Description);
    
    Console.WriteLine("# Model Description (Markdown)");
    Console.WriteLine(markdown);
}
#endregion

#region HtmlPlaintext
// Convert HTML description to plain text
var plainModelResult = await apiClient.Models.GetByIdAsync(4201);

if (plainModelResult is Result<Model>.Success plainTextSuccess)
{
    var model = plainTextSuccess.Data;
    
    // Using the static parser
    var plainText = HtmlParser.ToPlainText(model.Description);
    
    Console.WriteLine("Model Description (Plain Text):");
    Console.WriteLine(plainText);
}
#endregion

#region HtmlExtensions
// Using extension methods for cleaner code
var extModelResult = await apiClient.Models.GetByIdAsync(4201);

if (extModelResult is Result<Model>.Success extensionsSuccess)
{
    var model = extensionsSuccess.Data;
    
    // Extension methods on Model
    Console.WriteLine("## Model Description");
    Console.WriteLine(model.GetDescriptionAsMarkdown());
    
    // Extension methods on ModelVersion
    foreach (var version in model.ModelVersions ?? [])
    {
        if (!string.IsNullOrWhiteSpace(version.Description))
        {
            Console.WriteLine($"### {version.Name}");
            Console.WriteLine(version.GetDescriptionAsMarkdown());
        }
    }
}
#endregion

#region DefaultConfiguration
// Uses system temp directory with minimal patterns
builder.Services.AddCivitaiDownloads();
#endregion

#region FromConfiguration
builder.Services.AddCivitaiDownloads(builder.Configuration.GetSection("CivitaiDownloads"));
#endregion

#region ProgrammaticConfiguration
builder.Services.AddCivitaiDownloads(options =>
{
    options.Images.BaseDirectory = @"C:\Downloads\Images";
    options.Images.PathPattern = "{Username}/{Id}.{Extension}";
    options.Images.OverwriteExisting = false;
    
    options.Models.BaseDirectory = @"C:\Models";
    options.Models.PathPattern = "{ModelType}/{ModelName}/{FileName}";
    options.Models.OverwriteExisting = true;
    options.Models.VerifyHash = true;
    options.Models.HashAlgorithm = HashAlgorithm.Sha256;
});
#endregion

#region HashVerificationConfig
builder.Services.AddCivitaiDownloads(options =>
{
    options.Models.VerifyHash = true;
    options.Models.HashAlgorithm = HashAlgorithm.Sha256;
});
#endregion

#region VerificationResult
var verifyResult = await downloadService.DownloadAsync(file, version);

if (verifyResult is Result<DownloadedFile>.Success verifySuccess)
{
    if (verifySuccess.Data.IsVerified)
    {
        Console.WriteLine($"Verified hash: {verifySuccess.Data.ComputedHash}");
    }
    else
    {
        Console.WriteLine("Hash verification was not performed");
    }
}
#endregion

#region FileFormatDetection
// Detect from file path
var format = await FileFormatDetector.DetectFormatAsync(filePath);
Console.WriteLine($"Detected format: {format}"); // "png", "jpg", "mp4", etc.

// Detect from stream
await using var formatStream = File.OpenRead(filePath);
var formatFromStream = await FileFormatDetector.DetectFormatAsync(formatStream);

// Detect from bytes
var header = new byte[16];
await formatStream.ReadAsync(header);
var formatFromBytes = FileFormatDetector.DetectFormat(header.AsSpan());
#endregion

#region ErrorHandling
var errorResult = await downloadService.DownloadAsync(file, version);

switch (errorResult)
{
    case Result<DownloadedFile>.Success success:
        Console.WriteLine($"Downloaded: {success.Data.FilePath}");
        break;
        
    case Result<DownloadedFile>.Failure { Error.Code: ErrorCode.HashVerificationFailed } failure:
        Console.WriteLine($"Corrupted download: {failure.Error.Message}");
        break;
        
    case Result<DownloadedFile>.Failure { Error.Code: ErrorCode.IoError } failure:
        Console.WriteLine($"File exists: {failure.Error.Message}");
        break;
        
    case Result<DownloadedFile>.Failure failure:
        Console.WriteLine($"Download failed: {failure.Error.Message}");
        break;
}
#endregion

#region DisplayingInConsole
var consoleResult = await apiClient.Models.GetByIdAsync(123456);

if (consoleResult is Result<Model>.Success consoleSuccess)
{
    var model = consoleSuccess.Data;
    
    Console.WriteLine($"# {model.Name}");
    Console.WriteLine();
    Console.WriteLine(model.GetDescriptionAsPlainText());
}
#endregion

#region SavingAsMarkdownFile
var saveResult = await apiClient.Models.GetByIdAsync(123456);

if (saveResult is Result<Model>.Success saveSuccess)
{
    var model = saveSuccess.Data;
    var markdown = model.GetDescriptionAsMarkdown();
    
    await File.WriteAllTextAsync($"{model.Name}.md", markdown);
}
#endregion

#region ProcessingVersionDescriptions
var processResult = await apiClient.Models.GetByIdAsync(123456);

if (processResult is Result<Model>.Success processSuccess)
{
    var model = processSuccess.Data;
    
    foreach (var version in model.ModelVersions ?? [])
    {
        if (!string.IsNullOrWhiteSpace(version.Description))
        {
            Console.WriteLine($"## {version.Name}");
            Console.WriteLine(version.GetDescriptionAsMarkdown());
            Console.WriteLine();
        }
    }
}
#endregion

#region HandlingEmptyContent
var emptyMarkdown = HtmlParser.ToMarkdown(null);     // Returns ""
var emptyPlainText = HtmlParser.ToPlainText("");     // Returns ""
var whitespaceResult = HtmlParser.ToMarkdown("   ");  // Returns ""
#endregion

#region IntegrationWithDownloads
// Combine HTML parsing with download services for complete model documentation
async Task DownloadWithReadmeAsync(long modelId, string outputDirectory)
{
    var readmeResult = await apiClient.Models.GetByIdAsync(modelId);
    
    if (readmeResult is not Result<Model>.Success readmeSuccess)
        return;
        
    var model = readmeSuccess.Data;
    var version = model.ModelVersions?.FirstOrDefault();
    var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
    
    if (file is null || version is null)
        return;
    
    // Download the model file
    var downloadReadmeResult = await downloadService.DownloadAsync(file, version, outputDirectory);
    
    if (downloadReadmeResult is Result<DownloadedFile>.Success downloadReadmeSuccess)
    {
        // Create README.md alongside the model
        var directory = Path.GetDirectoryName(downloadReadmeSuccess.Data.FilePath);
        var readmePath = Path.Combine(directory!, "README.md");
        
        var readme = $"""
            # {model.Name}
            
            **Type:** {model.Type}
            **Creator:** {model.Creator?.Username}
            **Version:** {version.Name}
            **Base Model:** {version.BaseModel}
            
            ## Description
            
            {model.GetDescriptionAsMarkdown()}
            
            ## Version Notes
            
            {version.GetDescriptionAsMarkdown()}
            
            ## Trigger Words
            
            {string.Join(", ", version.TrainedWords ?? [])}
            """;
        
        await File.WriteAllTextAsync(readmePath, readme);
    }
}

await DownloadWithReadmeAsync(123456, @"C:\Downloads\Models");
#endregion

#region FileHashingServiceUsage
// Inject IFileHashingService to compute file hashes
public class MyHashingService(IFileHashingService hashingService)
{
    public async Task VerifyFileAsync(string filePath)
    {
        var result = await hashingService.ComputeHashAsync(filePath, HashAlgorithm.Sha256);
        
        if (result is Result<HashedFile>.Success success)
        {
            Console.WriteLine($"Hash: {success.Data.Hash}");
            Console.WriteLine($"Size: {success.Data.FileSize} bytes");
            Console.WriteLine($"Time: {success.Data.ComputationTime.TotalMilliseconds}ms");
        }
    }
}
#endregion

#region DownloadServiceUsage
// Inject IDownloadService and IApiClient to download model files
public class MyDownloadService(IDownloadService downloadService, IApiClient apiClient)
{
    public async Task DownloadModelAsync()
    {
        // Get a model version
        var modelResult = await apiClient.Models.GetByIdAsync(123456);
        if (modelResult is not Result<Model>.Success modelSuccess)
            return;
            
        var model = modelSuccess.Data;
        var version = model.ModelVersions?.FirstOrDefault();
        var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
        
        if (file is null || version is null)
            return;
        
        // Download with hash verification
        var result = await downloadService.DownloadAsync(file, version);
        
        if (result is Result<DownloadedFile>.Success success)
        {
            Console.WriteLine($"Downloaded to: {success.Data.FilePath}");
            Console.WriteLine($"Verified: {success.Data.IsVerified}");
        }
    }
}
#endregion

#region HtmlParserUsage
// Use HtmlParser to convert model descriptions
var markdown = HtmlParser.ToMarkdown(model.Description);
var plainText = HtmlParser.ToPlainText(model.Description);

// Or using extension methods
var markdown2 = model.GetDescriptionAsMarkdown();
var plainText2 = model.GetDescriptionAsPlainText();
#endregion

#region AsyncCancellationExample
// Hash a file with cancellation support
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

var cancellationResult = await hashingService.ComputeHashAsync(
    largeFilePath, 
    HashAlgorithm.Blake3, 
    cts.Token);
#endregion

#region ErrorHandlingSwitchExample
// Handle hashing errors with switch expression
var errorSwitchResult = await hashingService.ComputeHashAsync(filePath, HashAlgorithm.Sha256);

switch (errorSwitchResult)
{
    case Result<HashedFile>.Success success:
        Console.WriteLine($"Hash: {success.Data.Hash}");
        break;
    case Result<HashedFile>.Failure { Error.Code: ErrorCode.FileNotFound }:
        Console.WriteLine("File not found");
        break;
    case Result<HashedFile>.Failure failure:
        Console.WriteLine($"Error: {failure.Error.Message}");
        break;
}
#endregion

#region IntegrationExample
// Integrate hashing with download configuration
builder.Services.AddCivitaiDownloads(options =>
{
    options.Models.VerifyHash = true;
    options.Models.HashAlgorithm = HashAlgorithm.Sha256;
});
#endregion

await host.StopAsync();
