using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages
{
    public partial class AppointmentsEdit : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Parameter] public int Id { get; set; }

        private Appointment appointment = new();
        private List<Patient> patients = new();
        private List<Doctor> doctors = new();
        private bool isLoading = true;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Use plural endpoint
                appointment = await Http.GetFromJsonAsync<Appointment>($"api/appointments/{Id}") ?? new Appointment();
                patients = await Http.GetFromJsonAsync<List<Patient>>("api/patients") ?? new List<Patient>();
                doctors = await Http.GetFromJsonAsync<List<Doctor>>("api/doctors") ?? new List<Doctor>();
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar dados: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                // Plural endpoint
                var response = await Http.PutAsJsonAsync($"api/appointments/{Id}", appointment);
                if (response.IsSuccessStatusCode)
                {
                    NavigationManager.NavigateTo("/appointments");
                }
                else
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    errorMessage = $"Error saving changes: {msg}";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error to connect the server: {ex.Message}";
            }
        }

        private void Cancel() => NavigationManager.NavigateTo("/appointments");
    }
}
