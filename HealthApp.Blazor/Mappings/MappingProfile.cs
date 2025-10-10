using AutoMapper;
using HealthApp.Blazor.Models;
using HealthApp.Domain.Models;

namespace HealthApp.Blazor.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Doctor, DoctorModel>().ReverseMap();
        CreateMap<Patient, PatientModel>().ReverseMap();
        CreateMap<Appointment, AppointmentModel>().ReverseMap();
    }
}