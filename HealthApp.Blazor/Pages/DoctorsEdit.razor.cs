using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages;

public partial class DoctorsEdit
{
    [Parameter] public int Id { get; set; }
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private Doctor doctor = new();

    protected override async Task OnInitializedAsync()
    {
        doctor = await Http.GetFromJsonAsync<Doctor>($"api/doctor/{Id}") ?? new Doctor();
    }

    private async Task HandleValidSubmit()
    {
        await Http.PutAsJsonAsync($"api/doctor/{Id}", doctor);
        NavigationManager.NavigateTo("/doctors");
    }

    private void Cancel() => NavigationManager.NavigateTo("/doctors");
}