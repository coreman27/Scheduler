namespace ScheduleService.Models.Dto
{
    // This is used to return the AppointmentSlot information back to the client to avoid returning back all provider information.
    public class ClientAppointmentSlotDto
    {
        public Guid AppointmentSlotUid { get; set; }
        public string ProviderName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public ClientAppointmentSlotDto(Guid appointmentSlotUid, string providerName, DateTime startDateTime, DateTime endDateTime)
        {
            AppointmentSlotUid = appointmentSlotUid;
            ProviderName = providerName;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }


    }
}
