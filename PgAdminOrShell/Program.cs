using Npgsql;
using PgAdminOrShell;

class Program
{
    public static string ConnectionString { get; set; }
    static async Task Main(string[] args)
    {
        
        TableServis tableServis = new TableServis();

        ConnectDatabase connectDatabase = new ConnectDatabase();
        ConnectionString = connectDatabase.ConnectionStringCollect();

        bool choice = true;
        do
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    await conn.CloseAsync();
                }
                List<string> schemaList = new List<string>() { "Tables", "Functions", "Procedures", "Sequences", "ERD For Database", "Query Tool", "Back" };
                string choicesDatabase = WorkingWithLists(schemaList);

                switch (choicesDatabase)
                {
                    case "Tables":
                        string tableQuery = "SELECT table_name \r\nFROM information_schema.tables \r\nWHERE table_schema = 'public';\r\n";
                        List<string> tableList = await ElementsInBaseConvert(tableQuery);
                        if (tableList.Count > 0)
                        {
                            string tableName = WorkingWithLists(tableList);
                            if (tableName != "Back")
                            {
                                List<string> tablePractices = new List<string> { "Create", "Select", "Insert", "Update", "Drop", "Structure", "Back" };
                                string tableChoice = WorkingWithLists(tablePractices);
                                switch (tableChoice)
                                {
                                    case "Create":
                                        Console.WriteLine("Create");
                                        break;

                                    case "Select":
                                        await tableServis.SelectTable(tableName,ConnectionString);
                                        Console.ReadKey();
                                        break;

                                    case "Insert":
                                        Console.WriteLine("Insert");
                                        break;

                                    case "Update":
                                        Console.WriteLine("Update");
                                        break;

                                    case "Drop":
                                        Console.WriteLine("Drop");
                                        break;

                                    case "Structure":
                                        Console.WriteLine("Structure");
                                        break;


                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Table not found!");
                        }
                        Console.ReadKey();
                        break;
                    case "Functions":
                        string functionQuery = "SELECT proname FROM pg_proc WHERE pronamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')";
                        List<string> functionList = await ElementsInBaseConvert(functionQuery);
                        if (functionList.Count > 0)
                        {
                            string functionName = WorkingWithLists(functionList);

                        }
                        else
                        {
                            Console.WriteLine("Function not found!");
                        }
                        Console.ReadKey();
                        break;
                    case "Procedures":
                        string procedureQuery = "SELECT proname FROM pg_proc WHERE pronamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')";
                        List<string> procedureList = await ElementsInBaseConvert(procedureQuery);
                        if (procedureList.Count > 0)
                        {
                            string procedureName = WorkingWithLists(procedureList);
                            Console.WriteLine(procedureName);
                        }
                        else
                        {
                            Console.WriteLine("Procedure not found!");
                        }
                        Console.ReadKey();
                        break;
                    case "Sequences":
                        string sequencesQuery = "SELECT proname FROM pg_proc WHERE pronamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')";
                        List<string> sequencesList = await ElementsInBaseConvert(sequencesQuery);
                        if (sequencesList.Count > 0)
                        {
                            string sequencesName = WorkingWithLists(sequencesList);
                        }
                        else
                        {
                            Console.WriteLine("Sequence not found!");
                        }
                        Console.ReadKey();
                        break;
                    case "ERD For Database":
                        using (var connection = new NpgsqlConnection(ConnectionString))
                        {
                            await connection.OpenAsync();

                            // Jadvallar ro'yxatini olish
                            string tableErdQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";
                            using (var command = new NpgsqlCommand(tableErdQuery, connection))
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                Console.WriteLine("Jadvallar:");
                                while (await reader.ReadAsync())
                                {
                                    Console.WriteLine($" - {reader.GetString(0)}");
                                }
                            }

                            // Har bir jadval uchun ustunlar va ularning turlarini olish
                            string columnQuery = "SELECT table_name, column_name, data_type FROM information_schema.columns WHERE table_schema = 'public'";
                            using (var command = new NpgsqlCommand(columnQuery, connection))
                            using (var reader = command.ExecuteReader())
                            {
                                Console.WriteLine("\nColumns and their types:");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Table: {reader.GetString(0)}, Comumn: {reader.GetString(1)}, Type: {reader.GetString(2)}");
                                }
                            }

                            // Jadvallar o'rtasidagi bog'lanishlar (agar mavjud bo'lsa)
                            string foreignKeyQuery = "SELECT tc.table_name, kcu.column_name, ccu.table_name AS foreign_table_name, ccu.column_name AS foreign_column_name " +
                                                      "FROM information_schema.table_constraints AS tc " +
                                                      "JOIN information_schema.key_column_usage AS kcu ON tc.constraint_name = kcu.constraint_name " +
                                                      "JOIN information_schema.constraint_column_usage AS ccu ON ccu.constraint_name = tc.constraint_name " +
                                                      "WHERE constraint_type = 'FOREIGN KEY'";
                            using (var command = new NpgsqlCommand(foreignKeyQuery, connection))
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    Console.WriteLine("\nConnections:");
                                    while (reader.Read())
                                    {
                                        Console.WriteLine($"Table: {reader.GetString(0)}.{reader.GetString(1)} -> {reader.GetString(2)}.{reader.GetString(3)}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("\nLink not available!");
                                }
                            }
                        }
                        Console.ReadKey();
                        break;
                    case "Query Tool":
                        Console.WriteLine("Iltimos, SQL so'rovini kiriting:");
                        string sqlQuery = Console.ReadLine();

                        using (var connection = new NpgsqlConnection(ConnectionString))
                        {
                             await connection.OpenAsync();

                            using (var command = new NpgsqlCommand(sqlQuery, connection))
                            {
                                try
                                {
                                    // SELECT so'rovi uchun
                                    if (sqlQuery.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                                    {
                                        using (var reader = await command.ExecuteReaderAsync())
                                        {
                                            // Jadval ko'rinishida chiqarish
                                            Console.WriteLine();
                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                Console.Write($"{reader.GetName(i),-20}"); // Ustun nomini chiqarish
                                            }
                                            Console.WriteLine();

                                            Console.WriteLine(new string('-', 20 * reader.FieldCount)); // Bo'sh chiziq

                                            if (reader.HasRows)
                                            {
                                                while (await reader.ReadAsync())
                                                {
                                                    for (int i = 0; i < reader.FieldCount; i++)
                                                    {
                                                        Console.Write($"{reader[i],-20} "); // Jadvaldagi ma'lumotlar
                                                    }
                                                    Console.WriteLine();
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Natijalar topilmadi.");
                                            }
                                        }
                                    }
                                    // CREATE, INSERT, UPDATE, DELETE, ALTER, DROP so'rovlari uchun
                                    else if (sqlQuery.TrimStart().StartsWith("CREATE", StringComparison.OrdinalIgnoreCase) ||
                                             sqlQuery.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase) ||
                                             sqlQuery.TrimStart().StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) ||
                                             sqlQuery.TrimStart().StartsWith("DELETE", StringComparison.OrdinalIgnoreCase) ||
                                             sqlQuery.TrimStart().StartsWith("ALTER", StringComparison.OrdinalIgnoreCase) ||
                                              sqlQuery.TrimStart().StartsWith("DROP", StringComparison.OrdinalIgnoreCase)
                                             )
                                    {
                                        int affectedRows = await command.ExecuteNonQueryAsync();
                                        Console.WriteLine($"{affectedRows} ta qator o'zgartirildi.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Noto'g'ri SQL so'rovi.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Xatolik: {ex.Message}");
                                }
                            }
                        }
                        Console.ReadKey();
                        break;

                    case "Back":
                        string databaseQuery = "SELECT datname FROM pg_database;";
                        List<string> elements = await ElementsInBaseConvert(databaseQuery);
                        string newDatabase = WorkingWithLists(elements);
                        if (newDatabase != "Back")
                        {
                            ConnectionString = $"Host = {connectDatabase.server};Port={connectDatabase.port};Database={newDatabase}; User Id={connectDatabase.username}; password={connectDatabase.password}";
                        }
                        else
                        {
                            choice = false;
                        }
                        Console.Clear();
                        break;
                }

            }
            #region Database Exception
            catch (NpgsqlException exDatabase)
            {
                Console.Clear();
                Console.WriteLine("\nDatabase, username, port or password may be wrong!");
                Console.ReadKey();
                Console.Clear();
                ConnectDatabase connectDatabases = new ConnectDatabase();
                ConnectionString = connectDatabases.ConnectionStringCollect();
            }
            #endregion

            #region Socket Exception
            catch (System.Net.Sockets.SocketException exSocket)
            {
                Console.Clear();
                Console.WriteLine("\nThe host may be in error!");
                Console.ReadKey();
                Console.Clear();
            }
            #endregion

            #region General Exception
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("\nAn error occurred in the program. Please try again!");
                Console.ReadKey();
                Console.Clear();
            }
            #endregion
        } while (choice);


    }

    static async Task<List<string>> ElementsInBaseConvert(string query)
    {
        List<string> list = new List<string>();

        using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while ( await reader.ReadAsync())
                    {
                        list.Add(reader.GetString(0)); // Baza nomini chiqaradi
                    }
                    list.Add("Back");
                }
            }
            await connection.CloseAsync();
        }

        return list;
    }
    static string WorkingWithLists(List<string> list)
    {
        int cursor = 0;
        string name = string.Empty;
        while (true)
        {
            Console.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                if (cursor == i)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                System.Console.WriteLine(list[i]);
                Console.ResetColor();

            }
            var kl = Console.ReadKey(true);

            if (kl.Key == ConsoleKey.DownArrow)
            {
                cursor = (cursor + 1) % list.Count;
            }
            else if (kl.Key == ConsoleKey.UpArrow)
            {
                cursor = (cursor + list.Count - 1) % list.Count;
            }
            else if (kl.Key == ConsoleKey.Enter)
            {
                name = list[cursor];
                break;
            }
        }
        return name;
    }
}