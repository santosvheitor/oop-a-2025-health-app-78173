using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthApp.Blazor.Pages
{
    public partial class AppointmentsAdd
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // Usando os tipos do Domain explicitamente para evitar conflito
        private HealthApp.Domain.Models.Appointment appointment = new();
        private List<HealthApp.Domain.Models.Patient> patients = new();
        private List<HealthApp.Domain.Models.Doctor> doctors = new();

        protected override async Task OnInitializedAsync()
        {
            // Carrega os pacientes e médicos usando namespace completo
            patients = await Http.GetFromJsonAsync<List<HealthApp.Domain.Models.Patient>>("api/patient") ?? new List<HealthApp.Domain.Models.Patient>();
            doctors = await Http.GetFromJsonAsync<List<HealthApp.Domain.Models.Doctor>>("api/doctor") ?? new List<HealthApp.Domain.Models.Doctor>();
        }

        private async Task HandleValidSubmit()
        {
            await Http.PostAsJsonAsync("api/appointment", appointment);
            NavigationManager.NavigateTo("/appointments");
        }

        private void Cancel() => NavigationManager.NavigateTo("/appointments");
    }
}