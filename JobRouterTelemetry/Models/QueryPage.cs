using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobRouterTelemetry.Models
{
    public record QueryPage<T>(ICollection<T> Data, string ContinuationToken);
}
