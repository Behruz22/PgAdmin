using Npgsql;
using PgAdminOrShell;
using System.Data;
using System.Xml.Linq;

class Program
{

    public static void Main(string[] args)
    {
        Table tables = new Table();
        metka:
        #region ConnectionString
        string server = "localhost";
       
        int port = 5432;

        string username = "postgres";

        Console.Write("Enter the database: ");
        string? database = Console.ReadLine();

        Console.Write("Password for user postgres: ");
        string? password = Console.ReadLine();
        

        string ConnectionString = $"Host = {server}; Port = {port}; Database = {database}; User Id = {username}; password = {password}";

        #endregion
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                bool choice = true;
                while (choice)
                {
                    Console.Clear();
                    
                    Console.WriteLine("1.Tables");
                    Console.WriteLine("2.Functions");
                    Console.WriteLine("3.Procedures");
                    Console.WriteLine("4.Sequences");
                    Console.WriteLine("5.Exit");


                    bool selectorBeauty = int.TryParse(Console.ReadLine(), out int selector);
                    Console.Clear();
                    switch (selector)
                    {
                        //Tables
                        case 1:
                            
                            bool choiceTable = true;
                            while (choiceTable)
                            {
                                try
                                {
                                    Console.Clear();
                                    Console.WriteLine("1.Create Table");
                                    Console.WriteLine("2.Select Table");
                                    Console.WriteLine("3.Insert Table");
                                    Console.WriteLine("4.Update Table");
                                    Console.WriteLine("5.Drop Table");
                                    Console.WriteLine("6.Tables List");
                                    Console.WriteLine("7.Table structure");
                                    Console.WriteLine("8.Exit");

                                    bool selectorTableBeauty = int.TryParse(Console.ReadLine(), out int selectorTable);
                                    Console.Clear();
                                    switch (selectorTable)
                                    {
                                        case 1:
                                            Console.Write("Table name: ");
                                            string tableNameCreate = Console.ReadLine();
                                            bool columnBeauty = true;
                                            List<string> columns = new List<string>();
                                            while (columnBeauty)
                                            {
                                                Console.Clear();
                                                Console.Write("Column name: ");
                                                string columnName= Console.ReadLine();

                                                Console.Write("Column type: ");
                                                string columnType = Console.ReadLine();
                                                columns.Add(columnName + " " + columnType);

                                                Console.WriteLine("Would you like to add more information?");
                                                Console.WriteLine("1.Yes\n2.No");
                                                if (Console.ReadLine() == "2")
                                                    columnBeauty = false;
                                                
                                                    

                                            }

                                            tables.CreateTable(tableNameCreate,connection, columns);
                                            Console.WriteLine("Succes!");
                                            Console.ReadKey();
                                            break;
                                        case 2:
                                            Console.Write("Table name: ");
                                            string tableNameSelect = Console.ReadLine();
                                            tables.SelectTable(tableNameSelect, connection);
                                            Console.ReadKey();
                                            break;
                                        case 3:
                                            Console.Write("Table name: ");
                                            string tableNameInsert = Console.ReadLine();
                                            tables.InsertTable(tableNameInsert, connection);
                                            Console.WriteLine("Succes!");
                                            Console.ReadKey();
                                            break;
                                        case 4:
                                            Console.Write("Table name: ");
                                            string tableNameUpdate = Console.ReadLine();

                                            Console.Write("Changes: ");
                                            string change = Console.ReadLine();

                                            Console.Write("Condition: ");
                                            string condition= Console.ReadLine();

                                            tables.UpdateTable(tableNameUpdate, connection,condition,change);
                                            Console.WriteLine("Succes!");
                                            Console.ReadKey();
                                            break;

                                        case 5:
                                            Console.Write("Table name: ");
                                            string tableNameDrop = Console.ReadLine();

                                            tables.DropTable(tableNameDrop, connection);
                                            Console.WriteLine("Succes!");
                                            Console.ReadKey();
                                            break;
                                        case 6:
                                            tables.ListTable(connection);
                                            Console.ReadKey();
                                            break;
                                        case 7:
                                            Console.Write("Table name: ");
                                            string tableStructName = Console.ReadLine();
                                            tables.StructureTable(tableStructName, connection);
                                            Console.ReadKey();
                                            break;
                                        case 8:
                                            choiceTable = false;
                                            break;
                                        default:
                                            Console.WriteLine("Error data!");
                                            break;
                                    }
                                }
                                catch (Exception ex) 
                                {
                                    Console.WriteLine(ex.Message); 
                                    Console.ReadKey();
                                }
                            }
                            
                            break;
                        //Functions
                        case 2:
                            string queryFunction = "SELECT proname FROM pg_proc WHERE pronamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')";

                            using (var command = new NpgsqlCommand(queryFunction, connection))
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    Console.WriteLine("Function Names:");
                                    while (reader.Read())
                                    {
                                        Console.WriteLine(reader["proname"]);
                                    }
                                }
                            }
                            Console.ReadKey();
                            break;
                        //Procedures
                        case 3:
                            string queryProcedure = "SELECT proname FROM pg_proc WHERE pronamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')";

                            using (var command = new NpgsqlCommand(queryProcedure, connection))
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    Console.WriteLine("Stored Procedure Names:");
                                    while (reader.Read())
                                    {
                                        Console.WriteLine(reader["proname"]);
                                    }
                                }
                            }
                            Console.ReadKey();
                            break;
                        //Sequences
                        case 4:
                            string query = "SELECT sequence_name FROM information_schema.sequences WHERE sequence_schema = 'public'";

                            using (var command = new NpgsqlCommand(query, connection))
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    Console.WriteLine("Sequence Names:");
                                    while (reader.Read())
                                    {
                                        Console.WriteLine(reader["sequence_name"]);
                                    }
                                }
                            }
                            Console.ReadKey();
                            break;
                        //Exit
                        case 5:
                            choice = false;
                            break;
                        default:
                            Console.WriteLine("Error data!");
                            break;
                    }
                }

                
                connection.Close();
            }
    }
        catch (Exception ex) 
        {
            Console.WriteLine(ex.Message);
            Console.ReadKey();
            Console.Clear();
            goto metka;
        }

    }

}
