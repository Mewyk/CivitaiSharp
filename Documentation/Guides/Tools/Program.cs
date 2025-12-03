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

var apiClient = host.Services.GetRequiredService<IApiClient>();
var hashingService = host.Services.GetRequiredService<IFileHashingService>();
var downloadService = host.Services.GetRequiredService<IDownloadService>();

// #region hash-file
// Compute SHA256 hash of a file
var hashResult = await hashingService.ComputeHashAsync(
    @"C:\Models\model.safetensors",
    HashAlgorithm.Sha256);

if (hashResult is Result<HashedFile>.Success hashSuccess)
{
    Console.WriteLine($"File: {hashSuccess.Data.FilePath}");
    Console.WriteLine($"Hash: {hashSuccess.Data.Hash}");
    Console.WriteLine($"Algorithm: {hashSuccess.Data.Algorithm}");
    Console.WriteLine($"Size: {hashSuccess.Data.FileSize:N0} bytes");
    Console.WriteLine($"Time: {hashSuccess.Data.ComputationTime.TotalMilliseconds:F2}ms");
}
else if (hashResult is Result<HashedFile>.Failure hashFailure)
{
    Console.WriteLine($"Error: {hashFailure.Error.Message}");
}
// #endregion hash-file

// #region hash-stream
// Compute hash from a stream
await using var stream = File.OpenRead(@"C:\Models\model.safetensors");

var streamResult = await hashingService.ComputeHashAsync(stream, HashAlgorithm.Blake3);

if (streamResult is Result<HashedFile>.Success streamSuccess)
{
    Console.WriteLine($"BLAKE3 Hash: {streamSuccess.Data.Hash}");
}
// #endregion hash-stream

// #region verify-download
// Verify a downloaded model against Civitai's hash
var modelResult = await apiClient.Models.GetByIdAsync(123456);

if (modelResult is Result<Model>.Success modelSuccess)
{
    var version = modelSuccess.Data.ModelVersions?.FirstOrDefault();
    var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
    
    if (file?.Hashes?.Sha256 is { } civitaiHash)
    {
        var verifyResult = await hashingService.ComputeHashAsync(
            @"C:\Models\downloaded_model.safetensors",
            HashAlgorithm.Sha256);
        
        if (verifyResult is Result<HashedFile>.Success verifySuccess)
        {
            var isValid = string.Equals(
                verifySuccess.Data.Hash,
                civitaiHash,
                StringComparison.OrdinalIgnoreCase);
            
            Console.WriteLine(isValid
                ? "File integrity verified!"
                : $"Hash mismatch! Expected: {civitaiHash}, Got: {verifySuccess.Data.Hash}");
        }
    }
}
// #endregion verify-download

// #region multiple-hashes
// Compute multiple hash types for comparison
var filePath = @"C:\Models\model.safetensors";

var sha256Task = hashingService.ComputeHashAsync(filePath, HashAlgorithm.Sha256);
var blake3Task = hashingService.ComputeHashAsync(filePath, HashAlgorithm.Blake3);
var crc32Task = hashingService.ComputeHashAsync(filePath, HashAlgorithm.Crc32);

await Task.WhenAll(sha256Task, blake3Task, crc32Task);

Console.WriteLine("Hash comparison:");
if (sha256Task.Result is Result<HashedFile>.Success s256)
    Console.WriteLine($"  SHA256: {s256.Data.Hash}");
if (blake3Task.Result is Result<HashedFile>.Success b3)
    Console.WriteLine($"  BLAKE3: {b3.Data.Hash}");
if (crc32Task.Result is Result<HashedFile>.Success crc)
    Console.WriteLine($"  CRC32:  {crc.Data.Hash}");
// #endregion multiple-hashes

// #region download-image
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
// #endregion download-image

// #region download-image-custom
// Download an image to a custom directory
var imageCustomResult = await apiClient.Images
    .WhereUsername("civitai")
    .ExecuteAsync(resultsLimit: 1);

if (imageCustomResult is Result<PagedResult<Image>>.Success customSuccess)
{
    var image = customSuccess.Data.Items.FirstOrDefault();
    if (image is not null)
    {
        var downloadResult = await downloadService.DownloadAsync(
            image,
            @"D:\CustomImages");
        
        if (downloadResult is Result<DownloadedFile>.Success dlSuccess)
        {
            Console.WriteLine($"Downloaded to custom path: {dlSuccess.Data.FilePath}");
        }
    }
}
// #endregion download-image-custom

// #region download-model
// Download a model file (without version context)
var modelFileResult = await apiClient.Models.GetByIdAsync(4201);

if (modelFileResult is Result<Model>.Success mfSuccess)
{
    var version = mfSuccess.Data.ModelVersions?.FirstOrDefault();
    var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
    
    if (file is not null)
    {
        var downloadResult = await downloadService.DownloadAsync(file);
        
        if (downloadResult is Result<DownloadedFile>.Success dlSuccess)
        {
            Console.WriteLine($"Downloaded: {dlSuccess.Data.FilePath}");
            Console.WriteLine($"Verified: {dlSuccess.Data.IsVerified}");
        }
    }
}
// #endregion download-model

// #region download-model-version
// Download a model file with version context for better path organization
var modelVersionResult = await apiClient.Models.GetByIdAsync(4201);

if (modelVersionResult is Result<Model>.Success mvSuccess)
{
    var version = mvSuccess.Data.ModelVersions?.FirstOrDefault();
    var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
    
    if (file is not null && version is not null)
    {
        // Providing version enables additional path tokens like {ModelName}, {BaseModel}, etc.
        var downloadResult = await downloadService.DownloadAsync(file, version);
        
        if (downloadResult is Result<DownloadedFile>.Success dlSuccess)
        {
            Console.WriteLine($"Downloaded: {dlSuccess.Data.FilePath}");
            Console.WriteLine($"Size: {dlSuccess.Data.SizeBytes:N0} bytes");
            Console.WriteLine($"Verified: {dlSuccess.Data.IsVerified}");
            if (dlSuccess.Data.ComputedHash is not null)
            {
                Console.WriteLine($"Hash: {dlSuccess.Data.ComputedHash}");
            }
        }
        else if (downloadResult is Result<DownloadedFile>.Failure dlFailure)
        {
            Console.WriteLine($"Download failed: {dlFailure.Error.Message}");
        }
    }
}
// #endregion download-model-version

// #region download-url
// Download from a raw URL with hash verification
var rawUrl = "https://civitai.com/api/download/models/130072";
var destinationPath = @"C:\Downloads\model.safetensors";
var expectedHash = "abc123def456...";

var urlResult = await downloadService.DownloadAsync(
    rawUrl,
    destinationPath,
    expectedHash,
    HashAlgorithm.Sha256);

if (urlResult is Result<DownloadedFile>.Success urlSuccess)
{
    Console.WriteLine($"Downloaded and verified: {urlSuccess.Data.FilePath}");
}
// #endregion download-url

// #region html-markdown
// Convert HTML description to Markdown
var htmlModelResult = await apiClient.Models.GetByIdAsync(4201);

if (htmlModelResult is Result<Model>.Success htmlSuccess)
{
    var model = htmlSuccess.Data;
    
    // Using the static parser directly
    var markdown = HtmlParser.ToMarkdown(model.Description);
    
    Console.WriteLine("# Model Description (Markdown)");
    Console.WriteLine(markdown);
}
// #endregion html-markdown

// #region html-plaintext
// Convert HTML description to plain text
var plainModelResult = await apiClient.Models.GetByIdAsync(4201);

if (plainModelResult is Result<Model>.Success plainSuccess)
{
    var model = plainSuccess.Data;
    
    // Using the static parser
    var plainText = HtmlParser.ToPlainText(model.Description);
    
    Console.WriteLine("Model Description (Plain Text):");
    Console.WriteLine(plainText);
}
// #endregion html-plaintext

// #region html-extensions
// Using extension methods for cleaner code
var extModelResult = await apiClient.Models.GetByIdAsync(4201);

if (extModelResult is Result<Model>.Success extSuccess)
{
    var model = extSuccess.Data;
    
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
// #endregion html-extensions
