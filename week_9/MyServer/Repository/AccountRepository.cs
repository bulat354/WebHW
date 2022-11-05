using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer.Controllers;

namespace MyServer.Repository
{
    public class AccountRepository : ISqlRepository<Account>
    {
        protected SqlConnection connection;
        protected SqlCommand command;
        
        public AccountRepository(string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AppDB;Integrated Security=True")
        {
            connection = new SqlConnection(connectionString);
            command = connection.CreateCommand();
        }

        private int GetResult(string query)
        {
            connection.Open();
            command.CommandText = query;
            int result;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            connection.Close();
            return result;
        }

        public bool Delete(Account entity)
        {
            return GetResult($"DELETE FROM Accounts WHERE Id = {entity.Id} AND Password = '{entity.Password}'") > 0;
        }

        public bool Insert(Account entity)
        {
            return GetResult($"INSERT INTO Accounts VALUES ({entity.Id}, '{entity.Password}')") > 0;
        }

        public bool Update(Account entity)
        {
            return GetResult($"UPDATE Accounts SET Password = '{entity.Password}' WHERE Id = {entity.Id}") > 0;
        }

        public List<Account> GetAll()
        {
            connection.Open();
            command.CommandText = $"SELECT * FROM Accounts";
            var result = Parse(command.ExecuteReader()).ToList();
            connection.Close();
            return result;
        }

        public List<Account> GetValues(ISqlSpecification<Account> specification)
        {
            connection.Open();
            command.CommandText = $"SELECT * FROM Accounts WHERE {specification.ToSqlClauses()}";
            var result = Parse(command.ExecuteReader()).ToList();
            connection.Close();
            return result;
        }

        private IEnumerable<Account> Parse(SqlDataReader reader)
        {
            if (reader.HasRows)
                while (reader.Read())
                    yield return new Account() { Id = reader.GetInt32(0), Password = reader.GetString(1) };

            yield break;
        }
    }
}
