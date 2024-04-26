# Schedule Service

The Schedule Service project is a .NET Core Web API application that provides APIs for managing schedules and appointments.

# Prerequisites
  - Visual Studio
  - Mirosoft SQL Server

## Features

- **Providers and Clients**: Allows providers to set their availability and clients to book appointments.
- **Conflict Management**: Doesnt allow the same provider to submit conflicting appointment slots. Doesnt allow clients to book the same appointment slot.
- **Background Job**: Deletes appointments every 1 minute that have not been confirmed and are passed the booking confirmation window
- **Timezone Handling**: Supports providers and clients in different timezones. All times are stored in UTC in the database

## Configuration
- Open appsettings.json and check the "DefaultConnection", for the SQL database connection string, is set correctly to connect to your SQL instance and set the database name you would like to use.
- Set the preferred "ConfirmationWindowLength" you would like to use for testing. The default value is 30 minutes. you can set this to something >= 1 if you want to see the job work faster.
- Upon running the project the EnitityFramework migration will run and create a database using the connection string provided. It will also prepopulate the database with a client in EST and a provider in CST.

## Endpoints 

### Client API
(all times returned in client local time)
- CreateClient: pass a clientName and timezoneId. (timezoneId must be a valid timezone such as "Central Standard Time" or "Eastern Standard Time". You can also use "CST" or "EST". This will return back a clientObject with a clientUid.
- GetClientsByName: pass the clientName to search for an existing client to attain the clientUid.
- GetAppointmentSlots: pass the clientUid. This will use your client's timezone to return back to you available appointment slots and the provider for each slot.
- CreateAppointment: pass the clientUid and an AppointmentSlotUid returned back to you in the GetAppointmentSlots api call.
- GetClientAppointment: pass the clientUid. This will return an appointment slot that is currently booked for the client.
- ConfirmAppointment: pass the appointmentSlotUid of an appointment you have booked to confirm it within the confirmation window.

### Provider API
(all times returned in provider local time)
- CreateProvider: pass a providerName and timezoneId.
- GetProviderByName: pass the providerName and get back the provider object with the providerUid
- SetAvailableTime: pass the providerUid, startDateTimeLocal, and EndDateTimeLocal. This will create and return back to you a list of appointment Slots that were created in 15 minute intervals in the time frame you entered. This will return an error if you try to submit available times for which you already have an appointment slot. 
- GetAppointmentSlots: pass the providerUid. this will return to you the list of AppointmentSlots you currently have for scheduling. 
## Testing

Use the Swagger UI to test the endpoints
![image](https://github.com/coreman27/Scheduler/assets/10369006/589d85ee-d7fd-4e65-a2f9-83ad72b06f53)

- Use existing pre-loaded client and provider. (You can also create your own!)
- api/provider/SubmitAvailableTime with the provider of your choosing.
- Use api/client/GetAppointmentSlots/{clientUid} to see a list of available slots you can choose from. Only slots that are at least 24 hours away will be returned.
- CreateAppointment with the clientUid and the AppointmentSlotUid of your choosing from the list of appointment slots returned.
- Confirm the appointment by calling api/client/confirmAppointment/{appointmentslotuid}.
- Check GetAppointmentSlots again and see the appointments that are booked do not show as available anymore.
- Book another appointment and don't confirm it.
- Wait the time you set for "ConfirmationWindowLength" and see the appointment get deleted. (Set this to 1 if you dont want to wait)
- Break the program
