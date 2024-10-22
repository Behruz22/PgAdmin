using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PgAdminOrShell;

public class TableServis
{
    public void CreateTable(string tableName,NpgsqlConnection connection,List<string> Columns)
    {
        if (Columns != null || tableName != null)
        {
            string query = $"Create table {tableName} ({string.Join(',', Columns)})";

            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }
        else
            Console.WriteLine("Empty!");
        
    }

    public void SelectTable(string tableName, NpgsqlConnection connection) 
    {
        if (tableName != null)
        {
            string query = $"SELECT * FROM {tableName}";

            using (var command = new NpgsqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetName(i)}\t");
                    }
                    Console.WriteLine();
                    Console.WriteLine(new string('-', 50));

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i]}\t");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
        else
            Console.WriteLine("Empty!");
    }

    public void InsertTable(string tableName,NpgsqlConnection connection)
    {
        List<object> columns = new List<object>();  
        if (tableName != null)
        {
            using (NpgsqlConnection connect = new NpgsqlConnection()) {
                string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.Write($" {reader["COLUMN_NAME"]} ({reader["DATA_TYPE"]}): ");
                            var column = Console.ReadLine();
                            columns.Add(column);

                        }
                        InsertData(connection, columns, tableName);
                    }
                }
            }
        }
        else
            Console.WriteLine("Empty!");
        

    }

    public void UpdateTable(string tableName,NpgsqlConnection connection,string condition,string change)
    {
        string query=$"update {tableName} set {change} where {condition}";
        using (var command = new NpgsqlCommand(query, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void DropTable(string tableName,NpgsqlConnection connection) 
    {
        string queryDropTable = $"Drop Table {tableName}";

        using (NpgsqlCommand command = new NpgsqlCommand(queryDropTable, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    

    public void StructureTable(string tableName,NpgsqlConnection connection)
    {
        if (tableName != null)
        {
            string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";

            using (var command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine($"Table: {tableName}");
                    while (reader.Read())
                    {
                        Console.WriteLine($" {reader["COLUMN_NAME"]} : {reader["DATA_TYPE"]}");
                    }
                }
            }
        }
        else
             Console.WriteLine("Empty!");

    }

   public void InsertData(NpgsqlConnection connection,List<object> columns,string tableName)
    {
        string query = $"Insert into {tableName} Values ({string.Join(',', columns)})";
        using (var command= new NpgsqlCommand(query, connection)) 
        {
            command.ExecuteNonQuery();
        }
    }
}
