namespace SmartPulseTask.Web.Models;
public class ApiResponseViewModel<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();

    public static ApiResponseViewModel<T> SuccessResult(T data)
    {
        return new ApiResponseViewModel<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponseViewModel<T> ErrorResult(string errorMessage)
    {
        return new ApiResponseViewModel<T>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    public static ApiResponseViewModel<T> ValidationErrorResult(List<string> validationErrors)
    {
        return new ApiResponseViewModel<T>
        {
            Success = false,
            ValidationErrors = validationErrors
        };
    }
}
