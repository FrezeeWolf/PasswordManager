using System;
using System.IO;
using Microsoft.Data.Sqlite;


namespace PasswordManager;

class Database
{   
    public static void InitializeDatabase()
    {
        var connectionString = "Data Source=passwords.db";

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using var command = connection.CreateCommand();

                command.CommandText = """
                    CREATE TABLE IF NOT EXISTS MasterPassword
                    (
                        Id INTEGER PRIMARY KEY,
                        PasswordHash TEXT NOT NULL,
                        Salt TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Services
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Login TEXT NOT NULL,
                        Password TEXT NOT NULL,
                        Description TEXT
                    );
                    """;

                command.ExecuteNonQuery();
            }
        }
        catch(Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
        }
    
    }
}