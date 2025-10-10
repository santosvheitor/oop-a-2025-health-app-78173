using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages;

public partial class DoctorsPage : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<Doctor> doctors = new();

    protected override async Task OnInitializedAsync()
    {
        doctors = await Http.GetFromJsonAsync<List<Doctor>>("api/doctor");
    }

    private void AddDoctor() => NavigationManager.NavigateTo("/doctors/add");
    private void EditDoctor(int id) => NavigationManager.NavigateTo($"/doctors/edit/{id}");

    private async Task DeleteDoctor(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (confirmed)
        {
            await Http.DeleteAsync($"api/doctor/{id}");
            doctors = await Http.GetFromJsonAsync<List<Doctor>>("api/doctor");
        }
    }
}