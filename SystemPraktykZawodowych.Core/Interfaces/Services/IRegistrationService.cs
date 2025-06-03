using SystemPraktykZawodowych.Core.Models;
using SystemPraktykZawodowych.Core.Interfaces;

namespace SystemPraktykZawodowych.Core.Interfaces.Services;

public interface IRegistrationService
{
    public Task<List<Registration>?> GetAllRegistrationsAsync();
    public Task<bool> AddRegistrationAsync(Registration registration);
    public Task<(bool Success, string ErrorMessage)> SendAgreementAsync(int registrationId);
    public Task<bool> UpdateRegistrationAsync(Registration registration);
    public Task<bool> DeleteRegistrationAsync(int registrationId);
    public Task<Registration?> GetRegistrationByIdAsync(int registrationId);
    public Task<int> CountRegistrationsByCompanyIdAsync(int companyId);
    public Task<List<Registration>?> GetRegistrationsByStudentIdAsync(int studentId);
}
