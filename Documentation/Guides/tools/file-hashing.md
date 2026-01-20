---
title: File Hashing
description: Compute cryptographic hashes for verification.
---

# File Hashing

Compute cryptographic hashes to verify downloaded files against Civitai metadata.

## Supported Algorithms

- `Sha256` - 64 hex characters
- `Sha512` - 128 hex characters
- `Blake3` - 64 hex characters (fast, modern)
- `Crc32` - 8 hex characters (integrity check)

## Hash File

[!code-csharp[Program.cs](examples/Tools/Program.cs#HashFile)]

## Hash Stream

[!code-csharp[Program.cs](examples/Tools/Program.cs#HashStream)]

## Verify Downloads

[!code-csharp[Program.cs](examples/Tools/Program.cs#VerifyDownload)]

## Multiple Hashes

[!code-csharp[Program.cs](examples/Tools/Program.cs#MultipleHashes)]

## Async with Cancellation

[!code-csharp[Program.cs](examples/Tools/Program.cs#AsyncCancellationExample)]

## Error Handling

**Error Codes:** `FileNotFound`, `StreamNotReadable`, `HashComputationFailed`

[!code-csharp[Program.cs](examples/Tools/Program.cs#ErrorHandlingSwitchExample)]

## Integration with Downloads

[!code-csharp[Program.cs](examples/Tools/Program.cs#IntegrationExample)]

## Next Steps

- [Downloading Files](downloading-files.md)
- [HTML Parsing](html-parsing.md)
