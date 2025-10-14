using HealthApp.Blazor.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace HealthApp.Blazor.Pages;

public partial class PatientsPage : ComponentBase
{
    private string searchTerm = "";

    public List<DoctorModel> doctors { get; set; } = new();
    public List<AppointmentModel> appointments { get; set; } = new();
    public List<PrescriptionModel> prescriptions { get; set; } = new();

    private DoctorModel? selectedDoctor;
    private DateTime? bookDateTime;

    private AppointmentModel? rescheduleAppointment;
    private DateTime? rescheduleDateTime;

    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadDoctors();
        await LoadAppointments();
        await LoadPrescriptions();
    }

    private async Task LoadDoctors() =>
        doctors = await Http.GetFromJsonAsync<List<DoctorModel>>("api/doctors") ?? new();

    private async Task LoadAppointments()
    {
        try
        {
            appointments = await Http.GetFromJsonAsync<List<AppointmentModel>>("api/appointments/mine") ?? new();
        }
        catch
        {
            appointments = new();
        }
    }

    private async Task LoadPrescriptions()
    {
        try
        {
            var result = await Http.GetFromJsonAsync<List<PrescriptionModel>>("api/prescriptions/mine");
            prescriptions = result ?? new();
        }
        catch
        {
            prescriptions = new();
        }
    }


    private void SearchDoctors()
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            _ = LoadDoctors();
            return;
        }

        doctors = doctors
            .Where(d => d.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                     || d.Specialty.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private async Task ResetSearch()
    {
        searchTerm = "";
        await LoadDoctors();
    }

    private async Task OpenBookModal(DoctorModel d)
    {
        selectedDoctor = d;
        bookDateTime = DateTime.Now.AddDays(1);
        await JS.InvokeVoidAsync("bootstrapInterop.showModal", "#bookModal");
    }

    private async Task CloseBookModal()
    {
        selectedDoctor = null;
        await JS.InvokeVoidAsync("bootstrapInterop.hideModal", "#bookModal");
    }

    private async Task ConfirmBook()
    {
        if (selectedDoctor == null || bookDateTime == null) return;

        var create = new
        {
            DoctorId = selectedDoctor.Id,
            Date = bookDateTime.Value
        };

        var resp = await Http.PostAsJsonAsync("api/appointments", create);
        if (resp.IsSuccessStatusCode)
        {
            await CloseBookModal();
            await LoadAppointments();
            await JS.InvokeVoidAsync("alert", "Appointment booked.");
        }
        else
        {
            var txt = await resp.Content.ReadAsStringAsync();
            await JS.InvokeVoidAsync("alert", $"Failed to book: {resp.StatusCode}\n{txt}");
        }
    }

    private bool CanCancel(AppointmentModel a) =>
        (a.Date - DateTime.UtcNow).TotalHours > 48;

    private async Task CancelAppointment(int appointmentId)
    {
        var ok = await JS.InvokeAsync<bool>("confirm", "Are you sure you want to cancel this appointment?");
        if (!ok) return;

        var resp = await Http.DeleteAsync($"api/appointments/{appointmentId}");
        if (resp.IsSuccessStatusCode)
        {
            await LoadAppointments();
            await JS.InvokeVoidAsync("alert", "Appointment canceled.");
        }
        else
        {
            var txt = await resp.Content.ReadAsStringAsync();
            await JS.InvokeVoidAsync("alert", $"Failed to cancel: {txt}");
        }
    }

    private async Task OpenRescheduleModal(AppointmentModel a)
    {
        rescheduleAppointment = a;
        rescheduleDateTime = a.Date;
        await JS.InvokeVoidAsync("bootstrapInterop.showModal", "#reschedModal");
    }

    private async Task CloseReschedModal()
    {
        rescheduleAppointment = null;
        await JS.InvokeVoidAsync("bootstrapInterop.hideModal", "#reschedModal");
    }

    private async Task ConfirmReschedule()
    {
        if (rescheduleAppointment == null || rescheduleDateTime == null) return;

        var update = new
        {
            Date = rescheduleDateTime.Value
        };

        var resp = await Http.PutAsJsonAsync($"api/appointments/{rescheduleAppointment.Id}/reschedule", update);
        if (resp.IsSuccessStatusCode)
        {
            await CloseReschedModal();
            await LoadAppointments();
            await JS.InvokeVoidAsync("alert", "Appointment rescheduled.");
        }
        else
        {
            var txt = await resp.Content.ReadAsStringAsync();
            await JS.InvokeVoidAsync("alert", $"Failed to reschedule: {txt}");
        }
    }
}
