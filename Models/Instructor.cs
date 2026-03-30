public class Instructor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public InstructorProfile Profile { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}