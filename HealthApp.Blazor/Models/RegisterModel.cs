namespace HealthApp.Blazor.Models;

public class RegisterModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "";
    public string? Specialty { get; set; } 
}