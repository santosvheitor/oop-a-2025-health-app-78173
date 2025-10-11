using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthApp.Blazor.Pages;

public partial class PatientsPage : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private List<Patient> patients = new();
    private bool userIsAdmin;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            patients = await Http.GetFromJsonAsync<List<Patient>>("api/patients"); // plural
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            patients = new List<Patient>();
        }
    }

    private void AddPatient() => NavigationManager.NavigateTo("/patients/add");
    private void EditPatient(int id) => NavigationManager.NavigateTo($"/patients/edit/{id}");

    private async Task DeletePatient(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (confirmed)
        {
            await Http.DeleteAsync($"api/patients/{id}"); // plural
            patients = await Http.GetFromJsonAsync<List<Patient>>("api/patients"); // plural
        }
    }
}