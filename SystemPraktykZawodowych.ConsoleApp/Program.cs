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
            .AddScoped<IStudentRepository, StudentRepository>()
            .AddScoped<IStudentService, StudentService>()
            .AddScoped<ICompanyRepository, CompanyRepository>()
            .AddScoped<ICompanyService, CompanyService>()
            .BuildServiceProvider();

        //var studentServiceTest = serviceProvider.GetRequiredService<IStudentService>();
        //List<Student>? students = await studentServiceTest.GetAllStudentsAsync();

        //if (students != null && students.Count > 0)
        //{
        //    Console.WriteLine($"Znaleziono {students.Count} studentow:");
        //    foreach (var student in students)
        //    {
        //        Console.WriteLine($"ID: {student.Id}, Imię: {student.FirstName}, Nazwisko: {student.LastName}");
        //    }
        //}
        //else
        //{
        //    Console.WriteLine("Nie znaleziono żadnych studentów lub wystąpił błąd.");
        //}

        //Console.WriteLine("Wprowadź ID studenta, którego chcesz pobrać:");
        //int id = int.Parse(Console.ReadLine() ?? "0");
        //var studentid1 = await studentServiceTest.GetStudentByIdAsync(id);
        //if (studentid1 != null)
        //{
        //    Console.WriteLine($"Znaleziono studenta: {studentid1.FirstName} {studentid1.LastName}");
        //}
        //else
        //{
        //    Console.WriteLine("Nie znaleziono studenta o podanym ID.");
        //}

        //Console.WriteLine("Podaj id studenta którego chcesz usunąć:");
        //int studentIdToDelete = int.Parse(Console.ReadLine() ?? "0");
        //bool isDeleted = await studentServiceTest.DeleteStudentAsync(studentIdToDelete);
        //if (isDeleted)
        //{
        //    Console.WriteLine($"Student o ID {studentIdToDelete} został usunięty pomyślnie.");
        //}
        //else
        //{
        //    Console.WriteLine($"Nie udało się usunąć studenta o ID {studentIdToDelete}.");
        //}

        //Student newStudent = new()
        //{
        //    FirstName = "John",
        //    LastName = "Doe",
        //    Class = "3A",
        //    PhoneNumber = "123456789",
        //    Email = "john@gmail.com"
        //};

        //bool isAdded = await studentServiceTest.AddStudentAsync(newStudent);
        //if (isAdded)
        //{
        //    Console.WriteLine("Nowy student został dodany pomyślnie.");
        //}
        //else
        //{
        //    Console.WriteLine("Wystąpił błąd podczas dodawania nowego studenta.");
        //}

        var companyServiceTest = serviceProvider.GetRequiredService<ICompanyService>();
        var newCompany = new Company
        {
            Name = "Tech Solutions",
            SupervisorName = "Anna Kowalska",
            Address = "ul. Przykładowa 123, 00-001 Warszawa",
            MaxInternships = 5,
            CurrentInternships = 2
        };

        //bool isCompanyAdded = await companyServiceTest.AddAsync(newCompany);
        //if (isCompanyAdded)
        //{
        //    Console.WriteLine("Nowa firma została dodana pomyślnie.");
        //}
        //else
        //{
        //    Console.WriteLine("Wystąpił błąd podczas dodawania nowej firmy.");

        //}


        List<Company>? companies = await companyServiceTest.GetAllCompanyAsync();
        if (companies != null && companies.Count > 0)
        {
            Console.WriteLine($"Znaleziono {companies.Count} firm:");
            foreach (var company in companies)
            {
                Console.WriteLine($"ID: {company.Id}, Nazwa: {company.Name}, Opiekun: {company.SupervisorName}, Adres: {company.Address}, Maksymalna liczba praktyk: {company.MaxInternships}, Aktualna liczba praktyk: {company.CurrentInternships}");
            }
        }
        else
        {
            Console.WriteLine("Nie znaleziono żadnych firm lub wystąpił błąd.");
        }
    }
}