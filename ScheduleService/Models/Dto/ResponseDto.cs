namespace ScheduleService.Models.Dto
{
    // This is used as the base return object for all responses
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
