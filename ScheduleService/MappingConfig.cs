using AutoMapper;
using ScheduleService.Models;
using ScheduleService.Models.Dto;

namespace ScheduleService
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<AppointmentDto, Appointment>();
                config.CreateMap<Appointment, AppointmentDto>();
                config.CreateMap<AppointmentSlotDto, AppointmentSlot>();
                config.CreateMap<AppointmentSlot, AppointmentSlotDto>();
                config.CreateMap<AppointmentSlotDto, ClientAppointmentSlotDto>();
                config.CreateMap<ProviderDto, Provider>();
                config.CreateMap<Provider, ProviderDto>();
                config.CreateMap<ClientDto, Client>();
                config.CreateMap<Client, ClientDto>();
            });
            return mappingConfig;
        }
    }
}
