using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Core.Interfaces.Repositories;

public interface IStudentRepository
{
    public Task<List<Student>?> GetAllAsync();
    public Task<bool> AddAsync(Student student);
    public Task<bool> UpdateAsync(Student student);
    public Task<bool> DeleteAsync(int studentId);
    public Task<Student?> GetByIdAsync(int studentId);
}