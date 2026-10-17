using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Validators
{
    public class ChangeTaskStatusRequestValidator : AbstractValidator<ChangeTaskStatusRequest>
    {
        public ChangeTaskStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status =>
                {
                    return Enum.TryParse<TaskItemStatus>(status, ignoreCase: true, out var parsedStatus)
                           && Enum.IsDefined(typeof(TaskItemStatus), parsedStatus);
                })
                .WithMessage($"Invalid status value. Allowed values are: {string.Join(", ", Enum.GetNames(typeof(TaskItemStatus)))}");
        }
    }
}
