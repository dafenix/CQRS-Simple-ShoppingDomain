using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.ReadModel.Reporting.CommandBuilder;

namespace CQRS.ReadModel.Reporting
{
    public class SqlDeleteBuilder : ISqlDeleteBuilder
    {
        public string CreateSqlDeleteStatementFromDto<TDto>()
        {
            return string.Format("{0};", SqlDeleteBuilder.GetDeleteString<TDto>());
        }

        public string CreateSqlDeleteStatementFromDto<TDto>(IEnumerable<KeyValuePair<string, object>> example)
            where TDto : class
        {
            if (example == null)
            {
                return string.Format("{0};", SqlDeleteBuilder.GetDeleteString<TDto>());
            }
            return string.Format("{0} {1};", SqlDeleteBuilder.GetDeleteString<TDto>(),
                                 SqlDeleteBuilder.GetWhereString(example));
        }

        private static string GetDeleteString<TDto>()
        {
            Type type = typeof (TDto);
            string tableName = type.Name;
            return string.Format("DELETE FROM {0}", tableName);
        }

        private static string GetWhereString(IEnumerable<KeyValuePair<string, object>> example)
        {
            if (example.Count<KeyValuePair<string, object>>() <= 0)
            {
                return string.Empty;
            }
            return string.Format("WHERE {0}", string.Join(" AND ", (
                                                                       from x in example
                                                                       select
                                                                           string.Format("{0} = @{1}", x.Key,
                                                                                         x.Key.ToLower()))
                                                                       .ToArray<string>()));
        }
    }
}