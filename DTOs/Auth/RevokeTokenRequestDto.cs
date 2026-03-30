using System.ComponentModel.DataAnnotations;

public class RevokeTokenRequestDto
{
    [Required]
    [MinLength(20)]
    public string RefreshToken { get; set; } = string.Empty;
}