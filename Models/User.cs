public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int TokenVersion { get; set; }
    public Student? StudentProfile { get; set; }
    public Instructor? InstructorProfile { get; set; }
}