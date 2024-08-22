using System.ComponentModel.DataAnnotations;

namespace ClubTools.Shared.Models;

public class RegistrationModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(6)]
    public string? Password { get; set; }

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }
}
