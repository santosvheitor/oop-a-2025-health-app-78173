using HealthApp.Data.Data;
using HealthApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly HospitalContext _context;

    public DoctorsController(HospitalContext context)
    {
        _context = context;
    }

    // Todos os médicos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
    {
        return await _context.Doctors.ToListAsync();
    }

    // Perfil do médico logado
    [HttpGet("me")]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<Doctor>> GetMyProfile()
    {
        var email = User.Identity?.Name; // ⚡ usar email do JWT
        if (string.IsNullOrEmpty(email))
            return Unauthorized();

        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Email == email);
        if (doctor == null)
            return NotFound("Doctor not found.");

        return Ok(doctor);
    }


    // Médico por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Doctor>> GetDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();
        return doctor;
    }

    // Criar médico (Admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
    {
        if (string.IsNullOrWhiteSpace(doctor.FullName) || string.IsNullOrWhiteSpace(doctor.Email))
            return BadRequest("Nome completo e email são obrigatórios.");

        if (await _context.Doctors.AnyAsync(d => d.Email == doctor.Email))
            return BadRequest("Já existe um médico com este email.");

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
    }

    // Atualizar (Admin ou Doctor)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
    {
        if (id != doctor.Id) return BadRequest();
        _context.Entry(doctor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Remover (Admin)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
