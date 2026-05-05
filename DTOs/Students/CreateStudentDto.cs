using System.ComponentModel.DataAnnotations;

public class CreateStudentDto
{
    [Required]
    [MinLength(2)]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;
}
