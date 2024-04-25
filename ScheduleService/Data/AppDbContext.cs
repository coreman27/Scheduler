using Microsoft.EntityFrameworkCore;
using ScheduleService.Models;

namespace ScheduleService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentSlot> AppointmentSlots { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Provider> Providers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>().HasData(new Client("client", "Eastern Standard Time"));
            modelBuilder.Entity<Provider>().HasData(new Provider("provider", "Central Standard Time"));
        }
    }


}
 