using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.Common
{
    public static class CacheKeys
    {
        public static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

        
        public static string GetProjectStatsKey(Guid projectId) => $"project:{projectId}:stats";
    }
}
