using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Service.Services;

public class CompanyService : ICompanyService
{

    public ICompanyRepository CompanyRepository { get; set; }

    public CompanyService(ICompanyRepository company)
    {
        CompanyRepository = company;
    }

    public async Task<List<Company>?> GetAllCompanyAsync() => await CompanyRepository.GetAllAsync();

    public async Task<bool> AddCompanyAsync(Company company) => await CompanyRepository.AddAsync(company);

    public async Task<bool> UpdateCompanyAsync(Company company) => await CompanyRepository.UpdateAsync(company);

    public async Task<bool> DeleteCompanyAsync(int companyId) => await CompanyRepository.DeleteAsync(companyId);

    public async Task<Company?> GetCompanyByIdAsync(int companyId) => await CompanyRepository.GetCompanyByIdAsync(companyId);
}