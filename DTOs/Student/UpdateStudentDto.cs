using System.ComponentModel.DataAnnotations;

public class UpdateStudentDto
{
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}