using HealthApp.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages
{
    public partial class MedicalRecordsPage : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        public List<MedicalRecord> records { get; set; } = new List<MedicalRecord>();

        protected override async Task OnInitializedAsync()
        {
            records = await Http.GetFromJsonAsync<List<MedicalRecord>>("api/medicalrecords") ?? new List<MedicalRecord>();
        }

        public void AddRecord()
        {
            Navigation.NavigateTo("/medicalrecords/add");
        }

        public void EditRecord(int id)
        {
            Navigation.NavigateTo($"/medicalrecords/edit/{id}");
        }

        public async Task DeleteRecord(int id)
        {
            await Http.DeleteAsync($"api/medicalrecords/{id}");
            records = await Http.GetFromJsonAsync<List<MedicalRecord>>("api/medicalrecords") ?? new List<MedicalRecord>();
        }
    }
}