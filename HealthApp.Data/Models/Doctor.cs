using System.ComponentModel.DataAnnotations;


namespace HealthApp.Data.Models;

public class Doctor
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public string Specialty { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    public string IdentityUserId { get; set; } = null!;
}