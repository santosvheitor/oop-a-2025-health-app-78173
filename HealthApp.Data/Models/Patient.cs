using System.ComponentModel.DataAnnotations;


namespace HealthApp.Data.Models;

public class Patient
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public DateTime DateOfBirth { get; set; }
}