using Microsoft.AspNetCore.Mvc.Filters;

namespace NetTemplate.Filters
{
    public class ParameterFilterAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}
