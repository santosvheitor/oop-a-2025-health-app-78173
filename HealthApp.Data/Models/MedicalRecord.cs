using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Data.Models;

public class MedicalRecord
{
    public int Id { get; set; }

    public string Diagnosis { get; set; } = string.Empty;

    public string Treatment { get; set; } = string.Empty;

    public DateTime RecordDate { get; set; } = DateTime.Now;

    [ForeignKey("Patient")]
    public int PatientId { get; set; }

    public Patient Patient { get; set; } = new Patient();
}