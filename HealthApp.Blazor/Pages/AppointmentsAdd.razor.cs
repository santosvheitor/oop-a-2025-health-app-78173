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
        private string patientFullName = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Load Doctors list
                doctors = await Http.GetFromJsonAsync<List<Doctor>>("api/doctors") ?? new List<Doctor>();

                // Initialize logged in patient
               
                patientFullName = "Logged Patient"; 
                appointment.PatientId = 1; // Set the PatientId of the logged in patient
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private async Task HandleAddAppointment()
        {
            appointment.Status = "Pending";

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
