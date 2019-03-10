namespace EverlandApi.Core
{
    public class SecurityOptions
    {
        public string SecretKey { get; set; }

        public SecurityOptions()
        {
            SecretKey = "default_secret_key";
        }
    }
}
