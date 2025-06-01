using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Models;
using SystemPraktykZawodowych.Data.Repositories;
using SystemPraktykZawodowych.Data;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Service.Services;
using SystemPraktykZawodowych.Core.Interfaces;

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

                .BuildServiceProvider();

           // Get the required services/repositories from DI
            var registrationRepository = serviceProvider.GetRequiredService<IRegistrationRepository>();

            // Get all registrations
            var allRegistrations = await registrationRepository.GetAllAsync();
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

            // Get registration by Id
            int someId = 4;
            var registrationById = await registrationRepository.GetRegistrationByIdAsync(someId);
            if (registrationById != null)
            {
                Console.WriteLine($"Found registration with ID {someId}: Student ID = {registrationById.StudentId}, Company ID = {registrationById.CompanyId}");
            }
            else
            {
                Console.WriteLine($"No registration found with ID {someId}");
            }

            // Update registration
            // if (registrationById != null)
            // {
            //     registrationById.AgreementGenerated = 1; // For example, changed the flag
            //     registrationById.AgreementGeneratedDate = DateTime.UtcNow;
            //
            //     bool updateResult = await registrationRepository.UpdateAsync(registrationById);
            //     Console.WriteLine(updateResult ? "Registration updated successfully." : "Failed to update registration.");
            // }
            //
            // // Delete registration by Id
            // int idToDelete = 2;
            // bool deleteResult = await registrationRepository.DeleteAsync(idToDelete);
            // Console.WriteLine(deleteResult ? $"Registration with ID {idToDelete} deleted." : $"Failed to delete registration with ID {idToDelete}.");

            Console.ReadLine(); // so that the console does not close immediately
        }
    }
}
