namespace HealthApp.Data.Models;

public class MedicalRecordCreateModel
{
    public string Diagnosis { get; set; }
    public string Treatment { get; set; }
    public DateTime RecordDate { get; set; }
    public int PatientId { get; set; }
}