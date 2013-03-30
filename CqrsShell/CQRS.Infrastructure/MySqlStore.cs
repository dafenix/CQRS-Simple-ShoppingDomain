using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CQRS.Infrastructure
{
    public class MySqlStore : IEventStore, ISnapshotStore
    {
        private readonly string _ConnectionString;
        private readonly IEventPublisher _Publisher;

        public MySqlStore(string connectionString, IEventPublisher publisher)
        {
            _ConnectionString = connectionString;
            _Publisher = publisher;
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, long expectedVersion)
        {
            if (null == events)
            {
                throw new ArgumentNullException("events");
            }

            using (var connection = new MySqlConnection(_ConnectionString))
            {
                connection.Open();
                long lastVersion;

                using (var getLastVersionQuery = new MySqlCommand(Queries.SelectSequenceQuery, connection))
                {
                    getLastVersionQuery.Parameters.AddWithValue("@id", aggregateId);
                    lastVersion = Convert.ToInt64(getLastVersionQuery.ExecuteScalar());
                    if (lastVersion > expectedVersion)
                    {
                        // TODO: "was intelligentes Programmieren", statt Exception werfen ;)
                        throw new ConcurrencyException(aggregateId, expectedVersion, lastVersion);
                    }
                }

                using (var insertEventsCommand = new MySqlCommand(Queries.InsertNewEventQuery, connection))
                {
                    insertEventsCommand.Parameters.Add("@EventId", MySqlDbType.VarChar);
                    insertEventsCommand.Parameters.Add("@EventSourceId", MySqlDbType.Guid);
                    insertEventsCommand.Parameters.Add("@Name", MySqlDbType.Text, -1);
                    insertEventsCommand.Parameters.Add("@Data", MySqlDbType.Text, -1);
                    insertEventsCommand.Parameters.Add("@Sequence", MySqlDbType.Int64);
                    insertEventsCommand.Parameters.Add("@TimeStamp", MySqlDbType.DateTime);
                    insertEventsCommand.Prepare();
                    foreach (var @event in events)
                    {
                        @event.Version = ++lastVersion;
                        insertEventsCommand.Parameters["@EventId"].Value = Guid.NewGuid().ToString();
                        insertEventsCommand.Parameters["@EventSourceId"].Value = aggregateId;
                        insertEventsCommand.Parameters["@Name"].Value = @event.GetType().AssemblyQualifiedName;
                        insertEventsCommand.Parameters["@Data"].Value = Serialization.Serialize(@event);
                        insertEventsCommand.Parameters["@Sequence"].Value = @event.Version;
                        insertEventsCommand.Parameters["@TimeStamp"].Value = DateTime.UtcNow;
                        insertEventsCommand.ExecuteNonQuery();
                        _Publisher.Publish(@event);
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
            using (var connection = new MySqlConnection(_ConnectionString))
            {
                connection.Open();
                using (var eventsQuery = new MySqlCommand(Queries.SelectAllEventsQuery, connection))
                {
                    eventsQuery.Parameters.AddWithValue("@EventSourceId", aggregateId.ToString());
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

        public void SaveSnapshot(Snapshot snapshot)
        {
            using (var connection = new MySqlConnection(_ConnectionString))
            {
                connection.Open();
                using (var saveCommand = new MySqlCommand(Queries.InsertSnapshot, connection))
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
            using (var connection = new MySqlConnection(_ConnectionString))
            {
                connection.Open();
                using (var getSnapshotQuery = new MySqlCommand(Queries.SelectLatestSnapshot, connection))
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
    }
}