using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.Features.Projects
{
    public record CreateProjectRequest(string Name, string Description);
}
