using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models
{
    public class Appointment
    {
        [Key]
        public Guid AppointmentUID { get; set; }

        [Required]
        public Guid AppointmentSlotUid { get; set; }


        [ForeignKey("AppointmentSlotUid")]
        public AppointmentSlot AppointmentSlot { get; set; }

        public Guid ClientUid { get; set; }

        [ForeignKey("ClientUid")]
        public Client Client { get; set; }
        public DateTime BookedDateTimeUTC { get; set; }
        public bool Confirmed { get; set; }

        public Appointment(Guid appointmentSlotUid, Guid clientUid, DateTime bookedDateTimeUTC)
        {
            AppointmentUID = Guid.NewGuid();
            AppointmentSlotUid = appointmentSlotUid;
            ClientUid = clientUid;
            BookedDateTimeUTC = bookedDateTimeUTC;
        }
    }
}
