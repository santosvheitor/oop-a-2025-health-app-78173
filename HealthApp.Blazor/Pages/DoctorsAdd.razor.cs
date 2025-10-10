using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages;

public partial class DoctorsAdd
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private Doctor doctor = new();

    private async Task HandleValidSubmit()
    {
        await Http.PostAsJsonAsync("api/doctor", doctor);
        NavigationManager.NavigateTo("/doctors");
    }

    private void Cancel() => NavigationManager.NavigateTo("/doctors");
}