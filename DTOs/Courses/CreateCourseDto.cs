using System.ComponentModel.DataAnnotations;

public class CreateCourseDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
}
