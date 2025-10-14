using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Data.Models;

public class Patient
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string IdentityUserId { get; set; } = null!;

    // 👇 Relacionamento com os registros médicos
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}