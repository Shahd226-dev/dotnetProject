using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequestDto
{
    [Required]
    [MinLength(20)]
    public string RefreshToken { get; set; } = string.Empty;
}