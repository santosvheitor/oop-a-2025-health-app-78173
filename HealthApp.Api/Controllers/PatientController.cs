using HealthApp.Data.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly HospitalContext _context;

    public PatientController(HospitalContext context)
    {
        _context = context;
    }

    // GET: api/patient
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
    {
        return await _context.Patients.ToListAsync();
    }

    // GET: api/patient/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Patient>> GetPatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound();
        return patient;
    }

    // POST: api/patient
    [HttpPost]
    public async Task<ActionResult<Patient>> PostPatient(Patient patient)
    {
        // 🔹 1. Verificação de campos obrigatórios
        if (string.IsNullOrWhiteSpace(patient.FullName) || string.IsNullOrWhiteSpace(patient.Email))
            return BadRequest("Full name and email are required.");
        
        
        //Check if there is already a patient with the same email (to avoid duplicates).
        var exists = await _context.Patients.AnyAsync(p => p.Email == patient.Email);
        if (exists)
            return BadRequest("There is already a patient registered with this email.");
        
        // Saving 
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
    }

    // PUT: api/patient/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPatient(int id, Patient patient)
    {
        if (id != patient.Id) return BadRequest();
        _context.Entry(patient).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/patient/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound();

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}