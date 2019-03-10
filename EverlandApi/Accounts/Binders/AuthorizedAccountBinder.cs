using System.Threading.Tasks;
using EverlandApi.Accounts.Models;
using EverlandApi.Core.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EverlandApi.Accounts.Binders
{
    public class AuthorizedAccountBinder : IModelBinder
    {
        private IAuthenticationService<Account> _authService;

        public AuthorizedAccountBinder(
            IAuthenticationService<Account> authService)
        {
            _authService = authService;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            bindingContext.Result = ModelBindingResult.Success(
                await _authService.AuthenticateAsync(bindingContext.HttpContext)
            );
        }
    }
}
