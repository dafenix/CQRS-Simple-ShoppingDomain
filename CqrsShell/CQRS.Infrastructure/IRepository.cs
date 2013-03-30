using System;

namespace CQRS.Infrastructure
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Save(AggregateRoot aggregate);
        T GetById(Guid id);
    }
}