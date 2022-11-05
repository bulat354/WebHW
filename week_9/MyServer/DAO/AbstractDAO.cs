using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.DAO
{
    public abstract class AbstractDAO<E, K>
    {
        protected SqlConnection connection;
        protected SqlCommand command;

        public AbstractDAO
            (string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AppDB;Integrated Security=True")
        {
            connection = new SqlConnection(connectionString);
            command = connection.CreateCommand();
        }

        protected void OpenConnection(string query)
        {
            connection.Open();
            command.CommandText = query;
        }

        protected void CloseConnection()
        {
            connection.Close();
        }

        public abstract List<E> GetAll();
        public abstract E? GetEntityById(K id);
        public abstract bool Delete(E entity);
        public abstract bool Insert(E entity);
        public abstract bool Update(E entity);
    }
}
