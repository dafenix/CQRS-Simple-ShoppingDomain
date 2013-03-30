using System;
using System.Globalization;

namespace CQRS.Infrastructure
{
    [Serializable]
    public class ConcurrencyException : Exception
    {
        private const string ErrorMessageFormat =
            "concurrency exception occured while trying to save changes for the aggregate with id {0}! Excepted version {1} but last store version is {2}";

        public ConcurrencyException(Guid aggregateId, long expectedVersion, long lastVersion)
            : base(
                string.Format(CultureInfo.InvariantCulture, ErrorMessageFormat, aggregateId, expectedVersion,
                              lastVersion))
        {
        }
    }
}