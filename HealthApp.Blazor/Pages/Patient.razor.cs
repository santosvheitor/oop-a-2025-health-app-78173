using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthApp.Blazor.Pages;


public partial class Patient 
{
    // Inject HttpClient
    [Inject] private HttpClient Http { get; set; } = default!;

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private List<PatientModel> patients = new();

    protected override async Task OnInitializedAsync()
    {
        patients = await Http.GetFromJsonAsync<List<PatientModel>>("api/patient");
    }

    private void AddPatient()
    {
        NavigationManager.NavigateTo("/patients/add");
    }

    private void EditPatient(int id)
    {
        NavigationManager.NavigateTo($"/patients/edit/{id}");
    }

    private async Task DeletePatient(int id)
    {
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (confirmed)
        {
            await Http.DeleteAsync($"api/patient/{id}");
            patients = await Http.GetFromJsonAsync<List<PatientModel>>("api/patient");
        }
    }
}