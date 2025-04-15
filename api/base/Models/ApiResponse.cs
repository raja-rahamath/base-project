namespace api.Models
{
    /// <summary>
    /// Generic API response model for consistent response format
    /// </summary>
    /// <typeparam name="T">The type of data contained in the response</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Whether the request was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Message providing additional information about the response
        /// </summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Data returned by the API (if applicable)
        /// </summary>
        public T? Data { get; set; }
        
        /// <summary>
        /// Error details (if applicable)
        /// </summary>
        public string? Error { get; set; }
        
        /// <summary>
        /// Create a successful response with data
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }
        
        /// <summary>
        /// Create an error response
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string error, string message = "An error occurred")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Error = error
            };
        }
    }
}