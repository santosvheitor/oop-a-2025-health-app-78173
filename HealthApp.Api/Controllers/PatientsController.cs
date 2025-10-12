using HealthApp.Data.Data;
using HealthApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApp.Api.Controllers;

[Authorize] // Todos os endpoints exigem usuário autenticado
[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly HospitalContext _context;

    public PatientsController(HospitalContext context)
    {
        _context = context;
    }

    // GET: api/patients
    [HttpGet]
    [Authorize(Roles = "Admin")] // Apenas admins podem listar todos os pacientes
    public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
    {
        return await _context.Patients.ToListAsync();
    }

    // GET: api/patients/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Doctor")] // Admins e Doctors podem acessar pacientes por ID
    public async Task<ActionResult<Patient>> GetPatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
            return NotFound("Patient not found.");

        return patient;
    }

    // GET: api/patients/me
    [HttpGet("me")]
    [Authorize(Roles = "Patient")]
    public async Task<ActionResult<Patient>> GetLoggedInPatient()
    {
        // Aqui usamos o email do token para buscar o paciente
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
            return Unauthorized("User is not authenticated.");

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Email == email);

        if (patient == null)
            return NotFound("Patient not found.");

        return patient;
    }

    // GET: api/patients/debug
    [HttpGet("debug")]
    public ActionResult<string> DebugUser()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);
        return $"UserId: {identityUserId}, Role: {role}";
    }

    // POST: api/patients
    [HttpPost]
    [AllowAnonymous] // Permite que novos pacientes se registrem sem estar logado
    public async Task<ActionResult<Patient>> PostPatient(Patient patient)
    {
        if (string.IsNullOrWhiteSpace(patient.FullName) || string.IsNullOrWhiteSpace(patient.Email))
            return BadRequest("Full name and email are required.");

        var exists = await _context.Patients.AnyAsync(p => p.Email == patient.Email);
        if (exists)
            return BadRequest("There is already a patient registered with this email.");

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
    }

    // PUT: api/patients/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Patient")]
    public async Task<IActionResult> PutPatient(int id, Patient patient)
    {
        if (id != patient.Id)
            return BadRequest("Patient ID mismatch.");

        _context.Entry(patient).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/patients/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
            return NotFound("Patient not found.");

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
