using Microsoft.JSInterop;

namespace ConventionGradingSystem.Host.Services;

public class FileDownloader
{
    private const string _functionName = "downloadFileFromStream";
    private const string _excelFileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IJSRuntime _jsRuntime;

    public FileDownloader(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task DownloadExcelFile(string fileName, byte[] fileData)
    {
        using var memoryStream = new MemoryStream(fileData);
        using var streamReference = new DotNetStreamReference(memoryStream);

        await _jsRuntime.InvokeVoidAsync(_functionName, fileName, _excelFileType, streamReference);
    }
}
