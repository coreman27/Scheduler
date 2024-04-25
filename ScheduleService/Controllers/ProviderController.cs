using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleService.Data;
using ScheduleService.Models;
using ScheduleService.Models.Dto;
using System;


namespace ScheduleService.Controllers
{
    [Route("api/provider")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;
        private int _appointmentSlotInterval;

        public ProviderController(AppDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _configuration = configuration;
            _appointmentSlotInterval = _configuration.GetValue<int>("AppSettings:AppointmentSlotInterval");
        }

        // POST: api/provider/CreateProvider
        // Create a new provider
        [HttpPost]
        [Route("CreateProvider")]
        public ResponseDto CreateProvider(string providerName, string timeZoneId)
        {
            try
            {
                // Check if timezoneid is valid
                TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                // Create a new Provider object
                Provider obj = new Provider(providerName, timeZoneId);

                // Add the new provider to the database
                _db.Providers.Add(obj);
                _db.SaveChanges();

                // Map the Provider object to ProviderDto and set it as the response result
                _response.Result = _mapper.Map<ProviderDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // GET: api/provider/GetProvidersByName
        // Get providers by name
        [HttpGet]
        [Route("GetProvidersByName/{providerName}")]
        public ResponseDto GetProvidersByName(string providerName)
        {
            try
            {
                // Get the providers from the database by name
                IEnumerable<Provider> objList =
                    _db.Providers
                    .Where(a => a.Name == providerName).ToList();

                // Map the Providers objList to IEnumerable<ProviderDto> and set it as the response result
                _response.Result = _mapper.Map<IEnumerable<ProviderDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // POST: api/provider/SubmitAvailableTime
        // Submit available time for the provider and create appointment slots within that time at an interval of _appointmentSlotInterval minutes
        [HttpPost]
        [Route("SubmitAvailableTime")]
        public ResponseDto SubmitAvailableTime(Guid providerUid, DateTime startDateTimeLocal, DateTime endDateTimeLocal)
        {
            try
            {
                // Initiate the return object
                var objList = new List<AppointmentSlotDto>();

                // Get the provider from the database in order to get the timezone of the provider
                Provider provider = _db.Providers
                   .Where(a => a.ProviderUid == providerUid).First();

                // Convert the providers local time range into UTC time
                var startDateTimeUTC = TimeZoneInfo.ConvertTimeToUtc(startDateTimeLocal, TimeZoneInfo.FindSystemTimeZoneById(provider.TimeZoneId));
                var endDateTimeUTC = TimeZoneInfo.ConvertTimeToUtc(endDateTimeLocal, TimeZoneInfo.FindSystemTimeZoneById(provider.TimeZoneId));

                // Check if there are any conflicting AppointmentSlots already created for this provider
                bool hasConflicts = _db.AppointmentSlots
                    .Any(a => a.ProviderUid == provider.ProviderUid &&
                    a.StartDateTimeUTC < endDateTimeUTC &&
                    a.EndDateTimeUTC > startDateTimeUTC);

                // If there are conflicts return an error
                if (hasConflicts)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Available times entered: conflict with existing Appointment Slots";
                    return _response;
                }

                // Loop through the range of time and build a list of start time for each appointment slot to be created
                var startTimeList = BuildStartTimeList(startDateTimeUTC, endDateTimeUTC);

                foreach(DateTime startTime in startTimeList)
                {
                    // Create a new AppointmentSlot object and add it to the dbcontext
                    AppointmentSlot obj = new AppointmentSlot(provider.ProviderUid, startTime);
                    obj.EndDateTimeUTC = startTime.AddMinutes(_appointmentSlotInterval);
                    _db.AppointmentSlots.Add(obj);

                    // Convert the times of each appointmentslot created back to the provider's local time zone
                    var appointmentSlotDto = _mapper.Map<AppointmentSlotDto>(obj);
                    appointmentSlotDto.StartDateTimeUTC = TimeZoneInfo.ConvertTimeFromUtc(appointmentSlotDto.StartDateTimeUTC, TimeZoneInfo.FindSystemTimeZoneById(appointmentSlotDto.Provider.TimeZoneId));
                    appointmentSlotDto.EndDateTimeUTC = TimeZoneInfo.ConvertTimeFromUtc(appointmentSlotDto.EndDateTimeUTC, TimeZoneInfo.FindSystemTimeZoneById(appointmentSlotDto.Provider.TimeZoneId));
                    objList.Add(appointmentSlotDto);
                }

                // Save the changes to the DB and return the slots that were created
                _db.SaveChanges();
                _response.Result = objList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // GET: api/provider/GetAppointmentSlots
        // Get all the appointment slots for the provider
        [HttpGet]
        [Route("GetAppointmentSlots")]
        public ResponseDto GetAppointmentSlots(Guid providerUid)
        {
            try
            {
                // Get the appointment slots from the db
                IEnumerable<AppointmentSlot> appointmentSlots = _db.AppointmentSlots
                    .Include(x => x.Provider)
                    .Where(x => x.ProviderUid == providerUid)
                    .ToList();

                // Map to IEnumerable<AppointmentSlotDto>
                var objList = _mapper.Map<IEnumerable<AppointmentSlotDto>>(appointmentSlots);

                // Convert each of the appointmentSlotDtos start and end time to the provider's local time before returning it back
                foreach (var appointmentSlotDto in objList)
                {
                    appointmentSlotDto.StartDateTimeUTC = TimeZoneInfo.ConvertTimeFromUtc(appointmentSlotDto.StartDateTimeUTC, TimeZoneInfo.FindSystemTimeZoneById(appointmentSlotDto.Provider.TimeZoneId));
                    appointmentSlotDto.EndDateTimeUTC = TimeZoneInfo.ConvertTimeFromUtc(appointmentSlotDto.EndDateTimeUTC, TimeZoneInfo.FindSystemTimeZoneById(appointmentSlotDto.Provider.TimeZoneId));
                }
                // Return the list
                _response.Result = objList.OrderBy(a => a.StartDateTimeUTC);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // Private method used by SubmitAvailableTime method
        private IEnumerable<DateTime> BuildStartTimeList(DateTime startDateTimeUTC, DateTime endDateTimeUTC)
        {
            var startTimeList = new List<DateTime>();
            int intervalMinutes = _appointmentSlotInterval;

            DateTime current = startDateTimeUTC;
            while (current.AddMinutes(intervalMinutes) <= endDateTimeUTC)
            {
                startTimeList.Add(current);
                current = current.AddMinutes(intervalMinutes);
            }
            return startTimeList;
        }
    }
}
