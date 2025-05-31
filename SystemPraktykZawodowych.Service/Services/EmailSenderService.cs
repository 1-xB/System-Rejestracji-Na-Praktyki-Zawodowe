using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using SystemPraktykZawodowych.Core.Interfaces.Services;

namespace SystemPraktykZawodowych.Service.Services;

public class EmailSenderService : IEmailSender
{
    public IConfiguration Configuration { get; set; }
    
    public EmailSenderService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public async Task<(bool Success, string? ErrorMessage)> SendEmailAsync(string receiver, string body, string subject, byte[]? pdfBytes)
    {
        if (string.IsNullOrEmpty(receiver))
            return (false, "The recipient's address is required");
        
        if (string.IsNullOrEmpty(subject))
            return (false, "The subject of the message is required");
        
        if (pdfBytes is null || pdfBytes.Length == 0)
            return (false, "No PDF attachment or the attachment is empty");
        
        try
        {
            var systemEmail = Configuration["EmailSender:Email"];
            var password = Configuration["EmailSender:Password"];

            if (systemEmail is null || password is null)
                return (false, "There was a problem with the settings of the email sending system");
            
            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(systemEmail, password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using var mail = new MailMessage(systemEmail, receiver)
            {
                Subject = subject,
                Body = body
            };
                
                
            using var stream = new MemoryStream(pdfBytes);
            mail.Attachments.Add(new Attachment(stream, "umowa.pdf", "application/pdf"));
                
                
            await client.SendMailAsync(mail);
            return (true, null);

        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
        
    }
    
}