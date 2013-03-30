using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRS.Infrastructure
{
    /// <summary>
    /// Repository für Domain-Objekte (d.h. Objekte, die die "Businesslogik" der App enthalten).
    /// Speichert die Änderungen dieser Objekte und baut deren Zustand wieder anhand von gespeicherten Events wieder auf.
    /// </summary>
    /// <typeparam name="T">Der genaue Typ des Aggregats (Domain-Objekts)</typeparam>
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly ISnapshotStore _snapshotStore;
        private readonly IEventStore _storage;
        // Gibt an bei wievielen Events ein Snapshot erstellt werden soll.
        // Snapshots werden verwendet, um das Laden von Domain-Objekten zu beschleunigen.
        private const long SnapshotIntervalInEvents = 15;

        public Repository(ISnapshotStore snapshotStore, IEventStore storage)
        {
            _snapshotStore = snapshotStore;
            _storage = storage;
        }

        public void Save(AggregateRoot aggregate)
        {
            if (ShouldCreateSnapshot(aggregate))
            {
                SaveSnapshot(aggregate);
            }

            SaveChanges(aggregate, aggregate.Version);
        }

        private bool ShouldCreateSnapshot(AggregateRoot aggregate)
        {
            if (_snapshotStore == null) return false;
            if (aggregate.GetType().GetSnapshotInterfaceType() == null) return false;

            var nextVersionCountSinceSnapshot = (aggregate.Version%SnapshotIntervalInEvents) +
                                                aggregate.UncommitedChangesCount;
            return nextVersionCountSinceSnapshot > SnapshotIntervalInEvents;
        }

        private void SaveSnapshot(AggregateRoot aggregate)
        {
            var snapshot = aggregate.Snapshot;
            if (null != snapshot)
            {
                snapshot.EventSourceId = aggregate.Id;
                snapshot.EventSourceVersion = aggregate.Version;
                _snapshotStore.SaveSnapshot(snapshot);
            }
        }

        private void SaveChanges(AggregateRoot aggregate, long expectedVersion)
        {
            if (null == aggregate) throw new ArgumentNullException("aggregate");
            if (expectedVersion < 0)
                throw new ArgumentException("you don't really expect a negative event source version, do you?");
            if (aggregate.Id.Equals(Guid.Empty))
            {
                throw new ArgumentException(
                    "Illegal aggregateId argument value: the id of an aggregate root must not be empty!");
            }

            _storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
            aggregate.MarkChangesAsCommitted();
        }

        public T GetById(Guid id)
        {
            var aggregateRoot = new T();
            var snapshot = _snapshotStore.GetSnapshot(id);
            IEnumerable<Event> events;
            if (snapshot != null)
            {
                RestoreAggregateFromSnapshot(aggregateRoot, snapshot);
                events = _storage.GetEventsSinceVersion(id, snapshot.EventSourceVersion);
            }
            else
            {
                events = _storage.GetEventsForAggregate(id);
                if (!events.Any())
                    throw new InvalidOperationException("No events found for the aggregate with the id " + id);
            }

            aggregateRoot.LoadsFromHistory(events);
            return aggregateRoot;
        }

        private static void RestoreAggregateFromSnapshot(T aggregateRoot, Snapshot snapshot)
        {
            var memType = typeof (T).GetSnapshotInterfaceType();
            var restoreMethod = memType.GetMethod("RestoreFromSnapshot");
            restoreMethod.Invoke(aggregateRoot, new object[] {snapshot});
            aggregateRoot.Version = snapshot.EventSourceVersion;
        }
    }
}