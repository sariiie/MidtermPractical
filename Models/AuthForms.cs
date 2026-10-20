using System.ComponentModel.DataAnnotations;

namespace Midterm.Models;

public class LoginForm
{
    [Required(ErrorMessage = "Enter your email.")]
    [EmailAddress(ErrorMessage = "That doesn't look like an email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Enter your password.")]
    public string Password { get; set; } = "";
}

public class RegisterForm
{
    [Required(ErrorMessage = "Pick a display name.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Display name must be 2 to 20 characters.")]
    public string DisplayName { get; set; } = "";

    [Required(ErrorMessage = "Enter your email.")]
    [EmailAddress(ErrorMessage = "That doesn't look like an email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Choose a password.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Type your password again.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
    public string ConfirmPassword { get; set; } = "";

    public string Avatar { get; set; } = "🦊";
}

public record AuthResult(bool Success, string? Error = null)
{
    public static AuthResult Ok() => new(true);
    public static AuthResult Fail(string error) => new(false, error);
}
