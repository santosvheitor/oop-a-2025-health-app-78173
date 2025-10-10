using Microsoft.AspNetCore.Identity;

namespace HealthApp.Data.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Patient"; // default role
}