namespace EverlandApi.Core.Models
{
    public class ApiResult
    {
        public bool Success
        {
            get { return Error == null; }
        }
        public ApiError Error { get; private set; }

        public ApiResult(ApiError error = null)
        {
            Error = error;
        }
    }

    public class ApiResult<DataT> : ApiResult
    {
        public DataT Data { get; private set; }

        public ApiResult(DataT data)
        {
            Data = data;
        }
    }
}
