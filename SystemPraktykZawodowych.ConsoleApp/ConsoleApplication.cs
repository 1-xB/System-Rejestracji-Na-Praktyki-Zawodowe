using System;
using System.Threading.Tasks;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Models;

namespace RegistrationConsoleApp
{
    public class ConsoleApplication
    {
        private readonly IRegistrationRepository _repository;

        public ConsoleApplication(IRegistrationRepository repository)
        {
            _repository = repository;
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
                        newReg.StudentId = studentId;

                        Console.Write("Company ID: ");
                        var companyInput = Console.ReadLine();
                        if (!int.TryParse(companyInput, out int companyId))
                        {
                            Console.WriteLine("Invalid company ID.");
                            break;
                        }
                        newReg.CompanyId = companyId;

                        newReg.RegistrationDate = DateTime.UtcNow;
                        newReg.AgreementGenerated = 0;
                        newReg.AgreementGeneratedDate = DateTime.UtcNow; 

                        var added = await _repository.AddAsync(newReg);
                        Console.WriteLine(added ? "Registration added." : "Error adding registration.");
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

                        Console.Write("New company ID: ");
                        var updCompanyInput = Console.ReadLine();
                        if (!int.TryParse(updCompanyInput, out int updCompanyId))
                        {
                            Console.WriteLine("Invalid company ID.");
                            break;
                        }

                        existing.StudentId = updStudentId;
                        existing.CompanyId = updCompanyId;
                        existing.RegistrationDate = DateTime.UtcNow;
                        existing.AgreementGenerated = 1; 
                        existing.AgreementGeneratedDate = DateTime.UtcNow;

                        var updated = await _repository.UpdateAsync(existing);
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
