using System;

namespace CQRS.Infrastructure
{
    [Serializable]
    public class AggregateNotFoundException : Exception
    {
    }
}