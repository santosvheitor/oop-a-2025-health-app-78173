using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages
{
    public partial class AppointmentsAdd : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private Appointment appointment = new();
        private List<Doctor> doctors = new();
        private string? patientFullName;
        private int patientId;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Inicializa a data do agendamento para hoje
                appointment.Date = DateTime.Today;

                // Carrega a lista de médicos
                doctors = await Http.GetFromJsonAsync<List<Doctor>>("api/doctors") 
                           ?? new List<Doctor>();

                // Carrega o paciente logado (sem redirecionar)
                var patient = await Http.GetFromJsonAsync<Patient>("api/patients/me");
                if (patient != null)
                {
                    patientFullName = patient.FullName;
                    patientId = patient.Id;
                    appointment.PatientId = patientId;
                }
                else
                {
                    Console.WriteLine("Patient not found.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleAddAppointment()
        {
            appointment.Status = "Pending";
            appointment.PatientId = patientId;

            if (appointment.PatientId == 0 || appointment.DoctorId == 0 || appointment.Date == default)
            {
                Console.WriteLine("Error: Fill in all required fields.");
                return;
            }

            try
            {
                var response = await Http.PostAsJsonAsync("api/appointments", appointment);
                if (response.IsSuccessStatusCode)
                {
                    NavigationManager.NavigateTo("/appointments");
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error adding appointment: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding appointment: {ex.Message}");
            }
        }
    }
}
