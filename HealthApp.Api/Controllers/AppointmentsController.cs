using HealthApp.Data.Data;
using HealthApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // 🔐 exige autenticação por padrão
public class AppointmentsController : ControllerBase
{
    private readonly HospitalContext _context;

    public AppointmentsController(HospitalContext context)
    {
        _context = context;
    }

    // 🔹 GET: api/appointments
    // Apenas Admins e Doctors podem ver todas
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        return await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .ToListAsync();
    }

    // 🔹 GET: api/appointments/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<Appointment>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound();

        return appointment;
    }

    // 🔹 GET: api/appointments/mine
    // Mostra apenas as consultas do paciente logado
    [HttpGet("mine")]
    [Authorize(Roles = "Patient")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetMyAppointments()
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized("Usuário não autenticado.");

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Email == userEmail);
        if (patient == null)
            return BadRequest("Paciente não encontrado para o usuário atual.");

        var appointments = await _context.Appointments
            .Where(a => a.PatientId == patient.Id)
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .ToListAsync();

        return appointments;
    }

    // 🔹 POST: api/appointments
    // Paciente marca uma nova consulta
    [HttpPost]
    [Authorize(Roles = "Patient")]
    public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized("Usuário não autenticado.");

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Email == userEmail);
        if (patient == null)
            return BadRequest("Paciente não encontrado para o usuário atual.");

        if (appointment.DoctorId <= 0)
            return BadRequest("DoctorId é obrigatório.");

        appointment.PatientId = patient.Id;
        appointment.Status ??= "Pending";

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    // 🔹 PUT: api/appointments/{id}
    // Edição geral — Admin e Doctor podem editar
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
    {
        if (id != appointment.Id)
            return BadRequest();

        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 🔹 PATCH: api/appointments/{id}/confirm
    // Apenas Doctors e Admins podem confirmar
    [HttpPatch("{id}/confirm")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> ConfirmAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        appointment.Status = "Confirmed";
        await _context.SaveChangesAsync();

        return Ok(appointment);
    }

    // 🔹 DELETE: api/appointments/{id}
    // Paciente pode excluir as próprias consultas
    [HttpDelete("{id}")]
    [Authorize(Roles = "Patient,Admin")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
