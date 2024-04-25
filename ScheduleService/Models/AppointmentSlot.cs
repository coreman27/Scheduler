using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models
{
    public class AppointmentSlot
    {
        [Key]
        public Guid AppointmentSlotUid { get; set; }
        [Required]
        public Guid ProviderUid { get; set; }
        [ForeignKey("ProviderUid")]
        public Provider Provider { get; set; }
        [Required]
        public DateTime StartDateTimeUTC { get; set; }
        [Required]
        public DateTime EndDateTimeUTC { get; set; }

        public AppointmentSlot(Guid providerUid, DateTime startDateTimeUTC)
        {
            AppointmentSlotUid = Guid.NewGuid();
            ProviderUid = providerUid;
            StartDateTimeUTC = startDateTimeUTC;
        }
    }
}
