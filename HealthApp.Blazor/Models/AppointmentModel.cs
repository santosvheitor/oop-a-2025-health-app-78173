using HealthApp.Domain.Models;

namespace HealthApp.Blazor.Models;

public class AppointmentModel
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;

    public Patient? Patient { get; set; }
    public DoctorModel? Doctor { get; set; }
}