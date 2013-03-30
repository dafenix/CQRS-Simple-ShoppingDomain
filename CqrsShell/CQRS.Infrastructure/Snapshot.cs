using System;

namespace CQRS.Infrastructure
{
    [Serializable]
    public class Snapshot
    {
        public Guid EventSourceId { get; set; }

        public long EventSourceVersion { get; set; }
    }
}