using Dapper;
using System.Data;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Data.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private DatabaseConnection dbConnection { get; }

        public RegistrationRepository(DatabaseConnection databaseConnection)
        {
            dbConnection = databaseConnection;
        }

        public async Task<List<Registration>?> GetAllAsync()
        {
            try
            {
                using (IDbConnection conn = dbConnection.CreateConnection())
                {
                    string sql = @"SELECT 
                                    registration_id AS RegistrationId,
                                    student_id AS StudentId,
                                    company_id AS CompanyId,
                                    registration_date AS RegistrationDate,
                                    agreement_generated AS AgreementGenerated,
                                    agreement_generated_date AS AgreementGeneratedDate
                                   FROM Registrations";
                    var registrations = await conn.QueryAsync<Registration>(sql);
                    return registrations.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Registration?> GetRegistrationByIdAsync(int registrationId)
        {
            try
            {
                using (IDbConnection conn = dbConnection.CreateConnection())
                {
                    string sql = @"SELECT 
                                    registration_id AS RegistrationId,
                                    student_id AS StudentId,
                                    company_id AS CompanyId,
                                    registration_date AS RegistrationDate,
                                    agreement_generated AS AgreementGenerated,
                                    agreement_generated_date AS AgreementGeneratedDate
                                    FROM Registrations WHERE registration_id = @registrationId";
                    var registration = await conn.QueryFirstOrDefaultAsync<Registration>(sql, new { registrationId });
                    return registration;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddAsync(Registration registration)
        {
            try
            {
                using (IDbConnection conn = dbConnection.CreateConnection())
                {
                    string sql = @"INSERT INTO Registrations (student_id, company_id, registration_date, agreement_generated, agreement_generated_date)
                                   VALUES (@StudentId, @CompanyId, @RegistrationDate, @AgreementGenerated, @AgreementGeneratedDate)";
                    var result = await conn.ExecuteAsync(sql, registration);
                    return result > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Registration registration)
        {
            try
            {
                using (IDbConnection conn = dbConnection.CreateConnection())
                {
                    string sql = @"UPDATE Registrations
                                   SET student_id = @StudentId,
                                       company_id = @CompanyId,
                                       registration_date = @RegistrationDate,
                                       agreement_generated = @AgreementGenerated,
                                       agreement_generated_date = @AgreementGeneratedDate
                                   WHERE registration_id = @RegistrationId";
                    var result = await conn.ExecuteAsync(sql, registration);
                    return result > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int registrationId)
        {
            try
            {
                using (IDbConnection conn = dbConnection.CreateConnection())
                {
                    string sql = @"DELETE FROM Registrations WHERE registration_id = @registrationId";
                    var result = await conn.ExecuteAsync(sql, new { registrationId });
                    return result > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
