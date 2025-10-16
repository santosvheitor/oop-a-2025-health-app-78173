using HealthApp.Domain.Models; 
using Microsoft.AspNetCore.Components; 
using Microsoft.AspNetCore.Components.Authorization; 
using System.Net.Http.Json;

namespace HealthApp.Blazor.Pages
{
    public partial class AppointmentsEdit : ComponentBase
    {
        [Parameter] public int Id { get; set; } 
        [Inject] private HttpClient Http { get; set; } = default!; 
        [Inject] private NavigationManager NavigationManager { get; set; } = default!; 
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!; 
        
        private Appointment appointment = new(); 
        private List<Patient> patients = new(); 
        private List<Doctor> doctors = new(); 
        private bool userIsAdmin;

        protected override async Task OnInitializedAsync()
        {
            var authState = await 
                AuthStateProvider.GetAuthenticationStateAsync(); 
            userIsAdmin = authState.User.IsInRole("Admin");

            if (userIsAdmin)
            {
                appointment = await 
                    Http.GetFromJsonAsync<Appointment>($"api/appointment/{Id}"); 
                patients = await Http.GetFromJsonAsync<List<Patient>>
                    ("api/patient"); 
                doctors = await Http.GetFromJsonAsync<List<Doctor>>
                    ("api/doctor");
            }
        }

        private async Task HandleValidSubmit()
        {
            await Http.PutAsJsonAsync($"api/appointment/{Id}", 
                appointment); 
            NavigationManager.NavigateTo("/appointments");
        }
    }
}