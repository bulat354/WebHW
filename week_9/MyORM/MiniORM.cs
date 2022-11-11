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

        internal int ExecuteNonQuery(SqlNonQueryBuilder query)
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("ERROR: Connection opened already");
                return 0;
            }

            _connection.Open();
            var command = query.GetSqlCommand();
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
            _connection.Close();

            return result;
        }

        internal object? ExecuteScalar(SqlScalarBuilder query)
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("ERROR: Connection opened already");
                return null;
            }

            _connection.Open();
            var command = query.GetSqlCommand();
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
            _connection.Close();

            return result;
        }

        internal IEnumerable<T> ExecuteReader<T>(SqlReaderBuilder query)
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("ERROR: Connection opened already");
                return Enumerable.Empty<T>();
            }

            _connection.Open();
            var command = query.GetSqlCommand();
            IEnumerable<T> result;

            try
            {
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                    result = Parse<T>(reader).ToArray();
                else
                    result = Enumerable.Empty<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Invalid query");
                Console.WriteLine(e);
                result = Enumerable.Empty<T>();
            }
            _connection.Close();

            return result;
        }

        internal IEnumerable<T> Parse<T>(SqlDataReader reader)
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

        public SqlSelectBuilder Select<T>() => 
            new SqlSelectBuilder(_connection.CreateCommand(), this).Select<T>();
        public SqlSelectBuilder Select<T>(params string[] selectors) => 
            new SqlSelectBuilder(_connection.CreateCommand(), this).Select<T>(selectors);

        public SqlInsertBuilder Insert<T>(params T[] objs) => 
            new SqlInsertBuilder(_connection.CreateCommand(), this).Insert<T>(objs);

        public SqlUpdateBuilder Update<T>(T obj) => 
            new SqlUpdateBuilder(_connection.CreateCommand(), this).Update<T>(obj);

        public SqlDeleteBuilder Delete<T>() => 
            new SqlDeleteBuilder(_connection.CreateCommand(), this).Delete<T>();

    }
}
