using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ScheduleService.Data;
using ScheduleService.Models;

namespace ScheduleService.Jobs
{
    public class BackgroundJobService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        private int confirmationWindow;

        public BackgroundJobService(IServiceProvider services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;
            confirmationWindow = _configuration.GetValue<int>("AppSettings:ConfirmationWindowLength");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    IEnumerable<Appointment> appointments = db.Appointments
                        .Where(x => x.BookedDateTimeUTC.AddMinutes(confirmationWindow) < DateTime.UtcNow && !x.Confirmed)
                        .ToList();

                    foreach (Appointment appointment in appointments)
                    {
                        db.Remove(appointment);
                    }
                    db.SaveChanges();
                }
                // Wait for 5 minutes before running the job again
                await Task.Delay(TimeSpan.FromMinutes(confirmationWindow), stoppingToken);
            }
        }
    }
}
