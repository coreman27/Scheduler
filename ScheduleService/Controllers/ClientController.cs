using Microsoft.AspNetCore.Mvc;
using ScheduleService.Models.Dto;
using ScheduleService.Models;
using AutoMapper;
using ScheduleService.Data;
using Microsoft.EntityFrameworkCore;

namespace ScheduleService.Controllers
{
    [Route("api/client")]
    [ApiController]
    public class ClientController
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public ClientController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        // POST: api/client/CreateClient
        // Create a new client
        [HttpPost]
        [Route("CreateClient")]
        public ResponseDto CreateClient(string clientName, string timeZoneId)
        {
            try
            {
                // Check if timezoneid is valid
                TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                // Create a new client object
                Client obj = new Client(clientName, timeZoneId);

                // Add it to the database
                _db.Clients.Add(obj);
                _db.SaveChanges();

                // Map to a ClientDto object and set as the response result
                _response.Result = _mapper.Map<ClientDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // GET: api/client/GetClientsByName
        // Get clients by name
        [HttpGet]
        [Route("GetClientsByName/{clientName}")]
        public ResponseDto GetClientByName(string clientName)
        {
            try
            {
                // Get clients from the database by name
                IEnumerable<Client> objList =
                _db.Clients
                    .Where(a => a.Name == clientName).ToList();

                // Map to IEnumerable<ClientDto> and set as the response result
                _response.Result = _mapper.Map<IEnumerable<ClientDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // GET: api/client/GetAppointmentSlots
        // Get appointment slots
        [HttpGet]
        [Route("GetAppointmentSlots/{clientUid:guid}")]
        public ResponseDto GetAppointmentSlots(Guid clientUid)
        {
            try
            {
                // Calculate the earliest appointment they can book. No earlier than 24 hours from now.
                var nextDayUTC = DateTime.UtcNow.AddDays(1);

                // Get the client from the database to get the timezone of the client
                var client = _db.Clients.Where(c => c.ClientUid == clientUid).First();

                // Get appointment slots that are at least 24 hours away and that dont have any existing appointments assigned to them already
                IEnumerable<AppointmentSlot> objList = _db.AppointmentSlots
                    .Include(x => x.Provider)
                    .Where(slot => slot.StartDateTimeUTC >= nextDayUTC && !_db.Appointments.Any(appointment => appointment.AppointmentSlotUid == slot.AppointmentSlotUid))
                    .ToList();

                // Map the appointmentSlots to IEnumerable<AppointmentSlotDto>
                var appointmentSlotDtoList = _mapper.Map<IEnumerable<AppointmentSlotDto>>(objList);

                // Build ClientAppointmentSlotDto that has only information relevant to the client
                var clientAppointmentSlotDtoList = new List<ClientAppointmentSlotDto>();

                foreach (var appointmentSlotDto in appointmentSlotDtoList)
                {
                    // Convert each appointment slot to the clients local time zone
                    clientAppointmentSlotDtoList.Add(BuildClientAppointmentSlotDto(appointmentSlotDto, client.TimeZoneId));
                }

                // Send the client an ordered list of available appointment slots
                _response.Result = clientAppointmentSlotDtoList.OrderBy(x => x.StartDateTime);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // POST: api/client/CreateAppointment
        // Create appointment for client
        [HttpPost]
        [Route("CreateAppointment")]
        public ResponseDto CreateAppointment(Guid clientUid, Guid appointmentSlotUid)
        {
            try
            {
                // Set the time the booking was requested
                DateTime bookedDateTimeUTC = DateTime.UtcNow;

                // See if the client already has an appointment booked. Return an error message if they do
                var existingAppointment = _db.Appointments.Any(a => a.ClientUid == clientUid);
                if (existingAppointment)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Client already has an appointment booked. Only one booking at a time.";
                    return _response;
                }

                // See if the client already has an appointment booked. Return an error message if they do
                var slotAlreadyBooked = _db.Appointments.Any(a => a.AppointmentSlotUid == appointmentSlotUid);
                if (slotAlreadyBooked)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Slot has already been booked";
                    return _response;
                }

                // Build a new appointment and add it to the database
                var appointment = new Appointment(appointmentSlotUid, clientUid, bookedDateTimeUTC);
                _db.Appointments.Add(appointment);
                _db.SaveChanges();

                // Get the appointments for the appointment that was just created with all of the relevant information.
                var bookedAppointment = GetAppointments()
                    .Where(a => a.AppointmentUID == appointment.AppointmentUID).First();

                // Map to ClientAppointmentSlotDto and return to the client
                var appointmentSlotDto = _mapper.Map<AppointmentSlotDto>(bookedAppointment.AppointmentSlot);
                _response.Result = BuildClientAppointmentSlotDto(appointmentSlotDto, bookedAppointment.Client.TimeZoneId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // GET: api/client/GetClientAppointment
        // Get appointment for client
        [HttpGet]
        [Route("GetClientAppointment/{clientUid:guid}")]
        public ResponseDto GetClientAppointment(Guid clientUid)
        {
            try
            {
                // Get appointments for the client
                var appointment = GetAppointments()
                    .FirstOrDefault(u => u.ClientUid == clientUid);

                // Return an error if no appointments exist
                if (appointment == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No appointments exist for this client.";
                    return _response;
                }

                // Map to ClientAppointmentSlotDto and return to the client
                var appointmentSlotDto = _mapper.Map<AppointmentSlotDto>(appointment.AppointmentSlot);
                _response.Result = BuildClientAppointmentSlotDto(appointmentSlotDto, appointment.Client.TimeZoneId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // PUT: api/client/ConfirmAppointment
        // Confirm the appointment for the client
        [HttpPut]
        [Route("ConfirmAppointment/{appointmentSlotUid:guid}")]
        public ResponseDto ConfirmAppointment(Guid appointmentSlotUid)
        {
            try
            {
                // Get the client appointment
                var appointment = GetAppointments()
                   .Where(a => a.AppointmentSlotUid == appointmentSlotUid).FirstOrDefault();

                // If no appointment is found return an error
                if (appointment == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No appointments exist for this client.";
                    return _response;
                }

                // Set the appointment to confirmed and Update the row in the database
                appointment.Confirmed = true;
                _db.Appointments.Update(appointment);
                _db.SaveChanges();

                // Map to ClientAppointmentSlotDto and return to the client
                var appointmentSlotDto = _mapper.Map<AppointmentSlotDto>(appointment.AppointmentSlot);
                _response.Result = BuildClientAppointmentSlotDto(appointmentSlotDto, appointment.Client.TimeZoneId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        //Private method to Get Appointments and reduce code redundency
        private IEnumerable<Appointment> GetAppointments()
        {
            return _db.Appointments
                    .Include(x => x.AppointmentSlot)
                    .Include(x => x.AppointmentSlot.Provider)
                    .Include(x => x.Client);
        }

        //Private method to map AppointmentSlotDto to a ClientAppointmentSlotDto and to reduce code redundency
        private ClientAppointmentSlotDto BuildClientAppointmentSlotDto(AppointmentSlotDto appointmentSlotDto, string timeZoneId)
        {
            var clientStartDateTime = TimeZoneInfo.ConvertTimeFromUtc(appointmentSlotDto.StartDateTimeUTC, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
            var clientEndDateTime = TimeZoneInfo.ConvertTimeFromUtc(appointmentSlotDto.EndDateTimeUTC, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));

            var obj = new ClientAppointmentSlotDto(appointmentSlotDto.AppointmentSlotUid, appointmentSlotDto.Provider.Name, clientStartDateTime, clientEndDateTime);
            return obj;
        }
    }
}
