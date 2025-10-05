using Microsoft.EntityFrameworkCore;
using HealthApp.Domain;
using HealthApp.Domain.Models;

namespace HealthApp.Data.Data;

public class HospitalContext : DbContext
{
    public HospitalContext(DbContextOptions<HospitalContext> options)
        : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
}