using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages;

public partial class Appointments
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<Appointment> appointments = new();

    protected override async Task OnInitializedAsync()
    {
        appointments = await Http.GetFromJsonAsync<List<Appointment>>("api/Appointment");
    }

    private void AddAppointment()
    {
        NavigationManager.NavigateTo("/appointments/add");
    }

    private void EditAppointment(int id)
    {
        NavigationManager.NavigateTo($"/appointments/edit/{id}");
    }

    private async Task DeleteAppointment(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this appointment?");
        if (confirmed)
        {
            await Http.DeleteAsync($"api/Appointment/{id}");
            appointments = await Http.GetFromJsonAsync<List<Appointment>>("api/Appointment");
        }
    }
}