using System.Text;

namespace ConventionGradingSystem.Host.Services;

public class QRCodeGenerator
{
    private const QRCoder.QRCodeGenerator.ECCLevel _eccLevelForSVGFormat = QRCoder.QRCodeGenerator.ECCLevel.M;
    private const QRCoder.QRCodeGenerator.ECCLevel _eccLevelForPNGFormat = QRCoder.QRCodeGenerator.ECCLevel.Q;

    private const int _moduleSize = 20;

    public Task<string> GenerateSVGInBase64(string payload)
    {
        return Task.Run(() =>
        {
            using var generator = new QRCoder.QRCodeGenerator();
            using var codeData = generator.CreateQrCode(payload, _eccLevelForSVGFormat);
            using var svgCode = new QRCoder.SvgQRCode(codeData);

            var svgString = svgCode.GetGraphic();
            var svgBytes = Encoding.UTF8.GetBytes(svgString);

            return Convert.ToBase64String(svgBytes);
        });
    }

    public Task<byte[]> GeneratePNGImage(string payload)
    {
        return Task.Run(() =>
        {
            using var generator = new QRCoder.QRCodeGenerator();
            using var codeData = generator.CreateQrCode(payload, _eccLevelForPNGFormat);
            using var pngCode = new QRCoder.PngByteQRCode(codeData);

            return pngCode.GetGraphic(_moduleSize);
        });
    }
}
