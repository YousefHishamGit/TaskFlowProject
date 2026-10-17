using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Common
{
    public static class TaskPriority
    {
        public const int MinPriority = 1;
        public const int MaxPriority = 4;

        public const int Low = 1;
        public const int Medium = 2;
        public const int High = 3;
        public const int Critical = 4;
    }
}
