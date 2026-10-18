using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.Features.Projects
{
    public record ProjectDetailsDto(
         Guid Id,
         string Name,
         string Description,
         Guid OwnerId,
         int MemberCount,
         DateTime CreatedAt,
         DateTime? UpdatedAt,
         ProjectStatsDto Stats
     );
}
