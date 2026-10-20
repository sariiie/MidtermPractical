namespace Midterm.Models;

public class UserAccount
{
    public string Id { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Avatar { get; set; } = "🦊";

    // Never store the plain password: PBKDF2 hash + per-user salt (both base64)
    public string PasswordHash { get; set; } = "";
    public string Salt { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsGuest { get; set; }
}
