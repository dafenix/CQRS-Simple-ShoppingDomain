using System;

namespace CQRS.Infrastructure
{
    public interface ISnapshotStore
    {
        void SaveSnapshot(Snapshot snapshot);

        Snapshot GetSnapshot(Guid id);
    }
}