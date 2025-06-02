using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Models;
using SystemPraktykZawodowych.Data.Repositories;
using SystemPraktykZawodowych.Data;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Service.Services;
using SystemPraktykZawodowych.Core.Interfaces;
using SystemPraktykZawodowych.ConsoleApp;  // чтобы увидеть RegistrationConsole
using RegistrationConsoleApp;


namespace SystemPraktykZawodowych.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<DatabaseConnection>()
                .AddSingleton<IConfiguration>(configuration)

                // Student
                .AddScoped<IStudentRepository, StudentRepository>()
                .AddScoped<IStudentService, StudentService>()

                // Company
                .AddScoped<ICompanyRepository, CompanyRepository>()
                .AddScoped<ICompanyService, CompanyService>()

                // Registration
                .AddScoped<IRegistrationRepository, RegistrationRepository>()
                .AddScoped<IRegistrationService, RegistrationService>()

                // Email Sender
                .AddScoped<IEmailSender, EmailSenderService>()

                // PDF Generator
                .AddScoped<IAgreementGeneratorService, AgreementGeneratorService>()

                // Registration Console — добавляем!
                .AddScoped <ConsoleApplication>()

                .BuildServiceProvider();

            var console = serviceProvider.GetRequiredService<ConsoleApplication>();


            await console.RunAsync();

            Console.ReadLine();
        }
    }
}
