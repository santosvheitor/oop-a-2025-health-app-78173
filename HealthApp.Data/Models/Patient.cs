using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HealthApp.Data.Models;

public class Patient
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    // Identity User
    [Required]
    [ForeignKey("User")]
    public string IdentityUserId { get; set; } = null!;

    public IdentityUser? User { get; set; }
}