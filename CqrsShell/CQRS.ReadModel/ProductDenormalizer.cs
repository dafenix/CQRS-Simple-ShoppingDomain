using System.Data;
using CQRS.Domain.Events;
using CQRS.Infrastructure;
using MySql.Data.MySqlClient;

namespace CQRS.ReadModel
{
    public class ProductDenormalizer : Handles<ProductCreatedEvent>
    {
        public void Handle(ProductCreatedEvent message)
        {
            var connectionString = @"Server=localhost;Database=mycqrstest;Uid=root;Pwd=;";
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Insert Into Product(id,name) Values(@id,@name)";
                    cmd.Parameters.AddWithValue("@id", message.Id);
                    cmd.Parameters.AddWithValue("@name", message.Name);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}