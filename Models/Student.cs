public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}