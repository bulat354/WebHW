using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer.Controllers;
using MyORM;
using System.Data.SqlClient;

namespace MyServer.DAO
{
    public class AccountDAO : AbstractDAO<Account, int>
    {
        public readonly string TableName = "Accounts";
        public readonly string IdName = "Id";
        public readonly string PasswordName = "Password";

        public AccountDAO(string tableName, string idName, string passwordName) : base()
        {
            TableName = tableName;
            IdName = idName;
            PasswordName = passwordName;
        }
        public AccountDAO() : base() { }

        public override bool Delete(Account entity)
        {
            OpenConnection
                (
                string.Join('\n',
                    $"DELETE FROM {TableName}",
                    $"WHERE {IdName} = {entity.Id}")
                );
            var result = command.ExecuteNonQuery();
            CloseConnection();
            return result > 0;
        }
        public override List<Account> GetAll()
        {
            OpenConnection($"SELECT * FROM {TableName}");
            var result = Parse(command.ExecuteReader()).ToList();
            CloseConnection();
            return result;
        }
        public override Account? GetEntityById(int id)
        {
            OpenConnection($"SELECT * FROM {TableName} WHERE {IdName} = {id}");
            var result = Parse(command.ExecuteReader()).ToList().FirstOrDefault();
            CloseConnection();
            return result;
        }
        public override bool Insert(Account entity)
        {
            OpenConnection
                (
                string.Join('\n',
                    $"INSERT INTO {TableName} VALUES",
                    $"({entity.Id}, '{entity.Password}')")
                );
            var result = command.ExecuteNonQuery();
            CloseConnection();
            return result > 0;
        }
        public override bool Update(Account entity)
        {
            OpenConnection
                (
                string.Join('\n', 
                $"UPDATE {TableName}",
                $"SET {PasswordName} = '{entity.Password}'",
                $"WHERE {IdName} = {entity.Id}")
                );
            var result = command.ExecuteNonQuery();
            CloseConnection();
            return result > 0;
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
