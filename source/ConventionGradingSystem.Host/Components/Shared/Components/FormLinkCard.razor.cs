using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

[SuppressMessage("Performance", "CA1863:Use 'CompositeFormat'")]
public partial class FormLinkCard
{
    private const string _imageFormat = "data:image/svg+xml;base64,{0}";

    private readonly QRCodeGenerator _qrCodeGenerator;
    private readonly FileDownloader _fileDownloader;
    private readonly NavigationManager _navigationManager;

    public FormLinkCard(
        QRCodeGenerator qrCodeGenerator,
        FileDownloader fileDownloader,
        NavigationManager navigationManager)
    {
        _qrCodeGenerator = qrCodeGenerator;
        _fileDownloader = fileDownloader;
        _navigationManager = navigationManager;
    }

    [CascadingParameter]
    private IMudDialogInstance DialogInstance { get; set; } = null!;

    [Parameter]
    public FormLinkCardParameters Parameters { get; set; } = null!;

    private string? QRCodeImage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Parameters is null)
        {
            DialogInstance.Cancel();
            return;
        }

        var absoluteFormUri = _navigationManager.ToAbsoluteUri(Parameters.FormLink);
        var qrCodeData = await _qrCodeGenerator.GenerateSVGInBase64(absoluteFormUri.ToString());

        QRCodeImage = string.Format(CultureInfo.InvariantCulture, _imageFormat, qrCodeData);
    }

    private async Task DownloadQRCode()
    {
        var absoluteFormUri = _navigationManager.ToAbsoluteUri(Parameters.FormLink);

        var fileName = $"{Parameters.EntityName} - {Parameters.FormType}.png";
        var fileData = await _qrCodeGenerator.GeneratePNGImage(absoluteFormUri.ToString());

        await _fileDownloader.DownloadPNGFile(new FileDownloader.File(fileName, fileData));
    }
}
