using System.ComponentModel.DataAnnotations;

namespace ScheduleService.Models
{
    public class Provider
    {
        [Key]
        public Guid ProviderUid { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string TimeZoneId { get; set; }

        public Provider(string name, string timeZoneId)
        {
            ProviderUid = Guid.NewGuid();
            Name = name;
            TimeZoneId = timeZoneId;
        }
    }
}
