namespace SystemPraktykZawodowych.Core.Interfaces.Services;

public interface IEmailSender
{
    Task<(bool Success, string? ErrorMessage)> SendEmailAsync(string receiver, string body, string subject, byte[]? pdfBytes);
}