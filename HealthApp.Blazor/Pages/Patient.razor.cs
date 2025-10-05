using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages
{
    public partial class Patient
    {
        [Inject]
        private HttpClient Http { get; set; }

        private List<PatientModel> patients;

        protected override async Task OnInitializedAsync()
        {
            patients = await Http.GetFromJsonAsync<List<PatientModel>>("api/patient");
        }

        public class PatientModel
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
        }
    }
}