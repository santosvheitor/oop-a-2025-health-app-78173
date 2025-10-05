using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages
{
    public partial class Doctor
    {
        [Inject]
        private HttpClient Http { get; set; }

        private List<DoctorModel> doctors;

        protected override async Task OnInitializedAsync()
        {
            doctors = await Http.GetFromJsonAsync<List<DoctorModel>>("api/doctor");
        }

        public class DoctorModel
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Specialty { get; set; }
            public string Email { get; set; }
        }
    }
}