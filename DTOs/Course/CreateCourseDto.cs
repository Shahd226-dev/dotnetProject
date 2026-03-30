using System.ComponentModel.DataAnnotations;

public class CreateCourseDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int InstructorId { get; set; }
}