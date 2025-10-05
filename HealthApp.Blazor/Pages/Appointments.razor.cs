using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Domain.Models; 
using Microsoft.AspNetCore.Components;

namespace HealthApp.Blazor.Pages;

    public partial class Appointments : ComponentBase
    {
        private List<Appointment>? appointments; // se refere ao Domain.Model.Appointment

        [Inject]
        public HttpClient Http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            appointments = await Http.GetFromJsonAsync<List<Appointment>>("api/Appointment");
        }
    }
