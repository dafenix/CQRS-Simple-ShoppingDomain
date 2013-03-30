using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CQRS.ReadModel.Reporting.CommandBuilder;

namespace CQRS.ReadModel.Reporting
{
    public class SqlSelectBuilder : ISqlSelectBuilder
    {
        public string CreateSqlSelectStatementFromDto<TDto>()
        {
            return string.Format("{0};", SqlSelectBuilder.GetSelectString<TDto>());
        }

        public string CreateSqlSelectStatementFromDto<TDto>(IEnumerable<KeyValuePair<string, object>> example)
            where TDto : class
        {
            if (example == null)
            {
                return string.Format("{0};", SqlSelectBuilder.GetSelectString<TDto>());
            }
            return string.Format("{0} {1};", SqlSelectBuilder.GetSelectString<TDto>(),
                                 SqlSelectBuilder.GetWhereString(example));
        }

        private static string GetSelectString<TDto>()
        {
            Type type = typeof (TDto);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = type.Name;
            return string.Format("SELECT {0} FROM {1}", string.Join(",", (
                                                                             from x in
                                                                                 properties.Where(
                                                                                     new Func<PropertyInfo, bool>(
                                                                                         SqlSelectBuilder.Where))
                                                                             select SqlSelectBuilder.GetColumn(x.Name))
                                                                             .ToArray<string>()), tableName);
        }

        private static bool Where(PropertyInfo propertyInfo)
        {
            return !propertyInfo.PropertyType.IsGenericType;
        }

        private static string GetColumn(string name)
        {
            return string.Format("{0}", name);
        }

        private static string GetWhereString(IEnumerable<KeyValuePair<string, object>> example)
        {
            if (example.Count<KeyValuePair<string, object>>() <= 0)
            {
                return string.Empty;
            }
            return string.Format("WHERE {0}",
                                 string.Join(" AND ",
                                             example.Select(
                                                 new Func<KeyValuePair<string, object>, string>(
                                                     SqlSelectBuilder.WhereConditionSubString)).ToArray<string>()));
        }

        private static string WhereConditionSubString(KeyValuePair<string, object> keyValuePair)
        {
            if (keyValuePair.Value != null)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} = @{1}", new object[]
                    {
                        SqlSelectBuilder.GetColumn(keyValuePair.Key),
                        keyValuePair.Key.ToLower()
                    });
            }
            return string.Format(CultureInfo.InvariantCulture, "{0} IS NULL", new object[]
                {
                    SqlSelectBuilder.GetColumn(keyValuePair.Key)
                });
        }
    }
}