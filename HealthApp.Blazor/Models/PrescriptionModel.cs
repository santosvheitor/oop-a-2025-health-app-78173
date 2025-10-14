using System;
using HealthApp.Domain.Models;

namespace HealthApp.Blazor.Models;

public class PrescriptionModel
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string Medication { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
}