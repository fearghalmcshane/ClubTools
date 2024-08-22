using System.ComponentModel.DataAnnotations;

namespace ClubTools.Shared.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; } = String.Empty;

    [Required]
    public string? Password { get; set; } = String.Empty;
}
