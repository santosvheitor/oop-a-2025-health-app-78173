using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages
{
    public partial class PatientsEdit : ComponentBase
    {
        [Parameter] public int Id { get; set; }
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private Patient patient = new();
        private bool userIsAdmin;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            userIsAdmin = authState.User.IsInRole("Admin");

            if (userIsAdmin)
                patient = await Http.GetFromJsonAsync<Patient>($"api/patient/{Id}");
        }

        private async Task HandleValidSubmit()
        {
            await Http.PutAsJsonAsync($"api/patient/{Id}", patient);
            NavigationManager.NavigateTo("/patients");
        }
    }
}