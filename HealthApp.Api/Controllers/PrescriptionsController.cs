using HealthApp.Data.Data;
using HealthApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionsController : ControllerBase
{
    private readonly HospitalContext _context;

    public PrescriptionsController(HospitalContext context)
    {
        _context = context;
    }

    // DTO para envio ao Blazor
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Medication { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Patient")]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetMyPrescriptions()
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized("Usuário não autenticado.");

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Email == userEmail);
        if (patient == null)
            return BadRequest("Paciente não encontrado para o usuário atual.");

        var prescriptions = await _context.Prescriptions
            .Where(p => p.PatientId == patient.Id)
            .ToListAsync();

        var dtos = prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            AppointmentId = p.AppointmentId,
            Medication = p.Medication,
            Dosage = p.Dosage,
            Instructions = p.Instructions,
            IssueDate = p.IssueDate
        }).ToList();

        return dtos;
    }
}