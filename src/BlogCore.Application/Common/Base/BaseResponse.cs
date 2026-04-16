namespace BlogCore.Application.Common.Base
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public BaseResponse()
        {
            Success = true;
        }

        public BaseResponse(string message)
        {
            Success = true;
            Message = message;
        }

        public BaseResponse(T data, string? message = null)
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public BaseResponse(string message, bool success)
        {
            Success = success;
            Message = message;
        }

        public static BaseResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new BaseResponse<T>(data, message);
        }

        public static BaseResponse<T> FailureResponse(string message, List<string>? errors = null)
        {
            return new BaseResponse<T>(message, false)
            {
                Errors = errors ?? new List<string>()
            };
        }
    }
}
