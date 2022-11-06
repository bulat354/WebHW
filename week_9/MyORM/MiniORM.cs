using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections.Specialized;
using MyORM.Builder;

namespace MyORM
{
    public class MiniORM : AbstractORM
    {
        public MiniORM(string dataSource = "(localdb)\\MSSQLLocalDB", string catalog = "AppDB") : base(dataSource, catalog) { }

        protected int ExecuteNonQuery(SqlNonQueryBuilder query)
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("ERROR: Connection opened already");
                return 0;
            }

            connection.Open();
            var command = query.GetSqlCommand();
            command.Connection = connection;
            int result;

            try
            {
                result = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Invalid query");
                Console.WriteLine(e);
                result = 0;
            }
            connection.Close();

            return result;
        }

        protected object? ExecuteScalar(SqlScalarBuilder query)
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("ERROR: Connection opened already");
                return null;
            }

            connection.Open();
            var command = query.GetSqlCommand();
            command.Connection = connection;
            object? result;

            try
            {
                result = command.ExecuteScalar();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Invalid query");
                Console.WriteLine(e);
                result = null;
            }
            connection.Close();

            return result;
        }

        protected IEnumerable<T> ExecuteReader<T>(SqlReaderBuilder query, Func<SqlDataReader, IEnumerable<T>> parser)
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("ERROR: Connection opened already");
                return Enumerable.Empty<T>();
            }

            connection.Open();
            var command = query.GetSqlCommand();
            command.Connection = connection;
            IEnumerable<T> result;

            try
            {
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                    result = parser(reader).ToArray();
                else
                    result = Enumerable.Empty<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Invalid query");
                Console.WriteLine(e);
                result = Enumerable.Empty<T>();
            }
            connection.Close();

            return result;
        }

        protected IEnumerable<T> Parse<T>(SqlDataReader reader)
        {
            var type = typeof(T);
            var colCount = reader.FieldCount;

            var names = new string[colCount];
            var propsDict = EntityModel.GetVisibleProperties<T>()
                .ToDictionary(x => EntityModel.GetColumnName(x));
            var properties = new PropertyInfo[colCount];

            for (int i = 0; i < colCount; i++)
            {
                names[i] = reader.GetName(i);

                try
                {
                    properties[i] = propsDict[names[i]];
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: Type {type.Name} doesn't contain property {names[i]}");
                    Console.WriteLine(e);
                    yield break;
                }
            }

            while (reader.Read())
            {
                var obj = Activator.CreateInstance<T>();

                for (int i = 0; i < properties.Length; i++)
                {
                    try
                    {
                        properties[i].SetValue(obj, reader.GetValue(i));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: Cannot cast {reader.GetValue(i)} to {properties[i].PropertyType}");
                        Console.WriteLine(e);
                        yield break;
                    }
                }

                yield return obj;
            }
        }

        public IEnumerable<T> Select<T>(SqlSelectBuilder query)
        {
            return ExecuteReader(query, Parse<T>);
        }

        public bool Insert<T>(SqlInsertBuilder query)
        {
            return ExecuteNonQuery(query) > 0;
        }

        public bool Update<T>(SqlUpdateBuilder query)
        {
            return ExecuteNonQuery(query) > 0;
        }

        public bool Delete<T>(SqlDeleteBuilder query)
        {
            return ExecuteNonQuery(query) > 0;
        }

        public SqlSelectBuilder Select() => new SqlSelectBuilder();
        public SqlInsertBuilder Insert() => new SqlInsertBuilder();
        public SqlUpdateBuilder Update() => new SqlUpdateBuilder();
        public SqlDeleteBuilder Delete() => new SqlDeleteBuilder();
    }
}
