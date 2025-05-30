using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Core.Interfaces.Repositories;

public interface ICompanyRepository
{
    public Task<List<Company>> GetAllAsync();
    public Task<bool> AddAsync(Company company);
    public Task<bool> UpdateAsync(Company company);
    public Task<bool> DeleteAsync(int companyId);
    public Task<Company> GetCompanyByIdAsync(int companyId);
    
}