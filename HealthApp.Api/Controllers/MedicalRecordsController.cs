using HealthApp.Data.Data;
using HealthApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Doctor")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly HospitalContext _context;

        public MedicalRecordsController(HospitalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicalRecord>>> GetAll()
        {
            return await _context.MedicalRecords
                                 .Include(r => r.Patient)
                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> Get(int id)
        {
            var record = await _context.MedicalRecords
                                       .Include(r => r.Patient)
                                       .FirstOrDefaultAsync(r => r.Id == id);
            if (record == null)
                return NotFound();

            return record;
        }

        // ✅ ALTERADO AQUI — agora usa o DTO (MedicalRecordCreateModel)
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] MedicalRecordCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Busca o paciente existente
            var patient = await _context.Patients.FindAsync(model.PatientId);
            if (patient == null)
                return BadRequest("Paciente não encontrado.");

            // Cria o novo registro médico com o paciente vinculado
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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
