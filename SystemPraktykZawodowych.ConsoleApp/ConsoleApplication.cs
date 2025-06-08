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
                DisplayMainMenu();

                var choice = Console.ReadLine()?.Trim();


                switch (choice)
                {
                    case "1":
                        await DisplayAllRegistrations();
                        break;

                    case "2":
                        int registrationIdInput = ReadIntegerInput("Enter registration ID:");
                        await DisplayRegistrationById(registrationIdInput);
                        break;

                    case "3":
                        var newReg = new Registration();

                        var student = await GetValidatedStudentAsync();
                        if (student == null)
                        {
                            Console.WriteLine("Cannot create registration without a valid student.");
                            break;  
                        }
                        
                        var company = await GetValidatedCompanyAsync();
                        if (company == null)
                        {
                            Console.WriteLine("Cannot create registration without a valid company.");
                            break; 
                        }
                        
                        if (await HasReachedMaxInternships(company))
                        {
                            Console.WriteLine("The company has reached the maximum number of internships.");
                            break;
                        }
                        
                        newReg.CompanyId = company.Id;
                        newReg.StudentId = student.Id;
                        newReg.RegistrationDate = DateTime.UtcNow;
                        newReg.AgreementGenerated = 0;
                        newReg.AgreementGeneratedDate = null; // Umowa nie jest jeszcze wygenerowana

                        await AddRegistrationAsync(newReg, student);
                        break;

                    case "4":
                        var registrationToUpdate = await GetValidatedRegistrationAsync();
                        // existing - registration to update
                        var studentById = await GetValidatedStudentAsync();
                        var companyById = await GetValidatedCompanyAsync();

                        if (await HasReachedMaxInternships(companyById))
                        {
                            Console.WriteLine("The company has reached the maximum number of internships.");
                            break;
                        }


                        registrationToUpdate.StudentId = studentById.Id;
                        registrationToUpdate.CompanyId = companyById.Id;
                        registrationToUpdate.RegistrationDate = DateTime.UtcNow;
                        registrationToUpdate.AgreementGenerated = 0; 
                        registrationToUpdate.AgreementGeneratedDate = null; 

                        await UpdateRegistrationAsync(registrationToUpdate, studentById);
                        break;

                    case "5":
                        Console.Write("Enter registration ID to delete: ");
                        int registrationId = ReadIntegerInput("Enter registration ID to delete:");
                        var deleted = await _registrationService.DeleteRegistrationAsync(registrationId);
                        Console.WriteLine(deleted ? "Registration deleted." : "Error deleting registration.");
                        break;
                    
                    case "6":
                        await SendAgreementAsync(); 
                        break;
                    case "7":
                        await ManageStudentsAsync();
                        break;
                    case "8":
                        await ManageCompaniesAsync();
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

        private async Task UpdateRegistrationAsync(Registration registration, Student student)
        {
            var updated = await _registrationService.UpdateRegistrationAsync(registration);
            if (updated)
            {
                Console.WriteLine("Registration updated successfully.");
                Console.WriteLine($"ID: {registration.RegistrationId}, Student: {registration.StudentId}, Company: {registration.CompanyId}, Registration Date: {registration.RegistrationDate}");
                Console.WriteLine($"Sending agreement to {student.Email}...");
                            
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
            else
            {
                Console.WriteLine("Error updating registration.");
            }
            Console.WriteLine(updated ? "Registration updated." : "Error updating registration.");
        }

        private async Task AddRegistrationAsync(Registration registration, Student student)
        {
            var added = await _registrationService.AddRegistrationAsync(registration);
            if (!added)
            {
                Console.WriteLine(
                    "Error adding registration. Make sure that the same registration does not already exist in the database");
            }
            else
            {
                Console.WriteLine("Registration added successfully.");
                Console.WriteLine(
                    $"Student: {registration.StudentId}, Company: {registration.CompanyId}, Registration Date: {registration.RegistrationDate}");
                Console.WriteLine($"Sending agreement to {student.Email} ...");

                var registrations = await _registrationService.GetAllRegistrationsAsync();
                var newRegistration = registrations?
                    .Where(r => r.StudentId == registration.StudentId && r.CompanyId == registration.CompanyId)
                    .FirstOrDefault();

                if (newRegistration == null)
                {
                    Console.WriteLine("Error finding the newly added registration.");
                }
                else
                {
                    var result = await _registrationService.SendAgreementAsync(newRegistration.RegistrationId);
                    Console.WriteLine(result.Success
                        ? "Agreement sent successfully."
                        : $"Error sending agreement: {result.ErrorMessage}");
                }
            }
        }

        private async Task DisplayRegistrationById(int registrationId)
        {
            var registration = await _registrationService.GetRegistrationByIdAsync(registrationId);
            if (registration != null)
            {
                Console.WriteLine($"ID: {registration.RegistrationId}, Student: {registration.StudentId}, Company: {registration.CompanyId}, Registration Date: {registration.RegistrationDate}");
            }
            else
            {
                Console.WriteLine("Registration not found.");
            }
        }

        private async Task DisplayAllRegistrations()
        {
            var registrations = await _registrationService.GetAllRegistrationsAsync();
            if (registrations == null || registrations.Count == 0)
            {
                Console.WriteLine("No registrations found.");
            }
            else
            {
                foreach (var registration in registrations)
                {
                    Console.WriteLine($"ID: {registration.RegistrationId}, Student: {registration.StudentId}, Company: {registration.CompanyId}, Registration Date: {registration.RegistrationDate}");
                }
            }
        }

        private static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Select an action:");
            Console.WriteLine("1. Show all registrations");
            Console.WriteLine("2. Find registration by ID");
            Console.WriteLine("3. Add registration");
            Console.WriteLine("4. Update registration");
            Console.WriteLine("5. Delete registration");
            Console.WriteLine("6. Send agreement");
            Console.WriteLine("7. Manage Students");
            Console.WriteLine("8. Manage Companies");
            Console.WriteLine("0. Exit");
            Console.Write("Choice: ");
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
        private async Task<bool> HasReachedMaxInternships(Company company)
        {
            int internshipsCount = await _registrationService.CountRegistrationsByCompanyIdAsync(company.Id);
            return internshipsCount >= company.MaxInternships;
        }
        
        private int ReadIntegerInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result))
                    return result;
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
        private async Task<Student> GetValidatedStudentAsync()
        {
            int studentId = ReadIntegerInput("Student ID: ");
            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student == null)
                Console.WriteLine("Student not found.");
            return student;
        }
        
        private async Task<Company> GetValidatedCompanyAsync()
        {
            int companyId = ReadIntegerInput("Company ID: ");
            var company = await _companyService.GetCompanyByIdAsync(companyId);
            if (company == null)
                Console.WriteLine("Company not found.");
            return company;
        }
        
        private async Task<Registration> GetValidatedRegistrationAsync() 
        {
            int registrationId = ReadIntegerInput("Registration ID: ");
            var registration = await _registrationService.GetRegistrationByIdAsync(registrationId);
            if (registration == null)
                Console.WriteLine("Registration not found.");
            return registration;
        }
        private async Task ManageStudentsAsync()
        {
            Console.Clear();
            Console.WriteLine("STUDENT MANAGEMENT");
            Console.WriteLine("1. Show all students");
            Console.WriteLine("2. Find student by ID");
            Console.WriteLine("3. Add student");
            Console.WriteLine("4. Update student");
            Console.WriteLine("5. Delete student");
            Console.Write("Choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var all = await _studentService.GetAllStudentsAsync();
                    if (all is null || all.Count == 0) Console.WriteLine("No students.");
                    else all.ForEach(s => Console.WriteLine($"{s.Id}: {s.FirstName} {s.LastName}, {s.Class}, {s.Email}, {s.PhoneNumber}"));
                    break;

                case "2":
                    Console.Write("Student ID: ");
                    if (int.TryParse(Console.ReadLine(), out int sid))
                    {
                        var s = await _studentService.GetStudentByIdAsync(sid);
                        if (s != null)
                            Console.WriteLine($"{s.Id}: {s.FirstName} {s.LastName}, {s.Class}, {s.Email}, {s.PhoneNumber}");
                        else Console.WriteLine("Student not found.");
                    }
                    else Console.WriteLine("Invalid input.");
                    break;

                case "3":
                    var student = new Student();
                    Console.Write("First name: "); student.FirstName = Console.ReadLine();
                    Console.Write("Last name: "); student.LastName = Console.ReadLine();
                    Console.Write("Class: "); student.Class = Console.ReadLine();
                    Console.Write("Phone: "); student.PhoneNumber = Console.ReadLine();
                    Console.Write("Email: "); student.Email = Console.ReadLine();
                    if (await _studentService.AddStudentAsync(student))
                        Console.WriteLine("Student added.");
                    else Console.WriteLine("Error adding student.");
                    break;

                case "4":
                    Console.Write("Student ID to update: ");
                    if (int.TryParse(Console.ReadLine(), out int uid))
                    {
                        var existing = await _studentService.GetStudentByIdAsync(uid);
                        if (existing == null) { Console.WriteLine("Not found."); break; }
                        Console.Write("First name: "); existing.FirstName = Console.ReadLine();
                        Console.Write("Last name: "); existing.LastName = Console.ReadLine();
                        Console.Write("Class: "); existing.Class = Console.ReadLine();
                        Console.Write("Phone: "); existing.PhoneNumber = Console.ReadLine();
                        Console.Write("Email: "); existing.Email = Console.ReadLine();
                        Console.WriteLine(await _studentService.UpdateStudentAsync(existing) ? "Updated." : "Update failed.");
                    }
                    else Console.WriteLine("Invalid ID.");
                    break;

                case "5":
                    Console.Write("Student ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int did))
                    {
                        Console.WriteLine(await _studentService.DeleteStudentAsync(did) ? "Deleted." : "Delete failed.");
                    }
                    else Console.WriteLine("Invalid input.");
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        private async Task ManageCompaniesAsync()
        {
            Console.Clear();
            Console.WriteLine("COMPANY MANAGEMENT");
            Console.WriteLine("1. Show all companies");
            Console.WriteLine("2. Find company by ID");
            Console.WriteLine("3. Add company");
            Console.WriteLine("4. Update company");
            Console.WriteLine("5. Delete company");
            Console.Write("Choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var all = await _companyService.GetAllCompanyAsync();
                    if (all is null || all.Count == 0) Console.WriteLine("No companies.");
                    else all.ForEach(c => Console.WriteLine($"{c.Id}: {c.Name}, {c.SupervisorName}, {c.Address}, Max: {c.MaxInternships}"));
                    break;

                case "2":
                    Console.Write("Company ID: ");
                    if (int.TryParse(Console.ReadLine(), out int cid))
                    {
                        var c = await _companyService.GetCompanyByIdAsync(cid);
                        if (c != null)
                            Console.WriteLine($"{c.Id}: {c.Name}, {c.SupervisorName}, {c.Address}, Max: {c.MaxInternships}");
                        else Console.WriteLine("Company not found.");
                    }
                    else Console.WriteLine("Invalid input.");
                    break;

                case "3":
                    var company = new Company();
                    Console.Write("Name: "); company.Name = Console.ReadLine();
                    Console.Write("Supervisor: "); company.SupervisorName = Console.ReadLine();
                    Console.Write("Address: "); company.Address = Console.ReadLine();
                    Console.Write("Max internships: ");
                    if (int.TryParse(Console.ReadLine(), out int max)) company.MaxInternships = max;
                    else { Console.WriteLine("Invalid number."); break; }
                    Console.WriteLine(await _companyService.AddCompanyAsync(company) ? "Company added." : "Add failed.");
                    break;

                case "4":
                    Console.Write("Company ID to update: ");
                    if (int.TryParse(Console.ReadLine(), out int uid))
                    {
                        var existing = await _companyService.GetCompanyByIdAsync(uid);
                        if (existing == null) { Console.WriteLine("Not found."); break; }
                        Console.Write("Name: "); existing.Name = Console.ReadLine();
                        Console.Write("Supervisor: "); existing.SupervisorName = Console.ReadLine();
                        Console.Write("Address: "); existing.Address = Console.ReadLine();
                        Console.Write("Max internships: ");
                        if (int.TryParse(Console.ReadLine(), out int maxUp)) existing.MaxInternships = maxUp;
                        else { Console.WriteLine("Invalid number."); break; }
                        Console.WriteLine(await _companyService.UpdateCompanyAsync(existing) ? "Updated." : "Update failed.");
                    }
                    else Console.WriteLine("Invalid ID.");
                    break;

                case "5":
                    Console.Write("Company ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int did))
                    {
                        Console.WriteLine(await _companyService.DeleteCompanyAsync(did) ? "Deleted." : "Delete failed.");
                    }
                    else Console.WriteLine("Invalid input.");
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
