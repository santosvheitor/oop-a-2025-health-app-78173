using HealthApp.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages
{
    public partial class MedicalRecordsEdit : ComponentBase
    {
        [Parameter] public int Id { get; set; }

        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        public MedicalRecord record { get; set; } = new MedicalRecord();
        public List<Patient> patients { get; set; } = new List<Patient>();

        protected override async Task OnInitializedAsync()
        {
            record = await Http.GetFromJsonAsync<MedicalRecord>($"api/medicalrecords/{Id}") ?? new MedicalRecord();
            patients = await Http.GetFromJsonAsync<List<Patient>>("api/patients") ?? new List<Patient>();
        }

        public async Task HandleUpdate()
        {
            if (record.PatientId == 0)
            {
                return; // não pode salvar sem paciente
            }

            await Http.PutAsJsonAsync($"api/medicalrecords/{Id}", record);
            Navigation.NavigateTo("/medicalrecords");
        }
    }
}