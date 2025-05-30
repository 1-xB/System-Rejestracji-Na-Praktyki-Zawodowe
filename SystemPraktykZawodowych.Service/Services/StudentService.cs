using SystemPraktykZawodowych.Core.Interfaces.Repositories;
using SystemPraktykZawodowych.Core.Interfaces.Services;
using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Service.Services;

public class StudentService : IStudentService
{
    public IStudentRepository StudentRepository { get; set; }
    
    public StudentService(IStudentRepository studentRepository)
    {
        StudentRepository = studentRepository;
    }
    
    public async Task<List<Student>?> GetAllStudentsAsync()
    {
        return await StudentRepository.GetAllAsync();
    }

    public Task<bool> AddStudentAsync(Student student) => StudentRepository.AddAsync(student);

    public Task<bool> UpdateStudentAsync(Student student) => StudentRepository.UpdateAsync(student);
    public Task<bool> DeleteStudentAsync(int studentId) => StudentRepository.DeleteAsync(studentId);

    public Task<Student?> GetStudentByIdAsync(int studentId) => StudentRepository.GetByIdAsync(studentId);
}