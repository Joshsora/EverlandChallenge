namespace EverlandApi.Core.Models
{
    public enum AuthenticationErrorCode
    {
        MissingHeader,
        InvalidHeader,
        UnknownAuthenticationMethod,
        NoUsableMethodFound,
        UnhandledMethod,
        MalformedCredentials,
        InvalidCredentials
    }
}
