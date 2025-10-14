using HealthApp.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace HealthApp.Blazor.Pages
{
    public partial class MedicalRecordsPage : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        public List<MedicalRecord> records { get; set; } = new();
        private bool canEdit = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            canEdit = user.IsInRole("Doctor") || user.IsInRole("Admin");

            try
            {
                records = await Http.GetFromJsonAsync<List<MedicalRecord>>("api/medicalrecords") ?? new();
            }
            catch
            {
                records = new();
            }
        }

        private void AddRecord() => Navigation.NavigateTo("/medicalrecords/add");
        private void EditRecord(int id) => Navigation.NavigateTo($"/medicalrecords/edit/{id}");

        private async Task DeleteRecord(int id)
        {
            if (!canEdit) return;

            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this record?");
            if (!confirmed) return;

            await Http.DeleteAsync($"api/medicalrecords/{id}");
            records = await Http.GetFromJsonAsync<List<MedicalRecord>>("api/medicalrecords") ?? new();
        }
    }
}