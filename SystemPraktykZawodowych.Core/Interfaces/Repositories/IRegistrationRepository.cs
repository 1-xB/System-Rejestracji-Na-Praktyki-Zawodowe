using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Core.Interfaces.Repositories
{
    public interface IRegistrationRepository
    {
        Task<List<Registration>?> GetAllAsync();
        Task<bool> AddAsync(Registration registration);
        Task<bool> UpdateAsync(Registration registration);
        Task<bool> DeleteAsync(int registrationId);
        Task<Registration?> GetByIdAsync(int registrationId);
        Task<int> CountByCompanyIdAsync(int companyId);
        Task<List<Registration>?> GetByStudentIdAsync(int studentId);
    }
}
