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

    // Date and hoour that starts
    public DateTime Date { get; set; }

    // Query duration in minutes (default: 30)
    public int DurationMinutes { get; set; } = 30;

    // Stores status as a string (for compatibility)
    public string Status { get; set; } = AppointmentStatus.Pending.ToString();

    public string? Notes { get; set; }

    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }

    // Calculated property (not mapped)
    public DateTime End => Date.AddMinutes(DurationMinutes);
}