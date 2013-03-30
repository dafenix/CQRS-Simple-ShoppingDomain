namespace CQRS.Infrastructure
{
    public interface ISnapshotable<TSnapshot> where TSnapshot : Snapshot
    {
        void RestoreFromSnapshot(Snapshot snapshot);
    }
}