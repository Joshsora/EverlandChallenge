using System.Collections.Generic;
using System.Linq;

namespace EverlandApi.Core.Models
{
    public class ApiResponse
    {
        public static ApiResponse Empty => new ApiResponse();

        private List<ApiError> _errors;

        public bool Success => _errors.Count == 0;
        public ApiError[] Errors => _errors.ToArray();

        public ApiResponse()
        {
            _errors = new List<ApiError>();
        }

        public ApiResponse(IEnumerable<ApiError> errors)
        {
            _errors = errors.ToList();
        }

        public ApiResponse(params ApiError[] errors)
        {
            _errors = errors.ToList();
        }

        public void PushError(ApiError error)
        {
            _errors.Add(error);
        }

        public void PushErrors(IEnumerable<ApiError> errors)
        {
            _errors.AddRange(errors);
        }

        public void PushErrors(params ApiError[] errors)
        {
            _errors.AddRange(errors);
        }
    }

    public class ApiResponse<TData> : ApiResponse
    {
        public TData Data { get; set; }

        public ApiResponse(TData data)
        {
            Data = data;
        }
    }
}
