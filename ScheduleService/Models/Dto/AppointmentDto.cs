namespace ScheduleService.Models.Dto
{
    public class AppointmentDto
    {
        public Guid AppointmentUID { get; set; }
        public Guid AppointmentSlotUid { get; set; }
        public AppointmentSlot AppointmentSlot { get; set; }
        public Guid ClientUid { get; set; }
        public Client Client { get; set; }
        public DateTime BookedDateTimeUTC { get; set; }
        public bool Confirmed { get; set; }
    }
}
