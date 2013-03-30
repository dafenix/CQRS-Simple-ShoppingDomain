using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CQRS.ReadModel.Reporting.CommandBuilder;

namespace CQRS.ReadModel.Reporting
{
    public class SqlUpdateBuilder : ISqlUpdateBuilder
    {
        public string GetUpdateString<TDto>(object update, object where) where TDto : class
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }
            if (where == null)
            {
                throw new ArgumentNullException("where");
            }
            Type type = typeof (TDto);
            IEnumerable<PropertyInfo> updateProperties =
                update.GetType().GetProperties().Where(new Func<PropertyInfo, bool>(SqlUpdateBuilder.Where));
            IEnumerable<PropertyInfo> whereProperties =
                where.GetType().GetProperties().Where(new Func<PropertyInfo, bool>(SqlUpdateBuilder.Where));
            if (updateProperties.Count<PropertyInfo>() == 0)
            {
                throw new ArgumentNullException("update");
            }
            if (whereProperties.Count<PropertyInfo>() == 0)
            {
                throw new ArgumentNullException("where");
            }
            string tableName = type.Name;
            return string.Format("UPDATE {0} SET {1} WHERE {2};", tableName, string.Join(",", (
                                                                                                  from x in
                                                                                                      updateProperties
                                                                                                  select
                                                                                                      string.Format(
                                                                                                          "{0}=@update_{1}",
                                                                                                          SqlUpdateBuilder
                                                                                                              .GetColumn
                                                                                                              (x.Name),
                                                                                                          x.Name.ToLower
                                                                                                              ()))
                                                                                                  .ToArray<string>()),
                                 string.Join(",", (
                                                      from x in whereProperties
                                                      select
                                                          string.Format("{0}=@{1}", SqlUpdateBuilder.GetColumn(x.Name),
                                                                        x.Name.ToLower())).ToArray<string>()));
        }

        private static string GetColumn(string name)
        {
            return string.Format("{0}", name);
        }

        private static bool Where(PropertyInfo propertyInfo)
        {
            return !propertyInfo.PropertyType.IsGenericType;
        }
    }
}