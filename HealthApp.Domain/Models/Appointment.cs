namespace HealthApp.Domain.Models;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Cancelled
}
public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }

    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}