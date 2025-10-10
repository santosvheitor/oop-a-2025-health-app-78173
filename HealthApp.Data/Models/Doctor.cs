using System.ComponentModel.DataAnnotations;


namespace HealthApp.Data.Models;

public class Doctor
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string Specialty { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}