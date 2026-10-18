using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.Features.Projects
{
    public record ProjectStatsDto(
        int Todo,
        int InProgress,
        int Done,
        int Total
    );
}
