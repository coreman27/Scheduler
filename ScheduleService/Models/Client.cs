using System.ComponentModel.DataAnnotations;

namespace ScheduleService.Models
{
    public class Client
    {
        [Key]
        public Guid ClientUid { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string TimeZoneId { get; set; }

        public Client(string name, string timeZoneId)
        {
            ClientUid = Guid.NewGuid();
            Name = name;
            TimeZoneId = timeZoneId;
        }
    }
}
