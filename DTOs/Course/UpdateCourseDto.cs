using System.ComponentModel.DataAnnotations;

public class UpdateCourseDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int InstructorId { get; set; }
}