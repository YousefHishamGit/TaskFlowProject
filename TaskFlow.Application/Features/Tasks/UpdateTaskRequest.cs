using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.Features.Tasks
{
    public record UpdateTaskRequest(
        string Title,
        string? Description,
        int Priority,
        DateTime? DueDate
    );
}
