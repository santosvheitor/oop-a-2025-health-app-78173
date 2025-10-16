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
    private Appointment newAppointment = new();
    private List<Doctor> doctors = new();
    private bool showAddModal = false;

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

        await LoadAppointments();
        await LoadDoctors();
    }

    private async Task LoadAppointments()
    {
        try
        {
            appointments = userIsPatient
                ? await Http.GetFromJsonAsync<List<Appointment>>("api/appointments/mine")
                : await Http.GetFromJsonAsync<List<Appointment>>("api/appointments");
        }
        catch
        {
            appointments = new();
        }
    }

    private async Task LoadDoctors()
    {
        try
        {
            doctors = await Http.GetFromJsonAsync<List<Doctor>>("api/doctors") ?? new();
        }
        catch
        {
            doctors = new();
        }
    }

    private void ShowAddModal()
    {
        newAppointment = new Appointment { Date = DateTime.Now.AddDays(1) };
        showAddModal = true;
    }

    private async Task AddAppointment()
    {
        if (newAppointment.DoctorId <= 0)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Please select a doctor.");
            return;
        }

        var response = await Http.PostAsJsonAsync("api/appointments", newAppointment);

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            await JSRuntime.InvokeVoidAsync("alert", error?["message"] ?? "Time slot unavailable.");
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Error creating appointment.");
            return;
        }

        await JSRuntime.InvokeVoidAsync("alert", "Appointment created successfully!");
        showAddModal = false;
        await LoadAppointments();
    }

    private void EditAppointment(int id) => NavigationManager.NavigateTo($"/appointments/edit/{id}");

    private async Task DeleteAppointment(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (confirmed)
        {
            await Http.DeleteAsync($"api/appointments/{id}");
            await LoadAppointments();
        }
    }

    private async Task ConfirmAppointment(int id)
    {
        var response = await Http.PatchAsync($"api/appointments/{id}/confirm", null);
        if (response.IsSuccessStatusCode)
        {
            await LoadAppointments();
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", "Error confirming appointment.");
        }
    }
}
