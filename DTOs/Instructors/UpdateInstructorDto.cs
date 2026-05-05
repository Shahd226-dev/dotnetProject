using System.ComponentModel.DataAnnotations;

public class UpdateInstructorDto
{
    [Required]
    [MinLength(2)]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Bio { get; set; }
}
