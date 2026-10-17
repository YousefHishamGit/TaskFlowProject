using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Validators
{
    public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
    {
        public UpdateTaskRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 200).WithMessage("Title must be between 3 and 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

            RuleFor(x => x.Priority)
                .InclusiveBetween(TaskPriority.MinPriority, TaskPriority.MaxPriority)
                .WithMessage($"Priority must be between {TaskPriority.MinPriority} and {TaskPriority.MaxPriority}.");
        }
    }
}
