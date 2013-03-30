using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CQRS.Infrastructure
{
    public class MsSqlStore : IEventStore, ISnapshotStore
    {
        private readonly IEventPublisher _publisher;
        private readonly string _connectionString;

        public MsSqlStore(string connectionString, IEventPublisher publisher)
        {
            _connectionString = connectionString;
            _publisher = publisher;
        }

        #region Implementation of IEventStore

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, long expectedVersion)
        {
            if (null == events)
            {
                throw new ArgumentNullException("events");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                long lastVersion;

                using (var getLastVersionQuery = new SqlCommand(Queries.SelectSequenceQuery, connection))
                {
                    getLastVersionQuery.Parameters.AddWithValue("@id", aggregateId);
                    lastVersion = Convert.ToInt64(getLastVersionQuery.ExecuteScalar());
                    if (lastVersion > expectedVersion)
                    {
                        // TODO: "was intelligentes Programmieren", statt Exception werfen ;)
                        throw new ConcurrencyException(aggregateId, expectedVersion, lastVersion);
                    }
                }

                using (var insertEventsCommand = new SqlCommand(Queries.InsertNewEventQuery, connection))
                {
                    insertEventsCommand.Parameters.Add("@EventId", SqlDbType.UniqueIdentifier);
                    insertEventsCommand.Parameters.Add("@EventSourceId", SqlDbType.UniqueIdentifier);
                    insertEventsCommand.Parameters.Add("@Name", SqlDbType.NVarChar, -1);
                    insertEventsCommand.Parameters.Add("@Data", SqlDbType.NVarChar, -1);
                    insertEventsCommand.Parameters.Add("@Sequence", SqlDbType.BigInt);
                    insertEventsCommand.Parameters.Add("@TimeStamp", SqlDbType.DateTime);
                    insertEventsCommand.Prepare();
                    foreach (var @event in events)
                    {
                        @event.Version = ++lastVersion;
                        insertEventsCommand.Parameters["@EventId"].Value = Guid.NewGuid();
                        insertEventsCommand.Parameters["@EventSourceId"].Value = aggregateId;
                        insertEventsCommand.Parameters["@Name"].Value = @event.GetType().AssemblyQualifiedName;
                        insertEventsCommand.Parameters["@Data"].Value = Serialization.Serialize(@event);
                        insertEventsCommand.Parameters["@Sequence"].Value = @event.Version;
                        insertEventsCommand.Parameters["@TimeStamp"].Value = DateTime.UtcNow;
                        insertEventsCommand.ExecuteNonQuery();
                        _publisher.Publish(@event);
                    }
                }
            }
        }

        public IEnumerable<Event> GetEventsForAggregate(Guid aggregateId)
        {
            return GetEventsSinceVersion(aggregateId, 0);
        }

        public IEnumerable<Event> GetEventsSinceVersion(Guid aggregateId, long version)
        {
            var events = new List<Event>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var eventsQuery = new SqlCommand(Queries.SelectAllEventsQuery, connection))
                {
                    eventsQuery.Parameters.AddWithValue("@EventSourceId", aggregateId);
                    eventsQuery.Parameters.AddWithValue("@EventSourceVersion", version);
                    using (var reader = eventsQuery.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var typeName = reader["Name"].ToString();
                            var type = Type.GetType(typeName);
                            var @event = (Event) Serialization.Deserialize(type, reader["Data"].ToString());
                            events.Add(@event);
                        }
                    }
                }
            }

            return events;
        }

        #endregion

        #region Implementation of ISnapshotStore

        public void SaveSnapshot(Snapshot snapshot)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var saveCommand = new SqlCommand(Queries.InsertSnapshot, connection))
                {
                    saveCommand.Parameters.AddWithValue("@EventSourceId", snapshot.EventSourceId);
                    saveCommand.Parameters.AddWithValue("@Type", snapshot.GetType().AssemblyQualifiedName);
                    saveCommand.Parameters.AddWithValue("@Sequence", snapshot.EventSourceVersion);
                    saveCommand.Parameters.AddWithValue("@Data", Serialization.SerializeSnaptshot(snapshot));
                    saveCommand.ExecuteNonQuery();
                }
            }
        }

        public Snapshot GetSnapshot(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSnapshotQuery = new SqlCommand(Queries.SelectLatestSnapshot, connection))
                {
                    getSnapshotQuery.Parameters.AddWithValue("@EventSourceId", id);
                    using (var reader = getSnapshotQuery.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var typeName = reader["Type"].ToString();
                            var snapshotData = reader["Data"];
                            if (null == snapshotData || DBNull.Value == snapshotData) return null;
                            return Serialization.DeserializeSnapshot((byte[]) snapshotData);
                        }
                    }
                }
            }

            return null;
        }

        #endregion
    }
}