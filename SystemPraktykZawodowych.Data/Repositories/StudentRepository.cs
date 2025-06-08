using System.Data;
using Dapper;
using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Data.Repositories;

public class StudentRepository : IStudentRepository
{
    private DatabaseConnection DbConnection { get; set; }
    public StudentRepository(DatabaseConnection databaseConnection)
    {
        DbConnection = databaseConnection;
    }
    public async Task<List<Student>?> GetAllAsync()
    {
        try
        {
            using (IDbConnection conn = DbConnection.CreateConnection())
            {
                string sql = @"SELECT 
                student_id AS Id, 
                first_name AS FirstName, 
                last_name AS LastName, 
                class AS Class, 
                phone_number AS PhoneNumber, 
                email AS Email 
                FROM Students";
                var students = await conn.QueryAsync<Student>(sql);
                return students.ToList();
            }
        }
        catch (Exception ex)
        {
            // Todo : Dodać obługe wyjątków
            throw;
        }
        
    }

    public async Task<bool> AddAsync(Student student)
    {
        try
        {
            using (IDbConnection conn = DbConnection.CreateConnection())
            {
                // https://www.w3schools.com/sql/sql_insert.asp
                string sql = @"INSERT INTO Students (first_name, last_name, class, phone_number, email) 
                               VALUES (@FirstName, @LastName, @Class, @PhoneNumber, @Email)";
                var result = await conn.ExecuteAsync(sql, student);
                return result > 0; // Zwraca true jeśli wstawiono co najmniej jeden rekord
            }
        }
        catch (Exception ex)
        {
            // Todo : Dodać obługe wyjątków
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Student student)
    {
        try
        {
            using (IDbConnection conn = DbConnection.CreateConnection())
            {
                // https://www.w3schools.com/sql/sql_update.asp
                string sql = @"UPDATE Students 
                               SET first_name = @FirstName, 
                                   last_name = @LastName, 
                                   class = @Class, 
                                   phone_number = @PhoneNumber, 
                                   email = @Email 
                               WHERE student_id = @Id";
                var result = await conn.ExecuteAsync(sql, student); // Wramach bezpieczeństwa używamy parametrów, aby uniknąć SQL Injection
                return result > 0; // Zwraca true jeśli usunięto co najmniej jeden rekord
            }
        }
        catch (Exception ex)
        {
            // Todo : Dodać obługe wyjątków
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int studentId)
    {
        try
        {
            using (IDbConnection conn = DbConnection.CreateConnection())
            {
                string sql = @"DELETE FROM Students WHERE student_id = @StudentId";
                var result = await conn.ExecuteAsync(sql, new { StudentId = studentId }); // Wramach bezpieczeństwa używamy parametrów, aby uniknąć SQL Injection
                return result > 0; // Zwraca true jeśli usunięto co najmniej jeden rekord
            }
        }
        catch (Exception ex)
        {
            // Todo : Dodać obługe wyjątków
            throw;
        }
    }

    public async Task<Student?> GetByIdAsync(int studentId)
    {
        try
        {
            using (IDbConnection conn = DbConnection.CreateConnection())
            {
                string sql = @"SELECT student_id as Id, 
                                      first_name as FirstName, 
                                      last_name as LastName, 
                                      class as Class, 
                                      phone_number as PhoneNumber, 
                                      email as Email 
                               FROM Students 
                               WHERE student_id = @StudentId";
                var student = await conn.QueryFirstOrDefaultAsync<Student>(sql, new { StudentId = studentId });
                return student;

            }
        }
        catch (Exception ex)
        {
            // Todo : Dodać obługe wyjątków
            throw;
        }
    }
}