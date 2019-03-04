using System.ComponentModel.DataAnnotations;

namespace EverlandApi.Core.Models
{
    public class ApiError
    {
        [Required]
        public string Reason { get; private set; }

        [Required]
        public ApiErrorCode ErrorCode { get; private set; }

        public ApiError(string reason, ApiErrorCode errorCode)
        {
            Reason = reason;
            ErrorCode = errorCode;
        }
    }
}
