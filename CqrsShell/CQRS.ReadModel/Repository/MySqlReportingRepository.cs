using CQRS.ReadModel.Reporting;
using CQRS.ReadModel.Reporting.CommandBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace CQRS.ReadModel.Repository
{
    public class MySqlReportingRepository : IReportingRepository
    {
        private string _sqlConnectionString;
        private readonly ISqlSelectBuilder _sqlSelectBuilder;
        private readonly ISqlInsertBuilder _sqlInsertBuilder;
        private readonly ISqlUpdateBuilder _sqlUpdateBuilder;
        private readonly ISqlDeleteBuilder _sqlDeleteBuilder;
        private readonly ISqlCreateBuilder _sqlCreateBuilder;

        public MySqlReportingRepository(ISqlSelectBuilder sqlSelectBuilder, ISqlInsertBuilder sqlInsertBuilder,
                                        ISqlUpdateBuilder sqlUpdateBuilder, ISqlDeleteBuilder sqlDeleteBuilder,
                                        ISqlCreateBuilder sqlCreateBuilder)
        {
            _sqlSelectBuilder = sqlSelectBuilder;
            _sqlInsertBuilder = sqlInsertBuilder;
            _sqlUpdateBuilder = sqlUpdateBuilder;
            _sqlDeleteBuilder = sqlDeleteBuilder;
            _sqlCreateBuilder = sqlCreateBuilder;
        }

        public void UseConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(this._sqlConnectionString))
            {
                this._sqlConnectionString = connectionString;
            }
        }

        public void Bootstrap(IEnumerable<Type> reportingTypes)
        {
            using (var connection = new MySqlConnection(this._sqlConnectionString))
            {
                connection.Open();
                reportingTypes.ToList().ForEach(t => EnsureTableExists(t, connection));
            }
        }

        public IEnumerable<TDto> GetByExample<TDto>(object example) where TDto : class
        {
            if (!TableExists(typeof (TDto).Name))
            {
                return new List<TDto>();
            }
            if (example != null)
            {
                return GetByExample<TDto>(GetPropertyInformation(example));
            }
            return GetByExample<TDto>(new Dictionary<string, object>());
        }

        private IEnumerable<TDto> GetByExample<TDto>(IDictionary<string, object> example) where TDto : class
        {
            Type dtoType = typeof (TDto);
            List<TDto> dtos;
            using (var sqlConnection = new MySqlConnection(_sqlConnectionString))
            {
                sqlConnection.Open();
                using (MySqlTransaction transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        dtos = this.DoGetByExample<TDto>(transaction, dtoType, example);
                        this.GetChildren<TDto>(transaction, dtos, dtoType);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return dtos;
        }

        public void Save<TDto>(TDto dto) where TDto : class
        {
            this.Save<TDto>(GetPropertyInformation(dto));
        }

        private void EnsureTableExists(Type dtoType, MySqlConnection connection)
        {
            if (TableExists(dtoType.Name, connection))
            {
                return;
            }
            string createTableStatement = this._sqlCreateBuilder.CreateSqlCreateStatementFromDto(dtoType);
            MySqlCommand createTableCommand = new MySqlCommand(createTableStatement, connection);
            createTableCommand.ExecuteNonQuery();
        }

        private void EnsureTableExists(Type dtoType)
        {
            using (MySqlConnection connection = new MySqlConnection(this._sqlConnectionString))
            {
                connection.Open();
                this.EnsureTableExists(dtoType, connection);
            }
        }

        private bool TableExists(string tableName)
        {
            bool result;
            using (MySqlConnection connection = new MySqlConnection(this._sqlConnectionString))
            {
                connection.Open();
                result = TableExists(tableName, connection);
            }
            return result;
        }

        private static bool TableExists(string tableName, MySqlConnection connection)
        {
            string checkStatement = string.Format(CultureInfo.InvariantCulture,
                                                  "SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'",
                                                  new object[]
                                                      {
                                                          tableName
                                                      });
            MySqlCommand checkTableExistsCommand = new MySqlCommand(checkStatement, connection);
            int tablesWithThisNameCount = Convert.ToInt32(checkTableExistsCommand.ExecuteScalar());
            return tablesWithThisNameCount > 0;
        }

        private void Save<TDto>(IEnumerable<KeyValuePair<string, object>> dto) where TDto : class
        {
            string commandText = this._sqlInsertBuilder.CreateSqlInsertStatementFromDto<TDto>();
            using (MySqlConnection sqlConnection = new MySqlConnection(this._sqlConnectionString))
            {
                sqlConnection.Open();
                using (MySqlTransaction transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (
                            MySqlCommand sqlCommand = new MySqlCommand(commandText, transaction.Connection, transaction)
                            )
                        {
                            AddParameters(sqlCommand, dto);
                            sqlCommand.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Update<TDto>(object update, object where) where TDto : class
        {
            string commandText = this._sqlUpdateBuilder.GetUpdateString<TDto>(update, where);
            using (MySqlConnection sqlConnection = new MySqlConnection(this._sqlConnectionString))
            {
                sqlConnection.Open();
                using (MySqlTransaction transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (
                            MySqlCommand sqlCommand = new MySqlCommand(commandText, transaction.Connection, transaction)
                            )
                        {
                            AddUpdateParameters(sqlCommand, GetPropertyInformation(update));
                            AddParameters(sqlCommand, GetPropertyInformation(where));
                            sqlCommand.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Delete<TDto>(object example) where TDto : class
        {
            this.Delete<TDto>(GetPropertyInformation(example));
        }

        private void Delete<TDto>(IEnumerable<KeyValuePair<string, object>> example) where TDto : class
        {
            string commandText = this._sqlDeleteBuilder.CreateSqlDeleteStatementFromDto<TDto>(example);
            using (MySqlConnection sqlConnection = new MySqlConnection(this._sqlConnectionString))
            {
                sqlConnection.Open();
                using (MySqlTransaction transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (
                            MySqlCommand sqlCommand = new MySqlCommand(commandText, transaction.Connection, transaction)
                            )
                        {
                            AddParameters(sqlCommand, example);
                            sqlCommand.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void GetChildren<TDto>(MySqlTransaction transaction, IEnumerable<TDto> dtos, Type dtoType)
            where TDto : class
        {
            foreach (PropertyInfo property in dtoType.GetProperties().Where(new Func<PropertyInfo, bool>(WhereGeneric)))
            {
                foreach (TDto dto in dtos)
                {
                    Type childDtoType = property.PropertyType.GetGenericArguments().First<Type>();
                    object childDtos = base.GetType()
                                           .GetMethod("DoGetByExample", BindingFlags.Instance | BindingFlags.NonPublic)
                                           .MakeGenericMethod(new Type[]
                                               {
                                                   childDtoType
                                               }).Invoke(this, new object[]
                                                   {
                                                       transaction,
                                                       childDtoType,
                                                       CreateSelectObject<TDto>(dto)
                                                   });
                    property.SetValue(dto, childDtos, new object[0]);
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> CreateSelectObject<TDto>(TDto parentDto)
        {
            string columnName = string.Format("{0}Id", parentDto.GetType().Name);
            object columnValue = parentDto.GetType().GetProperty("Id").GetValue(parentDto, new object[0]);
            return new Dictionary<string, object>
                {
                    {
                        columnName,
                        columnValue
                    }
                };
        }

        private List<TDto> DoGetByExample<TDto>(MySqlTransaction transaction, Type dtoType,
                                                IEnumerable<KeyValuePair<string, object>> example) where TDto : class
        {
            Func<ConstructorInfo, bool> func = null;
            List<TDto> dtos = new List<TDto>();
            string commandText = this._sqlSelectBuilder.CreateSqlSelectStatementFromDto<TDto>(example);
            using (var sqlCommand = new MySqlCommand(commandText, transaction.Connection, transaction))
            {
                AddParameters(sqlCommand, example);
                using (MySqlDataReader sqLiteDataReader = sqlCommand.ExecuteReader())
                {
                    IEnumerable<ConstructorInfo> arg_4C_0 = dtoType.GetConstructors();
                    if (func == null)
                    {
                        func = ((ConstructorInfo c) => c.GetParameters().Count<ParameterInfo>() == 0);
                    }
                    ConstructorInfo dtoConstructor = arg_4C_0.Single(func);
                    while (sqLiteDataReader.Read())
                    {
                        dtos.Add(BuildDto<TDto>(dtoType, dtoConstructor, sqLiteDataReader));
                    }
                }
            }
            return dtos;
        }

        private static TDto BuildDto<TDto>(Type dtoType, ConstructorInfo dtoConstructor, IDataRecord sqLiteDataReader)
            where TDto : class
        {
            List<object> constructorArguments = new List<object>();
            object dtoInstance = dtoConstructor.Invoke(constructorArguments.ToArray());
            PropertyInfo[] properties = dtoType.GetProperties();
            PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo property = array[i];
                object value = (sqLiteDataReader[property.Name] == DBNull.Value)
                                   ? null
                                   : sqLiteDataReader[property.Name];
                property.SetValue(dtoInstance, value, new object[0]);
            }
            return (TDto) ((object) dtoInstance);
        }

        private static Dictionary<string, object> GetPropertyInformation(object example)
        {
            Dictionary<string, object> exampleData = new Dictionary<string, object>();
            example.GetType()
                   .GetProperties()
                   .Where(new Func<PropertyInfo, bool>(Where))
                   .ToList<PropertyInfo>()
                   .ForEach(delegate(PropertyInfo x) { exampleData.Add(x.Name, x.GetValue(example, new object[0])); });
            return exampleData;
        }

        private static void AddParameters(MySqlCommand sqlCommand, IEnumerable<KeyValuePair<string, object>> example)
        {
            if (example == null)
            {
                return;
            }
            example.ToList<KeyValuePair<string, object>>()
                   .ForEach(
                       delegate(KeyValuePair<string, object> x)
                           {
                               sqlCommand.Parameters.AddWithValue(string.Format("@{0}", x.Key.ToLower()),
                                                                  x.Value ?? DBNull.Value);
                           });
        }

        private static void AddUpdateParameters(MySqlCommand sqlCommand,
                                                IEnumerable<KeyValuePair<string, object>> example)
        {
            if (example == null)
            {
                return;
            }
            example.ToList<KeyValuePair<string, object>>()
                   .ForEach(
                       delegate(KeyValuePair<string, object> x)
                           {
                               sqlCommand.Parameters.AddWithValue(string.Format("@update_{0}", x.Key.ToLower()),
                                                                  x.Value ?? DBNull.Value);
                           });
        }

        private static bool Where(PropertyInfo propertyInfo)
        {
            return !propertyInfo.PropertyType.IsGenericType;
        }

        private static bool WhereGeneric(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsGenericType;
        }
    }

    internal class SqlReportingRepository
    {
    }
}