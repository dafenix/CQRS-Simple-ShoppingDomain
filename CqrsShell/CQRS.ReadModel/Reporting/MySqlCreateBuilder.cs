using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CQRS.ReadModel.Reporting.CommandBuilder;

namespace CQRS.ReadModel.Reporting
{
    public class SqlCreateBuilder : ISqlCreateBuilder
    {
        private readonly IDictionary<Type, string> _columnTypes;

        public SqlCreateBuilder()
        {
            this._columnTypes = new Dictionary<Type, string>
                {
                    {
                        typeof (bool),
                        "bit"
                    },

                    {
                        typeof (short),
                        "smallint"
                    },

                    {
                        typeof (int),
                        "int"
                    },

                    {
                        typeof (long),
                        "bigint"
                    },

                    {
                        typeof (string),
                        "nvarchar(max)"
                    },

                    {
                        typeof (double),
                        "numeric"
                    },

                    {
                        typeof (decimal),
                        "numeric"
                    },

                    {
                        typeof (float),
                        "numeric"
                    },

                    {
                        typeof (Guid),
                        "uniqueidentifier"
                    },

                    {
                        typeof (DateTime),
                        "datetime"
                    }
                };
        }

        public string CreateSqlCreateStatementFromDto(Type dtoType)
        {
            string tableName = dtoType.Name;
            return string.Format("CREATE TABLE [{0}] ({1});", tableName, this.GetColumns(dtoType));
        }

        private string GetColumns(Type dtoType)
        {
            PropertyInfo[] properties = dtoType.GetProperties();
            return string.Join(", ", (
                                         from x in
                                             properties.Where(new Func<PropertyInfo, bool>(SqlCreateBuilder.Where))
                                         select this.GetColumn(x)).ToArray<string>());
        }

        private static bool Where(PropertyInfo propertyInfo)
        {
            return !propertyInfo.PropertyType.IsGenericType;
        }

        private string GetColumn(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.Name.Equals("Id"))
            {
                return string.Format("[{0}] {1}", propertyInfo.Name, this.GetColumnType(propertyInfo));
            }
            return string.Format("[{0}] {1} primary key", propertyInfo.Name, this.GetColumnType(propertyInfo));
        }

        private string GetColumnType(PropertyInfo propertyInfo)
        {
            Type type = propertyInfo.PropertyType;
            if (!this._columnTypes.ContainsKey(type))
            {
                throw new Exception(string.Format("The key {0} was not found!", type.Name));
            }
            return this._columnTypes[type];
        }
    }
}