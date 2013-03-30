using System.Data;
using System.Data.SqlClient;
using CQRS.Domain.Events;
using CQRS.Infrastructure;
using MySql.Data.MySqlClient;

namespace CQRS.ReadModel
{
    public class CustomerDenormalizer : Handles<CustomerCreatedEvent>, Handles<CustomerRenamedEvent>
    {
        public void Handle(CustomerCreatedEvent message)
        {
            return;
            var connectionString = @"Server=localhost;Database=mycqrstest;Uid=root;Pwd=;";
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Insert Into Customer(id,Firstname,Lastname) Values(@id,@firstname,@lastname)";
                    cmd.Parameters.AddWithValue("@id", message.Id);
                    cmd.Parameters.AddWithValue("@firstname", message.FirstName);
                    cmd.Parameters.AddWithValue("@lastname", message.LastName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Handle(CustomerRenamedEvent message)
        {
            return;
            var connectionString = @"Server=.\sqlexpress;Database=mycqrstest;Trusted_Connection=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "Update dbo.Customer Set Firstname = @firstname,Lastname = @lastname Where Id = @Id";
                    cmd.Parameters.AddWithValue("@id", message.Id);
                    cmd.Parameters.AddWithValue("@firstname", message.FirstName);
                    cmd.Parameters.AddWithValue("@lastname", message.LastName);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}