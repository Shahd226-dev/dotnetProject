using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = RoleConstants.Student;

    [MinLength(2)]
    [MaxLength(150)]
    public string? FullName { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }
}