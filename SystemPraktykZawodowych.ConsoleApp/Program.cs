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

            var registrationService = serviceProvider.GetRequiredService<IRegistrationService>();
            var allRegistrations = await registrationService.GetAllRegistrationsAsync();
            if (allRegistrations != null)
            {
                foreach (var reg in allRegistrations)
                {
                    Console.WriteLine($"Registration ID: {reg.RegistrationId}, Student ID: {reg.StudentId}, Company ID: {reg.CompanyId}");
                }
            }
            else
            {
                Console.WriteLine("No registrations found.");
            }
            
            var newRegistration = new Registration
            {
                StudentId = 5, // Example student ID
                CompanyId = 1, // Example company ID
            };
            
            bool addResult = await registrationService.AddRegistrationAsync(newRegistration);
            if (addResult)
            {
                Console.WriteLine("Registration added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add registration.");
            }
            
            // Email sending example
            var AllRegistrations = await registrationService.GetAllRegistrationsAsync();
            if (AllRegistrations != null && AllRegistrations.Count > 0)
            {
                foreach (var registration in AllRegistrations)
                {
                    Console.WriteLine($"Registration ID: {registration.RegistrationId}, Student ID: {registration.StudentId}, Company ID: {registration.CompanyId}");
                }
            }
            else
            {
                Console.WriteLine("No registrations available to send agreements.");
            }

            Console.WriteLine("Do jakiej rejestracji chcesz wysłać umowę? (Podaj ID rejestracji)");
            if (int.TryParse(Console.ReadLine(), out int registrationId))
            {
                var sendAgreementResult = await registrationService.SendAgreementAsync(registrationId);
                if (sendAgreementResult.Success)
                {
                    Console.WriteLine("Agreement sent successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to send agreement: {sendAgreementResult.ErrorMessage}");
                }
            }
            else
            {
                Console.WriteLine("Invalid registration ID.");
            }
            
          
        }
    }
}
