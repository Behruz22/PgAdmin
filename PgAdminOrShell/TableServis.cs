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
    public async Task CreateTable(string tableName,NpgsqlConnection connection,List<string> Columns)
    {
        if (Columns != null || tableName != null)
        {
            string query = $"Create table {tableName} ({string.Join(',', Columns)})";

            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                await command.ExecuteNonQueryAsync();
            }
        }
        else
            Console.WriteLine("Empty!");
        
    }

    public async Task SelectTable(string tableName,string connectionString) 
    {
        Console.WriteLine(tableName);
        Console.WriteLine(connectionString);
        Console.ReadKey();
        if (tableName != null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = $"SELECT * FROM {tableName}";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Console.WriteLine("Reader");
                        Console.ReadKey();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader.GetName(i)}\t");
                        }
                        Console.WriteLine();
                        Console.WriteLine(new string('-', 50));

                        while (await reader.ReadAsync())
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
        }
        else
            Console.WriteLine("Empty!");
    }

    public async Task InsertTable(string tableName,NpgsqlConnection connection)
    {
        List<object> columns = new List<object>();  
        if (tableName != null)
        {
            using (NpgsqlConnection connect = new NpgsqlConnection()) {
                string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.Write($" {reader["COLUMN_NAME"]} ({reader["DATA_TYPE"]}): ");
                            var column = Console.ReadLine();
                            columns.Add(column);

                        }
                        await InsertData(connection, columns, tableName);
                    }
                }
            }
        }
        else
            Console.WriteLine("Empty!");
        

    }

    public async Task UpdateTable(string tableName,NpgsqlConnection connection,string condition,string change)
    {
        string query=$"update {tableName} set {change} where {condition}";
        using (var command = new NpgsqlCommand(query, connection))
        {
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DropTable(string tableName,NpgsqlConnection connection) 
    {
        string queryDropTable = $"Drop Table {tableName}";

        using (NpgsqlCommand command = new NpgsqlCommand(queryDropTable, connection))
        {
            await command.ExecuteNonQueryAsync();
        }
    }

    

    public async Task StructureTable(string tableName,NpgsqlConnection connection)
    {
        if (tableName != null)
        {
            string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";

            using (var command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader =await command.ExecuteReaderAsync())
                {
                    Console.WriteLine($"Table: {tableName}");
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($" {reader["COLUMN_NAME"]} : {reader["DATA_TYPE"]}");
                    }
                }
            }
        }
        else
             Console.WriteLine("Empty!");

    }

   public async Task InsertData(NpgsqlConnection connection,List<object> columns,string tableName)
    {
        string query = $"Insert into {tableName} Values ({string.Join(',', columns)})";
        using (var command= new NpgsqlCommand(query, connection)) 
        {
            await command.ExecuteNonQueryAsync();
        }
    }
}
