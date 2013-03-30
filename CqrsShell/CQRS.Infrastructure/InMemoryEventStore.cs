using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRS.Infrastructure
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly IEventPublisher _publisher;

        private struct EventDescriptor
        {
            public readonly Event EventData;
            public readonly Guid Id;
            public readonly long Version;

            public EventDescriptor(Guid id, Event eventData, long version)
            {
                EventData = eventData;
                Version = version;
                Id = id;
            }
        }

        public InMemoryEventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        private readonly Dictionary<Guid, List<EventDescriptor>> _current =
            new Dictionary<Guid, List<EventDescriptor>>();

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, long expectedVersion)
        {
            List<EventDescriptor> eventDescriptors;
            if (!_current.TryGetValue(aggregateId, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                _current.Add(aggregateId, eventDescriptors);
            }
            else if (eventDescriptors[eventDescriptors.Count - 1].Version != expectedVersion && expectedVersion != -1)
            {
                throw new ConcurrencyException(aggregateId, expectedVersion,
                                               eventDescriptors[eventDescriptors.Count - 1].Version);
            }
            var i = expectedVersion;
            foreach (var @event in events)
            {
                i++;
                @event.Version = i;
                eventDescriptors.Add(new EventDescriptor(aggregateId, @event, i));
                _publisher.Publish(@event);
            }
        }

        public IEnumerable<Event> GetEventsForAggregate(Guid aggregateId)
        {
            List<EventDescriptor> eventDescriptors;
            if (!_current.TryGetValue(aggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }
            return eventDescriptors.Select(desc => desc.EventData).ToList();
        }

        public IEnumerable<Event> GetEventsSinceVersion(Guid aggregateId, long version)
        {
            return GetEventsForAggregate(aggregateId).Where(e => e.Version > version);
        }
    }
}