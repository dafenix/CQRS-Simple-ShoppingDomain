using System;

namespace CQRS.Infrastructure
{
    internal static class Queries
    {
        public const String InsertNewEventQuery =
            //"INSERT INTO [Events]([Id], [EventSourceId], [Name], [Data], [Sequence], [TimeStamp]) VALUES (@EventId, @EventSourceId, @Name, @Data, @Sequence, @TimeStamp)";
            "INSERT INTO Events(Id, EventSourceId, Name, Data, Sequence, TimeStamp) VALUES (@EventId, @EventSourceId, @Name, @Data, @Sequence, @TimeStamp)";

        public const String SelectAllEventsQuery =
            //"SELECT [Data], [Name] FROM [Events] WHERE [EventSourceId] = @EventSourceId AND [Sequence] > @EventSourceVersion ORDER BY [Sequence]";
            "SELECT Data, Name FROM Events WHERE EventSourceId = @EventSourceId AND Sequence > @EventSourceVersion ORDER BY Sequence";

        public const String SelectSequenceQuery =
            //"SELECT ISNULL(MAX([Sequence]), 0) FROM [Events] WHERE [EventSourceId] = @id";
            "SELECT CASE WHEN ISNULL(Max(`Sequence`)) then 0 else Max(`Sequence`) end FROM EVENTS WHERE EventSourceId = @id";

        public const String InsertSnapshot =
            "DELETE FROM [Snapshots] WHERE [EventSourceId]=@EventSourceId; INSERT INTO [Snapshots]([EventSourceId], [Timestamp], [Type], [EventSourceSequence], [Data]) VALUES (@EventSourceId, GETDATE(), @Type, @Sequence, @Data)";

        public const String SelectLatestSnapshot =
            //"SELECT TOP 1 [Type], [Data] FROM [Snapshots] WHERE [EventSourceId]=@EventSourceId ORDER BY [EventSourceSequence] DESC";
            "SELECT Type, Data FROM Snapshots  WHERE EventSourceId=@EventSourceId ORDER BY EventSourceSequence DESC limit 1";
    }
}