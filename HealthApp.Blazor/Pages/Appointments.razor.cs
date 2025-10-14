using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthApp.Blazor.Pages;

public partial class Appointments : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private List<Appointment> appointments = new();
    private bool userIsAdmin;
    private bool userIsPatient;
    private bool userIsDoctor;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        userIsAdmin = user.IsInRole("Admin");
        userIsPatient = user.IsInRole("Patient");
        userIsDoctor = user.IsInRole("Doctor");

        try
        {
            if (userIsPatient)
                appointments = await Http.GetFromJsonAsync<List<Appointment>>("api/appointments/mine");
            else
                appointments = await Http.GetFromJsonAsync<List<Appointment>>("api/appointments");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Erro ao carregar appointments: {ex.Message}");
            appointments = new();
        }
    }

    private void AddAppointment() => NavigationManager.NavigateTo("/appointments/add");
    private void EditAppointment(int id) => NavigationManager.NavigateTo($"/appointments/edit/{id}");

    private async Task DeleteAppointment(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (confirmed)
        {
            await Http.DeleteAsync($"api/appointments/{id}");
            appointments = await Http.GetFromJsonAsync<List<Appointment>>("api/appointments");
        }
    }

    private async Task ConfirmAppointment(int id)
    {
        // Encontrar o appointment
        var appointment = appointments.FirstOrDefault(a => a.Id == id);
        if (appointment != null)
        {
            appointment.Status = "Confirmed";

            // Enviar atualização para o servidor via PUT
            var response = await Http.PutAsJsonAsync($"api/appointments/{id}", appointment);
            if (response.IsSuccessStatusCode)
            {
                // Atualizar lista local para refletir mudança
                appointments = await Http.GetFromJsonAsync<List<Appointment>>("api/appointments");
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao confirmar appointment: {errorMsg}");
            }
        }
    }
}
