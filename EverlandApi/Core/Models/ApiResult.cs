using System.Collections.Generic;
using System.Linq;

namespace EverlandApi.Core.Models
{
    public class ApiResult
    {
        public bool Success
        {
            get { return Errors.Count == 0; }
        }
        public ICollection<ApiError> Errors { get; private set; }

        public ApiResult()
        {
            Errors = new List<ApiError>();
        }

        public ApiResult(ICollection<ApiError> errors)
        {
            Errors = errors.ToList();
        }

        public ApiResult(params ApiError[] errors)
        {
            Errors = errors.ToList();
        }
    }

    public class ApiResult<TData> : ApiResult
    {
        public TData Data { get; private set; }

        public ApiResult(TData data)
        {
            Data = data;
        }
    }
}
