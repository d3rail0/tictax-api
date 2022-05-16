namespace tictax.api.Data.Models
{
    public class ErrorResponse
    {
        public int Code { get; set; } = 0;
        public string Message { get; set; } = string.Empty;

        public ErrorResponse(int code, string msg)
        {
            Code = code;
            Message = msg;
        }
    }
}
