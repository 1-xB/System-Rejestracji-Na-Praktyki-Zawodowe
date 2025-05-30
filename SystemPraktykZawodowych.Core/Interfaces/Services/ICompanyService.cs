using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Core.Interfaces.Services;

public interface ICompanyService
{
    public Task<List<Company>?> GetAllCompanyAsync();
    public Task<bool> AddCompanyAsync(Company company);
    public Task<bool> UpdateCompanyAsync(Company company);
    public Task<bool> DeleteCompanyAsync(int companyId);
    public Task<Company?> GetCompanyByIdAsync(int companyId);
}