namespace EverlandApi.Core.Services
{
    public class RedisOptions
    {
        public string ConnectionString { get; set; }

        public RedisOptions()
        {
            ConnectionString = "localhost";
        }
    }
}
