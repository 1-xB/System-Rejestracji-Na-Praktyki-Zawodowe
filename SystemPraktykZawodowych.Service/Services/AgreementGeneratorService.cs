using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Service.Services;

public class AgreementGeneratorService : IAgreementGeneratorService
{
    public ICompanyService CompanyService { get; set; }
    public IStudentService StudentService { get; set; }

    public AgreementGeneratorService(ICompanyService companyService, IStudentService studentService)
    {
        CompanyService = companyService;
        StudentService = studentService;
    }
    public async Task<byte[]> GenerateAgreement(Registration registration)
    {
        var student = await StudentService.GetStudentByIdAsync(registration.StudentId);
        var company = await CompanyService.GetCompanyByIdAsync(registration.CompanyId);
        
        // Jak tworzyć PDF
        // https://github.com/ststeiger/PdfSharpCore/blob/master/docs/PdfSharpCore/index.md#first-steps
            PdfDocument document = new PdfDocument();
        document.Info.Title = "Umowa o praktyki zawodowe";

        PdfPage page = document.AddPage();
        page.Size = PdfSharpCore.PageSize.A4;

        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Czcionki
        XFont titleFont = new XFont("Arial", 14, XFontStyle.Bold);
        XFont normalFont = new XFont("Arial", 11, XFontStyle.Regular);
        XFont boldFont = new XFont("Arial", 11, XFontStyle.Bold);

        double marginLeft = 50;
        double currentY = 50;
        double lineHeight = 20;

        // Tytuł
        gfx.DrawString("UMOWA O PRAKTYKI ZAWODOWE", titleFont, XBrushes.Black,
            new XPoint(page.Width / 2, currentY), XStringFormats.TopCenter);
        currentY += lineHeight * 2;

        // Data
        gfx.DrawString($"Data: {DateTime.Now:dd.MM.yyyy}", normalFont, XBrushes.Black,
            new XPoint(marginLeft, currentY));
        currentY += lineHeight * 2;

        // Firma
        gfx.DrawString("Firma:", boldFont, XBrushes.Black, new XPoint(marginLeft, currentY));
        currentY += lineHeight;
        gfx.DrawString($"{company.Name}, {company.Address}", normalFont, XBrushes.Black, 
            new XPoint(marginLeft, currentY));
        currentY += lineHeight;
        gfx.DrawString($"Przedstawiciel: {company.SupervisorName}", normalFont, XBrushes.Black,
            new XPoint(marginLeft, currentY));
        currentY += lineHeight * 2;

        // Student
        gfx.DrawString("Praktykant:", boldFont, XBrushes.Black, new XPoint(marginLeft, currentY));
        currentY += lineHeight;
        gfx.DrawString($"{student.FirstName} {student.LastName}", normalFont, XBrushes.Black,
            new XPoint(marginLeft, currentY));
        currentY += lineHeight;
        gfx.DrawString($"Klasa: {student.Class}, Kontakt: {student.Email}", normalFont, XBrushes.Black,
            new XPoint(marginLeft, currentY));
        currentY += lineHeight * 2;

        // Treść umowy 
        gfx.DrawString("Warunki umowy:", boldFont, XBrushes.Black, new XPoint(marginLeft, currentY));
        currentY += lineHeight;
        gfx.DrawString("1. Firma przyjmuje Praktykanta na praktyki zawodowe.", normalFont,
            XBrushes.Black, new XPoint(marginLeft, currentY));
        currentY += lineHeight;
        gfx.DrawString($"2. Data rozpoczęcia praktyk: {registration.RegistrationDate:dd.MM.yyyy}", normalFont,
            XBrushes.Black, new XPoint(marginLeft, currentY));
        currentY += lineHeight * 3;

        // Podpisy
        gfx.DrawString(".................", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
        gfx.DrawString(".................", normalFont, XBrushes.Black, new XPoint(page.Width - marginLeft - 70, currentY));
        currentY += lineHeight;
        
        gfx.DrawString("Praktykant", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
        gfx.DrawString("Firma", normalFont, XBrushes.Black, new XPoint(page.Width - marginLeft - 70, currentY));

        // Zapisywanie PDF w byte[] https://stackoverflow.com/questions/1073277/pdfsharp-save-to-memorystream
        // Służy to do tego by dodać PDF odrazu do maila, bez zapisywania pdf bezpośrednio na dysku
        using (MemoryStream stream = new MemoryStream())
        {
            document.Save(stream, false);
            return stream.ToArray();
        }
    }
}