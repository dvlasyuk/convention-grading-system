using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;

using Microsoft.JSInterop;

namespace ConventionGradingSystem.Host.Services;

[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
public class FileDownloader
{
    private const string _functionName = "downloadFileFromStream";

    private const string _excelFileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string _pngFileType = "image/png";
    private const string _zipFileType = "application/zip";

    private static readonly char[] _invalidCharacters = Path.GetInvalidFileNameChars();
    private const char _invalidCharacterReplacement = '_';

    private readonly IJSRuntime _jsRuntime;

    public FileDownloader(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task DownloadExcelFile([NotNull] File file)
    {
        await DownloadFile(file.FileName, _excelFileType, file.FileData);
    }

    public async Task DownloadPNGFile([NotNull] File file)
    {
        await DownloadFile(file.FileName, _pngFileType, file.FileData);
    }

    public async Task DownloadZIPArhive(string archiveName, [NotNull] IEnumerable<File> files)
    {
        using var archiveStream = new MemoryStream();
        using (var archive = new ZipArchive(
            stream: archiveStream,
            mode: ZipArchiveMode.Create,
            leaveOpen: true))
        {
            foreach (var file in files)
            {
                var safeFileName = GetSafeFileName(file.FileName);

                var archiveEntry = archive.CreateEntry(safeFileName);
                using var entryStream = await archiveEntry.OpenAsync();

                await entryStream.WriteAsync(file.FileData.AsMemory());
            }
        }

        var arhiveData = archiveStream.ToArray();
        await DownloadFile(archiveName, _zipFileType, arhiveData);
    }

    private async Task DownloadFile(string fileName, string fileType, byte[] fileData)
    {
        var safeFileName = GetSafeFileName(fileName);

        using var memoryStream = new MemoryStream(fileData);
        using var streamReference = new DotNetStreamReference(memoryStream);

        await _jsRuntime.InvokeVoidAsync(_functionName, safeFileName, fileType, streamReference);
    }

    private static string GetSafeFileName(string fileName)
    {
        return string.Create(
            length: fileName.Length,
            state: (
                Source: fileName,
                Invalid: _invalidCharacters,
                Replacement: _invalidCharacterReplacement),
            action: (span, state) =>
            {
                for (var characterNumber = 0; characterNumber < state.Source.Length; characterNumber++)
                {
                    var sourceCharacter = state.Source[characterNumber];
                    span[characterNumber] = state.Invalid.Contains(sourceCharacter)
                        ? state.Replacement
                        : sourceCharacter;
                }
            });
    }

    public record File(string FileName, byte[] FileData);
}
