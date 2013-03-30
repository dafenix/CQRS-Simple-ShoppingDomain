using System;
using System.Collections.Generic;

namespace CQRS.Infrastructure
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();

        public abstract Guid Id { get; }
        public abstract Snapshot Snapshot { get; }
        public long Version { get; internal set; }

        public long UncommitedChangesCount
        {
            get { return _changes.Count; }
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history) ApplyChange(e, false);
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);
            if (isNew) _changes.Add(@event);
            Version++;
        }
    }
}