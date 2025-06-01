using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Service.Services;

public class RegistrationService : IRegistrationService
{
    public IRegistrationRepository RegistrationRepository { get; set; }

    public RegistrationService(IRegistrationRepository registrationRepository)
    {
        RegistrationRepository = registrationRepository;
    }

    public async Task<List<Registration>?> GetAllRegistrationsAsync() => await RegistrationRepository.GetAllAsync();

    public async Task<bool> AddRegistrationAsync(Registration registration) => await RegistrationRepository.AddAsync(registration);

    public async Task<bool> UpdateRegistrationAsync(Registration registration) => await RegistrationRepository.UpdateAsync(registration);

    public async Task<bool> DeleteRegistrationAsync(int registrationId) => await RegistrationRepository.DeleteAsync(registrationId);

    public async Task<Registration?> GetRegistrationByIdAsync(int registrationId) => await RegistrationRepository.GetRegistrationByIdAsync(registrationId);
}

