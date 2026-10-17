using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskFlow.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var failures = new List<ValidationFailure>();

            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var argumentType = argument.GetType(); 
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

          
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                
                    var validationContext = new ValidationContext<object>(argument);
                    var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                    if (!validationResult.IsValid)
                    {
                        failures.AddRange(validationResult.Errors);
                    }
                }
            }

           
            if (failures.Count > 0)
            {
            
                throw new ValidationException(failures);
            }

           
            await next();
        }
    }
}