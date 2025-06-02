using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Service.Services;

public class RegistrationService : IRegistrationService
{
    public IRegistrationRepository RegistrationRepository { get; set; }
    public IAgreementGeneratorService AgreementGeneratorService { get; set; }
    public IEmailSender EmailSender { get; set; }
    public IStudentService StudentService { get; set; }

    public RegistrationService(IRegistrationRepository registrationRepository, IAgreementGeneratorService agreementGeneratorService, IEmailSender emailSender, IStudentService studentService)
    {
        RegistrationRepository = registrationRepository;
        AgreementGeneratorService = agreementGeneratorService;
        EmailSender = emailSender;
        StudentService = studentService;
    }

    public async Task<List<Registration>?> GetAllRegistrationsAsync() => await RegistrationRepository.GetAllAsync();

    public async Task<bool> AddRegistrationAsync(Registration registration) => await RegistrationRepository.AddAsync(registration);
    public async Task<(bool Success, string ErrorMessage)> SendAgreementAsync(int registrationId)
    {
        var registration = await GetRegistrationByIdAsync(registrationId);
        if (registration is null)
        {
            return (false, "Registration with the given Id does not exist");
        }
        
        var student = await StudentService.GetStudentByIdAsync(registration.StudentId);
        if (student is null)
        {
            return (false, "The student with the given ID does not exist");
        }

        try
        {
            byte[] Pdf = await AgreementGeneratorService.GenerateAgreement(registration);
            string emailBody =
                $"Witaj {student.FirstName} ! Zostałeś dodany na praktyki zawodowe. Poniżej znajduję się umowa o zapisie do nich";

            var result = await EmailSender.SendEmailAsync(student.Email, emailBody, "UMOWA PRAKTYK ZAWODOWYCH", Pdf);
            if (result.Success)
            {
                registration.AgreementGenerated = 1;
                registration.AgreementGeneratedDate = DateTime.UtcNow;
                await UpdateRegistrationAsync(registration);
                return (true, "");
            }

            return (false, "The email failed to send. Try again later");
        }
        catch (Exception ex)
        {
            return (false, $"The email failed to send. Error : {ex.Message}");
        }
    }

    public async Task<bool> UpdateRegistrationAsync(Registration registration) => await RegistrationRepository.UpdateAsync(registration);

    public async Task<bool> DeleteRegistrationAsync(int registrationId) => await RegistrationRepository.DeleteAsync(registrationId);

    public async Task<Registration?> GetRegistrationByIdAsync(int registrationId) => await RegistrationRepository.GetRegistrationByIdAsync(registrationId);
}

