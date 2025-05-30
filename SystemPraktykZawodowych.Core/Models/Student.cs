using System.ComponentModel.DataAnnotations.Schema;
namespace SystemPraktykZawodowych.Core.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Class { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}