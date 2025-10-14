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

    // ✅ Retorna todos os médicos (para Admin, ou público se quiser)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
    {
        return await _context.Doctors.ToListAsync();
    }

    // ✅ Retorna o perfil do médico logado
    [HttpGet("me")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return BadRequest("Usuário inválido.");

        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdentityUserId == userId);
        if (doctor == null)
            return NotFound("Médico não encontrado.");

        return Ok(doctor);
    }

    // ✅ Retorna um médico por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Doctor>> GetDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
            return NotFound();

        return doctor;
    }

    // ✅ Cria novo médico (somente Admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
    {
        if (string.IsNullOrWhiteSpace(doctor.FullName) || string.IsNullOrWhiteSpace(doctor.Email))
            return BadRequest("Nome completo e email são obrigatórios.");

        var exists = await _context.Doctors.AnyAsync(d => d.Email == doctor.Email);
        if (exists)
            return BadRequest("Já existe um médico cadastrado com este email.");

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
    }

    // ✅ Atualiza dados
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
    {
        if (id != doctor.Id)
            return BadRequest();

        _context.Entry(doctor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ✅ Remove médico
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
            return NotFound();

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
