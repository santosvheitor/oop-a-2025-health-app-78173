using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthApp.Blazor.Pages;

public partial class DoctorsPage : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private List<Doctor> doctors = new();
    private bool userIsAdmin;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        userIsAdmin = authState.User.IsInRole("Admin");

        try
        {
            // <-- Use o endpoint correto da API
            doctors = await Http.GetFromJsonAsync<List<Doctor>>("api/doctors");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Erro ao carregar médicos: {ex.Message}");
            doctors = new List<Doctor>(); // fallback
        }
    }
    
    private void AddDoctor() => NavigationManager.NavigateTo("/doctors/add");
    private void EditDoctor(int id) => NavigationManager.NavigateTo($"/doctors/edit/{id}");

    private async Task DeleteDoctor(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (confirmed)
        {
            await Http.DeleteAsync($"api/doctors/{id}"); // plural
            doctors = await Http.GetFromJsonAsync<List<Doctor>>("api/doctors"); // plural
        }
    }
}