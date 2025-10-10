using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Data.Models;

public class Appointment
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    [ForeignKey("Doctor")]
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    [ForeignKey("Patient")]
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
}