using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EverlandApi.Core.Services
{
    public interface IAuthenticationService<TResource>
    {
        Task<TResource> AuthenticateAsync(HttpContext httpContext);
    }
}
