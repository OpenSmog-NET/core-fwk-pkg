namespace OS.Core
{
    public class ApiError
    {
        public ApiErrorType Type { get; set; }
        public string Message { get; set; }
    }

    public enum ApiErrorType
    {
        Validation = 0,
        Operation
    }
}