using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgAdminOrShell;

public class ConnectDatabase
{
    public string database { get; set; }
    public string server { get; set; }
    public int port;
    public string username { get; set; }
    public string password { get; set; }

    public string ConnectionStringCollect()
    {
    metka:
        Console.Write("Server [localhost]: ");
        server = Console.ReadLine();
        if (string.IsNullOrEmpty(server))
            server = "localhost";

        Console.Write("Database [postgres]: ");
        database = Console.ReadLine();
        if (string.IsNullOrEmpty(database))
            database = "postgres";

        Console.Write("Port [5432]: ");
        bool portBeauty = int.TryParse(Console.ReadLine(),out port); 
        if (port == 0)
            port = 5432;

        Console.Write("Username [postgres]: ");
        username = Console.ReadLine();
        if (string.IsNullOrEmpty(username))
            username = "postgres";


        Console.Write("Password for user postgres: ");
        password = string.Empty;
        ConsoleKeyInfo key;

        while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (key.Key != ConsoleKey.Backspace)
            {
                password += key.KeyChar;
            }
            else if (password.Length > 0)
            {
                password = password[0..^1];
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write(" ");
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }
        if (password.Length > 0)
        {
            string ConnectionString = $"Host = {server};Port={port};Database={database}; User Id={username}; password={password}";
            return ConnectionString;
        }
        else
        {
            Console.Clear();
            Console.WriteLine("\nPassword cannot be empty!");
            Console.ReadKey();
            Console.Clear();

            goto metka;
        }

    }
}
