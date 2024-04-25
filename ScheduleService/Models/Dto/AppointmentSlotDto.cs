namespace ScheduleService.Models.Dto
{
    public class AppointmentSlotDto
    {
        public Guid AppointmentSlotUid { get; set; }
        public Guid ProviderUid { get; set; }
        public Provider Provider { get; set; }
        public DateTime StartDateTimeUTC { get; set; }
        public DateTime EndDateTimeUTC { get; set; }
    }
}
