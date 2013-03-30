namespace CQRS.Infrastructure
{
    public class Event : Message
    {
        public long Version;
    }
}