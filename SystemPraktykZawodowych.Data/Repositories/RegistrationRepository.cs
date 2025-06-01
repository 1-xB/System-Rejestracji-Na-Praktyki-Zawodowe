using Dapper;
using System.Data;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Data.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private DatabaseConnection DbConnection { get; }

        public RegistrationRepository(DatabaseConnection databaseConnection)
        {
            DbConnection = databaseConnection;
        }

        public async Task<List<Registration>?> GetAllAsync()
        {
            try
            {
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"SELECT 
                                    registration_id AS Registration_id,
                                    student_id AS Student_id,
                                    company_id AS Company_id,
                                    registration_date AS Registration_date,
                                    agreement_generated AS Agreement_generated,
                                    agreement_generated_date AS Agreement_generated_date
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
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"SELECT * FROM Registrations WHERE registration_id = @RegistrationId";
                    var registration = await conn.QueryFirstOrDefaultAsync<Registration>(sql, new { RegistrationId = registrationId });
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
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"INSERT INTO Registrations (student_id, company_id, registration_date, agreement_generated, agreement_generated_date)
                                   VALUES (@Student_id, @Company_id, @Registration_date, @Agreement_generated, @Agreement_generated_date)";
                    var result = await conn.ExecuteAsync(sql, registration);
                    return result > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Registration registration)
        {
            try
            {
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"UPDATE Registrations
                                   SET student_id = @Student_id,
                                       company_id = @Company_id,
                                       registration_date = @Registration_date,
                                       agreement_generated = @Agreement_generated,
                                       agreement_generated_date = @Agreement_generated_date
                                   WHERE registration_id = @Registration_id";
                    var result = await conn.ExecuteAsync(sql, registration);
                    return result > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int registrationId)
        {
            try
            {
                using (IDbConnection conn = DbConnection.CreateConnection())
                {
                    string sql = @"DELETE FROM Registrations WHERE registration_id = @RegistrationId";
                    var result = await conn.ExecuteAsync(sql, new { RegistrationId = registrationId });
                    return result > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


