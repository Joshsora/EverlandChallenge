using EverlandApi.Core.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EverlandApi.Core.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                context.Result = new ValidationFailedResult(context.ModelState);
        }
    }
}
