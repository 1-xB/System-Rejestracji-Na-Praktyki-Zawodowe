using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Core.Interfaces.Services;

public interface IStudentService
{
    public Task<List<Student>?> GetAllStudentsAsync();
    public Task<bool> AddStudentAsync(Student student);
    public Task<bool> UpdateStudentAsync(Student student);
    public Task<bool> DeleteStudentAsync(int studentId);
    public Task<Student?> GetStudentByIdAsync(int studentId);
    
}