using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Data.Models;

public class MedicalRecord
{
    public int Id { get; set; }
    public string Diagnosis { get; set; }
    public string Treatment { get; set; }
    public DateTime RecordDate { get; set; }

    [ForeignKey("Patient")]
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
}