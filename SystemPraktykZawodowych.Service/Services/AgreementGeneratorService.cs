using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SystemPraktykZawodowych.Core.Interfaces.Services;

namespace SystemPraktykZawodowych.Service.Services;

public class AgreementGeneratorService : IAgreementGeneratorService
{
    public byte[] GenerateAgreement()
    {
        
        // Jak tworzyć PDF
        // https://github.com/ststeiger/PdfSharpCore/blob/master/docs/PdfSharpCore/index.md#first-steps
        PdfDocument document = new PdfDocument();
        document.Info.Title = "SYSTEM PRAKTYK ZAWODOWYCH";
        
        // pusta strona
        PdfPage page = document.AddPage();
        
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
        
        gfx.DrawString(
            "System praktyk zawodowych!", font, XBrushes.Black,
            new XRect(0, 0, page.Width, page.Height),
            XStringFormats.Center);
        
        // TODO : Dodać wymagany tekst do umowy
        
        // Zapisywanie PDF w byte[] https://stackoverflow.com/questions/1073277/pdfsharp-save-to-memorystream
        // Służy to do tego by dodać PDF odrazu do maila, bez zapisywania pdf bezpośrednio na dysku
        using (MemoryStream stream = new MemoryStream())
        {
            document.Save(stream, false);
            return stream.ToArray();
        }
    }
}