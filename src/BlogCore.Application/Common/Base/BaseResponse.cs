namespace BlogCore.Application.Common.Base
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        // This property name must match what you use in GlobalExceptionHandler
        public Dictionary<string, string[]>? Errors { get; set; }

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
            Message = message ?? string.Empty;
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

        public static BaseResponse<T> FailureResponse(string message, Dictionary<string, string[]>? errors = null)
        {
            return new BaseResponse<T>(message, false)
            {
                Errors = errors ?? new Dictionary<string, string[]>()
            };
        }

        public static BaseResponse<T> FailureResponse(string message, string error)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "general", new[] { error } }
            };

            return new BaseResponse<T>(message, false)
            {
                Errors = errors
            };
        }
    }
    //public class BaseResponse<T>
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; } = string.Empty;
    //    public T? Data { get; set; }
    //    public List<string> Errors { get; set; } = new();

    //    public BaseResponse()
    //    {
    //        Success = true;
    //    }

    //    public BaseResponse(string message)
    //    {
    //        Success = true;
    //        Message = string.Empty;
    //    }

    //    public BaseResponse(T data, string? message = null)
    //    {
    //        Success = true;
    //        Data = data;
    //        Message = string.Empty;
    //    }

    //    public BaseResponse(string message, bool success)
    //    {
    //        Success = success;
    //        Message = string.Empty;
    //    }

    //    public static BaseResponse<T> SuccessResponse(T data, string? message = null)
    //    {
    //        return new BaseResponse<T>(data, message);
    //    }

    //    public static BaseResponse<T> FailureResponse(string message, List<string>? errors = null)
    //    {
    //        return new BaseResponse<T>(message ?? "An error occurred", false)
    //        {
    //            Errors = errors ?? new List<string>()
    //        };
    //    }
    //}
}
