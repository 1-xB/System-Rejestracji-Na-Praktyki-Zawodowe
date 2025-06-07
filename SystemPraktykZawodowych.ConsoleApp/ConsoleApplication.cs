using System;
using System.Threading.Tasks;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;

namespace RegistrationConsoleApp
{
    public class ConsoleApplication
    {
        private readonly IRegistrationService _registrationService;
        private readonly IStudentService _studentService;
        private readonly ICompanyService _companyService;
        

        public ConsoleApplication(IRegistrationRepository repository, 
                                  IRegistrationService registrationService,
                                  IStudentService studentService,
                                  ICompanyService companyService)
        {
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
                Console.WriteLine("6. Send agreement");
                // TODO: Obsługa firm i studentow
                Console.WriteLine("0. Exit");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim();


                switch (choice)
                {
                    case "1":
                        var all = await _registrationService.GetAllRegistrationsAsync();
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
                        var reg = await _registrationService.GetRegistrationByIdAsync(id);
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
                        
                        var student = await _studentService.GetStudentByIdAsync(studentId);
                        if (student == null)
                        {
                            Console.WriteLine("Student not found.");
                            break;
                        }
                        
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
                        
                        
                        if (await HasAvailableInternshipSlots(company))
                        {
                            Console.WriteLine("The company has reached the maximum number of internships.");
                            break;
                        }
                        
                        newReg.CompanyId = companyId;
                        newReg.StudentId = studentId;
                        newReg.RegistrationDate = DateTime.UtcNow;
                        newReg.AgreementGenerated = 0;
                        newReg.AgreementGeneratedDate = null; // Umowa nie jest jeszcze wygenerowana

                        var added = await _registrationService.AddRegistrationAsync(newReg);
                        if (added)
                        {
                            Console.WriteLine("Registration added successfully.");
                            Console.WriteLine($"Student: {newReg.StudentId}, Company: {newReg.CompanyId}, Registration Date: {newReg.RegistrationDate}");
                            Console.WriteLine($"Sending agreement to {student.Email} ...");
                            
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
                        var existing = await _registrationService.GetRegistrationByIdAsync(updId);
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
                        var studentById = await _studentService.GetStudentByIdAsync(updStudentId);
                        if (studentById == null)
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
                        var comp = await _companyService.GetCompanyByIdAsync(updCompanyId);
                        if (comp == null)
                        {
                            Console.WriteLine("Company not found.");
                            break;
                        }

                        if (await HasAvailableInternshipSlots(comp))
                        {
                            Console.WriteLine("The company has reached the maximum number of internships.");
                            break;
                        }


                        existing.StudentId = updStudentId;
                        existing.CompanyId = updCompanyId;
                        existing.RegistrationDate = DateTime.UtcNow;
                        existing.AgreementGenerated = 0; 
                        existing.AgreementGeneratedDate = null; 

                        var updated = await _registrationService.UpdateRegistrationAsync(existing);
                        if (updated)
                        {
                            Console.WriteLine("Registration updated successfully.");
                            Console.WriteLine($"ID: {existing.RegistrationId}, Student: {existing.StudentId}, Company: {existing.CompanyId}, Registration Date: {existing.RegistrationDate}");
                            Console.WriteLine($"Sending agreement to {studentById.Email}...");
                            
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
                        var deleted = await _registrationService.DeleteRegistrationAsync(registrationId);
                        Console.WriteLine(deleted ? "Registration deleted." : "Error deleting registration.");
                        break;
                    
                    case "6":
                        await SendAgreementAsync(); // TODO : Możemy w kazdym case, obsłużyć to przez metodę, bo jest czytelniej.
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
        
        private async Task SendAgreementAsync()
        {
            Console.WriteLine("Enter your student ID:");
            string studentIdInput = Console.ReadLine();
            
            if (!int.TryParse(studentIdInput, out int studentId))
            {
                Console.WriteLine("Invalid student ID.");
                return;
            }

            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            
            var registrations = await _registrationService.GetRegistrationsByStudentIdAsync(studentId);
            if (registrations == null || registrations.Count == 0)
            {
                Console.WriteLine("No registrations found for this student.");
                return;
            }
            Console.WriteLine("Select a registration to send the agreement:");
            for (int i = 0; i < registrations.Count; i++)
            {
                var reg = registrations[i];
                Console.WriteLine($"ID: {reg.RegistrationId}, Company ID: {reg.CompanyId}, Registration Date: {reg.RegistrationDate}");
            }
            
            Console.Write("Enter the registration id: ");
            string regIdInput = Console.ReadLine();
            if (!int.TryParse(regIdInput, out int registrationId))
            {
                Console.WriteLine("Invalid registration ID.");
                return;
            }
            var registration = await _registrationService.GetRegistrationByIdAsync(registrationId);
            if (registration == null)
            {
                Console.WriteLine("Registration not found.");
                return;
            }
            if (registration.AgreementGenerated == 1)
            {
                Console.WriteLine("Agreement has already been generated for this registration.");
                return;
            }
            
            Console.WriteLine("Sending agreement...");
            
            var result = await _registrationService.SendAgreementAsync(registration.RegistrationId);
            if (result.Success)
            {
                Console.WriteLine("Agreement sent successfully.");
            }
            else
            {
                Console.WriteLine($"Error sending agreement: {result.ErrorMessage}");
            }
        }
        private async Task<bool> HasAvailableInternshipSlots(Company comp)
        {
            int internshipsCount = await _registrationService.CountRegistrationsByCompanyIdAsync(comp.Id);
            if (internshipsCount >= comp.MaxInternships)
            {
                return true;
            }

            return false;
        }
    }
}
