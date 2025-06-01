using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;
using SystemPraktykZawodowych.Data;
using SystemPraktykZawodowych.Data.Repositories;
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

            // Получаем нужные сервисы/репозитории из DI
            var registrationRepository = serviceProvider.GetRequiredService<IRegistrationRepository>();

            // Получить все регистрации
            var allRegistrations = await registrationRepository.GetAllAsync();
            if (allRegistrations != null)
            {
                foreach (var reg in allRegistrations)
                {
                    Console.WriteLine($"Registration ID: {reg.registration_id}, Student ID: {reg.student_id}, Company ID: {reg.company_id}");
                }
            }
            else
            {
                Console.WriteLine("No registrations found.");
            }

            // Получить регистрацию по Id
            int someId = 1;
            var registrationById = await registrationRepository.GetRegistrationByIdAsync(someId);
            if (registrationById != null)
            {
                Console.WriteLine($"Found registration with ID {someId}: Student ID = {registrationById.student_id}, Company ID = {registrationById.company_id}");
            }
            else
            {
                Console.WriteLine($"No registration found with ID {someId}");
            }

            // Обновить регистрацию
            if (registrationById != null)
            {
                registrationById.agreement_generated = 1; // Например, поменяли флаг
                registrationById.agreement_generated_date = DateTime.UtcNow;

                bool updateResult = await registrationRepository.UpdateAsync(registrationById);
                Console.WriteLine(updateResult ? "Registration updated successfully." : "Failed to update registration.");
            }

            // Удалить регистрацию по Id
            int idToDelete = 2;
            bool deleteResult = await registrationRepository.DeleteAsync(idToDelete);
            Console.WriteLine(deleteResult ? $"Registration with ID {idToDelete} deleted." : $"Failed to delete registration with ID {idToDelete}.");

            Console.ReadLine();
        }
    }
}
