using System;
using System.Threading.Tasks;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;

namespace RegistrationConsoleApp
{
    public class ConsoleApplication
    {
        private readonly IRegistrationRepository _repository;
        private readonly IRegistrationService _registrationService;
        private readonly IStudentService _studentService;
        private readonly ICompanyService _companyService;
        

        public ConsoleApplication(IRegistrationRepository repository, 
                                  IRegistrationService registrationService,
                                  IStudentService studentService,
                                  ICompanyService companyService)
        {
            _repository = repository;
            _registrationService = registrationService;
            _studentService = studentService;
            _companyService = companyService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select an action:");
                Console.WriteLine("1. Show all registrations");
                Console.WriteLine("2. Find registration by ID");
                Console.WriteLine("3. Add registration");
                Console.WriteLine("4. Update registration");
                Console.WriteLine("5. Delete registration");
                // Console.WriteLine("6. Send agreement"); // TODO: Implementacja wysyłania umowy, czyli wyswietlenie listy rejestracji, i mozliwość wyboru rejestracji, której umowę chcemy wysłać.
                // TODO: Obsługa firm i studentow
                Console.WriteLine("0. Exit");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim();


                switch (choice)
                {
                    case "1":
                        var all = await _repository.GetAllAsync();
                        if (all == null || all.Count == 0)
                        {
                            Console.WriteLine("No registrations found.");
                        }
                        else
                        {
                            foreach (var r in all)
                            {
                                Console.WriteLine($"ID: {r.RegistrationId}, Student: {r.StudentId}, Company: {r.CompanyId}, Registration Date: {r.RegistrationDate}");
                            }
                        }
                        break;

                    case "2":
                        Console.Write("Enter registration ID: ");
                        var input = Console.ReadLine();
                        if (!int.TryParse(input, out int id))
                        {
                            Console.WriteLine("Invalid ID input.");
                            break;
                        }
                        var reg = await _repository.GetRegistrationByIdAsync(id);
                        if (reg != null)
                        {
                            Console.WriteLine($"ID: {reg.RegistrationId}, Student: {reg.StudentId}, Company: {reg.CompanyId}, Registration Date: {reg.RegistrationDate}");
                        }
                        else
                        {
                            Console.WriteLine("Registration not found.");
                        }
                        break;

                    case "3":
                        var newReg = new Registration();

                        Console.Write("Student ID: ");
                        var studentInput = Console.ReadLine();
                        if (!int.TryParse(studentInput, out int studentId))
                        {
                            Console.WriteLine("Invalid student ID.");
                            break;
                        }
                        if (await _studentService.GetStudentByIdAsync(studentId) == null)
                        {
                            Console.WriteLine("Student not found.");
                            break;
                        }
                        
                        newReg.StudentId = studentId;

                        Console.Write("Company ID: ");
                        var companyInput = Console.ReadLine();
                        if (!int.TryParse(companyInput, out int companyId))
                        {
                            Console.WriteLine("Invalid company ID.");
                            break;
                        }
                        var company = await _companyService.GetCompanyByIdAsync(companyId);
                        if (company == null)
                        {
                            Console.WriteLine("Company not found.");
                            break;
                        }
                        
                        // TODO : Sprawdzenie, czy firma ma wolne miejsca na praktyki
                        
                        newReg.CompanyId = companyId;

                        newReg.RegistrationDate = DateTime.UtcNow;
                        newReg.AgreementGenerated = 0;
                        newReg.AgreementGeneratedDate = null; // Umowa nie jest jeszcze wygenerowana

                        var added = await _registrationService.AddRegistrationAsync(newReg);
                        if (added)
                        {
                            Console.WriteLine("Registration added successfully.");
                            Console.WriteLine($"ID: {newReg.RegistrationId}, Student: {newReg.StudentId}, Company: {newReg.CompanyId}, Registration Date: {newReg.RegistrationDate}");
                            Console.WriteLine("Sending agreement...");
                            
                            var registrations = await _registrationService.GetAllRegistrationsAsync();
                            var newRegistration = registrations?
                                .Where(r => r.StudentId == newReg.StudentId && r.CompanyId == newReg.CompanyId)
                                .FirstOrDefault();

                            if (newRegistration != null)
                            {
                                var result = await _registrationService.SendAgreementAsync(newRegistration.RegistrationId);
                                if (result.Success)
                                {
                                    Console.WriteLine("Agreement sent successfully.");
                                }
                                else
                                {
                                    Console.WriteLine($"Error sending agreement: {result.ErrorMessage}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error finding the newly added registration.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error adding registration. Make sure that the same registration does not already exist in the database");
                        }
                        break;

                    case "4":
                        Console.Write("Enter registration ID to update: ");
                        var updInput = Console.ReadLine();
                        if (!int.TryParse(updInput, out int updId))
                        {
                            Console.WriteLine("Invalid registration ID.");
                            break;
                        }
                        var existing = await _repository.GetRegistrationByIdAsync(updId);
                        if (existing == null)
                        {
                            Console.WriteLine("Registration not found.");
                            break;
                        }

                        Console.Write("New student ID: ");
                        var updStudentInput = Console.ReadLine();
                        if (!int.TryParse(updStudentInput, out int updStudentId))
                        {
                            Console.WriteLine("Invalid student ID.");
                            break;
                        }
                        
                        if (await _studentService.GetStudentByIdAsync(updStudentId) == null)
                        {
                            Console.WriteLine("Student not found.");
                            break;
                        }

                        Console.Write("New company ID: ");
                        var updCompanyInput = Console.ReadLine();
                        if (!int.TryParse(updCompanyInput, out int updCompanyId))
                        {
                            Console.WriteLine("Invalid company ID.");
                            break;
                        }
                        
                        if (await _companyService.GetCompanyByIdAsync(updCompanyId) == null)
                        {
                            Console.WriteLine("Company not found.");
                            break;
                        }
                        
                        // TODO : Sprawdzenie, czy firma ma wolne miejsca na praktyki
                        
                        
                        existing.StudentId = updStudentId;
                        existing.CompanyId = updCompanyId;
                        existing.RegistrationDate = DateTime.UtcNow;
                        existing.AgreementGenerated = 0; 
                        existing.AgreementGeneratedDate = null; 

                        var updated = await _repository.UpdateAsync(existing);
                        if (updated)
                        {
                            Console.WriteLine("Registration updated successfully.");
                            Console.WriteLine($"ID: {existing.RegistrationId}, Student: {existing.StudentId}, Company: {existing.CompanyId}, Registration Date: {existing.RegistrationDate}");
                            Console.WriteLine("Sending agreement...");
                            
                            var result = await _registrationService.SendAgreementAsync(existing.RegistrationId);
                            if (result.Success)
                            {
                                Console.WriteLine("Agreement sent successfully.");
                            }
                            else
                            {
                                Console.WriteLine($"Error sending agreement: {result.ErrorMessage}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error updating registration.");
                        }
                        Console.WriteLine(updated ? "Registration updated." : "Error updating registration.");
                        break;

                    case "5":
                        Console.Write("Enter registration ID to delete: ");
                        var delInput = Console.ReadLine();
                        if (!int.TryParse(delInput, out int registrationId))
                        {
                            Console.WriteLine("Invalid registration ID.");
                            break;
                        }
                        var deleted = await _repository.DeleteAsync(registrationId);
                        Console.WriteLine(deleted ? "Registration deleted." : "Error deleting registration.");
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
