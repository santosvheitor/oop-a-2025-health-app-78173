using HealthApp.Data.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly HospitalContext _context;

    public DoctorController(HospitalContext context)
    {
        _context = context;
    }

    // GET: api/doctor
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
    {
        return await _context.Doctors.ToListAsync();
    }

    // GET: api/doctor/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Doctor>> GetDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();
        return doctor;
    }

    // POST: api/doctor
    [HttpPost]
    public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
    {
        // Checking mandatory fields
        if (string.IsNullOrWhiteSpace(doctor.FullName) || string.IsNullOrWhiteSpace(doctor.Email))
            return BadRequest("Full name and email are required.");
        
        //Check if there is already a doctor with the same email.
        var exists = await _context.Doctors.AnyAsync(d => d.Email == doctor.Email);
        if (exists)
            return BadRequest("There is already a doctor registered with this email.");
        
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
    }

    // PUT: api/doctor/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
    {
        if (id != doctor.Id) return BadRequest();
        _context.Entry(doctor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/doctor/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}