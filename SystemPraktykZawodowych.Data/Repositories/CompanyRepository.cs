using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private DatabaseConnection DbConnection { get; set; }

        public CompanyRepository(DatabaseConnection databaseConnection)
        {
            DbConnection = databaseConnection;
        }
        public async Task<List<Company>> GetAllAsync()
        {
            try
            {
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"SELECT 
                                    company_id AS Id, 
                                    name AS Name, 
                                    supervisor_name AS SupervisorName, 
                                    address AS Address, 
                                    max_internships AS MaxInternships 
                                   FROM Companies";
                    var companies = await conn.QueryAsync<Company>(sql);
                    return companies.ToList();
                }
            }
            catch (Exception ex)
            {
                // Todo : Dodać obługe wyjątków
                throw;
            }
        }

        public async Task<bool> AddAsync(Company company)
        {
            try
            {
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"INSERT INTO Companies (name, supervisor_name, address, max_internships)
                            VALUES (@Name, @SupervisorName, @Address, @MaxInternships)";
                    var result = await conn.ExecuteAsync(sql, company);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int companyId)
        {
            try
            {
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"DELETE FROM Companies WHERE student_id = @CompanyId";
                    var result = await conn.ExecuteAsync(sql, new { CompanyId = companyId });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Company> GetCompanyByIdAsync(int companyId)
        {
            try
            {
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"SELECT * FROM Companies WHERE company_id = @CompanyId";
                    var company = await conn.QueryFirstOrDefaultAsync<Company>(sql, new { CompanyId = companyId });
                    return company;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Company company)
        {
            using (IDbConnection conn = DbConnection.CreateConnection())
            {
                string sql = @"UPDATE Companies
                              SET name = @Name,
                              supervisor_name = @SupervisorName,
                              address = @Address,
                              max_internships = @MaxInternships,
                              ";
                var result = await conn.ExecuteAsync(sql, company);
                return result > 0;
            }
        }
    }
}
