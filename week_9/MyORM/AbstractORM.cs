using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORM
{
    public abstract class AbstractORM : IDisposable
    {
        public readonly string connectionString;
        protected SqlConnection _connection;

        public AbstractORM(string dataSource = "(localdb)\\MSSQLLocalDB", string catalog = "AppDB")
        {
            if (dataSource == null || catalog == null)
                throw new ArgumentNullException();

            connectionString = $"Data Source={dataSource};Initial Catalog={catalog};Integrated Security=True";
            _connection = new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
