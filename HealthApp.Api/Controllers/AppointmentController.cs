using HealthApp.Data.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController : ControllerBase
{
    private readonly HospitalContext _context;

    public AppointmentController(HospitalContext context)
    {
        _context = context;
    }

    // GET: api/Appointment
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ToListAsync();

        return Ok(appointments);
    }


    // GET: api/Appointment/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }


    // POST: api/Appointment
    [HttpPost]
    public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
    {
        // Validação básica de campos obrigatórios
        if (appointment.PatientId == 0 || appointment.DoctorId == 0 || appointment.Date == default)
            return BadRequest("PatientId, DoctorId, and Date are required.");

        // Verifica se paciente e médico existem
        var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
        var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);

        if (!patientExists) return BadRequest("Patient not found.");
        if (!doctorExists) return BadRequest("Doctor not found.");

        // Verifica se o médico já tem outro compromisso no mesmo horário
        var conflict = await _context.Appointments
            .AnyAsync(a => a.DoctorId == appointment.DoctorId && a.Date == appointment.Date);

        if (conflict)
            return BadRequest("The doctor already has an appointment at that time.");

        // Adiciona o novo agendamento
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Recarrega o appointment com os relacionamentos
        var result = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == appointment.Id);

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, result);
    }

    // PUT: api/Appointment/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
    {
        if (id != appointment.Id)
            return BadRequest();

        // Checa se paciente e médico existem
        var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
        var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);

        if (!patientExists) return BadRequest("Patient not found.");
        if (!doctorExists) return BadRequest("Doctor not found.");

        // Verifica conflito de horário do médico
        var conflict = await _context.Appointments
            .AnyAsync(a => a.DoctorId == appointment.DoctorId && a.Date == appointment.Date && a.Id != appointment.Id);

        if (conflict)
            return BadRequest("The doctor already has an appointment at that time.");

        _context.Entry(appointment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Appointments.Any(a => a.Id == id))
                return NotFound();
            else
                throw;
        }

        // Recarrega o appointment atualizado com os relacionamentos
        var updatedAppointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == appointment.Id);

        return Ok(updatedAppointment);
    }

    // DELETE: api/Appointment/5
    [HttpDelete("{id}")]
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
