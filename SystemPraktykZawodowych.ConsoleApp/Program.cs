using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;
using SystemPraktykZawodowych.Data;
using SystemPraktykZawodowych.Data.Repositories;
using SystemPraktykZawodowych.Service.Services;

namespace SystemPraktykZawodowych.ConsoleApp;


class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder() // Wczytywanie ustawień konfiguracyjnych np. Połączenie z bazą danych
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceProvider = new ServiceCollection()
            // Singleton - Obiekt ma jedną instancje w całej aplikacji
            .AddSingleton<DatabaseConnection>()
            .AddSingleton<IConfiguration>(configuration) // umożliwia odczyt ustawień z appsettings.json - jak mozna używać: https://learn.microsoft.com/pl-pl/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0
            // Scoped obiekt będzie tworzony raz na żądanie (request) lub zakres (scope)
            // Students
            .AddScoped<IStudentRepository, StudentRepository>()
            .AddScoped<IStudentService, StudentService>()
            //Company
            .AddScoped<ICompanyRepository, CompanyRepository>()
            .AddScoped<ICompanyService, CompanyService>()
            // Email Sender
            .AddScoped<IEmailSender, EmailSenderService>()
            // PDF
            .AddScoped<IAgreementGeneratorService, AgreementGeneratorService>()
            .BuildServiceProvider();


        var emailSenderServiceTest = serviceProvider.GetRequiredService<IEmailSender>();
        var pdfServiceTest = serviceProvider.GetRequiredService<IAgreementGeneratorService>();
        
        
        var message = await emailSenderServiceTest.SendEmailAsync("{email}", "test", "temat", pdfServiceTest.GenerateAgreement());
        if (message.Success)
        {
            Console.WriteLine("Wysłano");
        }
        else
        {
            Console.WriteLine(message.ErrorMessage);
        }
    }
}