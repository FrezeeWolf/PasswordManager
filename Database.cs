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
                        PasswordHash TEXT NOT NULL,
                        Salt TEXT NOT NULL,
                        EncryptedDEK BLOB NOT NULL,
                        Nonce BLOB NOT NULL,
                        Tag BLOB NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Services
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Login BLOB NOT NULL,
                        EncryptedPassword BLOB NOT NULL,
                        Description TEXT,
                        PassNonce BLOB NOT NULL,
                        PassTag BLOB NOT NULL,
                        LoginNonce BLOB NOT NULL,
                        LoginTag BLBO NOT NULL
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