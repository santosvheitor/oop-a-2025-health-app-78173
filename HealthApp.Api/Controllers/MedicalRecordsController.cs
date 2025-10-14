using HealthApp.Data.Data;
using HealthApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly HospitalContext _context;

        public MedicalRecordsController(HospitalContext context)
        {
            _context = context;
        }

        // ✅ GET: api/medicalrecords
        [HttpGet]
        public async Task<ActionResult<List<MedicalRecord>>> GetAll()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IQueryable<MedicalRecord> query = _context.MedicalRecords
                .Include(r => r.Patient)
                .AsQueryable();

            if (userRole == "Patient")
            {
                // 🔍 Busca o paciente com base no IdentityUserId (do AspNetUsers)
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

                if (patient == null)
                    return Forbid();

                // ✅ Filtra apenas os registros do paciente logado
                query = query.Where(r => r.PatientId == patient.Id);
            }

            var records = await query.ToListAsync();
            return Ok(records);
        }

        // ✅ GET: api/medicalrecords/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> Get(int id)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
                return NotFound();

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userRole == "Patient")
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

                if (patient == null || record.PatientId != patient.Id)
                    return Forbid();
            }

            return Ok(record);
        }

        // ✅ POST: api/medicalrecords
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> Add([FromBody] MedicalRecordCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = await _context.Patients.FindAsync(model.PatientId);
            if (patient == null)
                return BadRequest("Paciente não encontrado.");

            var record = new MedicalRecord
            {
                Diagnosis = model.Diagnosis,
                Treatment = model.Treatment,
                RecordDate = model.RecordDate,
                PatientId = model.PatientId,
                Patient = patient
            };

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            return Ok(record);
        }

        // ✅ PUT: api/medicalrecords/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> Update(int id, MedicalRecord record)
        {
            if (id != record.Id)
                return BadRequest();

            _context.Entry(record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.MedicalRecords.Any(r => r.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/medicalrecords/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> Delete(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null)
                return NotFound();

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
