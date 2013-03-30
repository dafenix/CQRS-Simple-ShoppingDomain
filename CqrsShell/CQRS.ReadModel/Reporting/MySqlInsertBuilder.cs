using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CQRS.ReadModel.Reporting.CommandBuilder;

namespace CQRS.ReadModel.Reporting
{
    public class SqlInsertBuilder : ISqlInsertBuilder
    {
        public string CreateSqlInsertStatementFromDto<TDto>() where TDto : class
        {
            return SqlInsertBuilder.GetInsertString<TDto>();
        }

        private static string GetInsertString<TDto>()
        {
            Type type = typeof (TDto);
            IEnumerable<PropertyInfo> properties =
                type.GetProperties().Where(new Func<PropertyInfo, bool>(SqlInsertBuilder.Where));
            string tableName = type.Name;
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2});", tableName, string.Join(",", (
                                                                                                        from x in
                                                                                                            properties
                                                                                                        select
                                                                                                            string
                                                                                                            .Format(
                                                                                                                "{0}",
                                                                                                                x.Name))
                                                                                                        .ToArray<string>
                                                                                                        ()),
                                 string.Join(",", (
                                                      from x in properties
                                                      select string.Format("@{0}", x.Name.ToLower())).ToArray<string>()));
        }

        private static bool Where(PropertyInfo propertyInfo)
        {
            return !propertyInfo.PropertyType.IsGenericType;
        }
    }
}