using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SetOrEnter();
    }

    //Шапка приложения
    private void HeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
    }

    private static bool IsMasterPasswordSet()
    {
        var connectionSring = "Data Source=passwords.db";

        try
        {
            using var connection = new SqliteConnection(connectionSring);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
            """
            SELECT COUNT(*)
            FROM MasterPassword;
            """;
            var count = Convert.ToInt32(command.ExecuteScalar());
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            return false;
        }
    }

    private static (string Hash, string Salt) GetMasterPasswordData()
    {
        var connectionString = "Data Source=passwords.db";

        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
            """
            SELECT PasswordHash, Salt
            FROM MasterPassword
            """;
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (
                    reader.GetString(0),
                    reader.GetString(1)
                );
            }
            else
            {
                throw new Exception("Master password not set.");
            }
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            return ("", "");
        }
    }

    private string ComputeHash(string text, string salt)
    {
        using var sha = SHA256.Create();

        byte[] bytes = Encoding.UTF8.GetBytes(text + salt);

        byte[] hash = sha.ComputeHash(bytes);

        return Convert.ToHexString(hash);
    }
    private void ValidateMasterPassword()
    {
        string enteredPass = inputMasterKey.Text ?? "";
        var (storedHash, storedSalt) = GetMasterPasswordData();
        var hashOfEntered = ComputeHash(enteredPass, storedSalt);
        if (hashOfEntered == storedHash)
        {
            var passList = new passList();
            passList.Show();

            this.Close();
        }
        else
        {
            inputMasterKey.Text = "";
            inputMasterKey.Watermark = "Неверный пароль!";
        }
    }

    private void SetMasterPassword()
    {
        string enteredPass = inputMasterKey.Text ?? "";
        string salt = Guid.NewGuid().ToString();
        string hash = ComputeHash(enteredPass, salt);

        var connectionString = "Data Source=passwords.db";
        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
            """
            INSERT INTO MasterPassword (PasswordHash, Salt)
            VALUES ($hash, $salt);
            """;
            command.Parameters.AddWithValue("$hash", hash);
            command.Parameters.AddWithValue("$salt", salt);
            command.ExecuteNonQuery();
            
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
        }
    }
    private void SetOrEnter()
    {
        if (IsMasterPasswordSet())
        {
            labelMasterKey.Content = "Введите мастер-пароль";
        }
        else
        {
            labelMasterKey.Content = "Установите мастер-пароль";
        }
    }
    
    private void DoneClick(object? sender, RoutedEventArgs e)
    {
        if(IsMasterPasswordSet())
        {
            ValidateMasterPassword();
        }
        else
        {
            SetMasterPassword();
            var passList = new passList();
            passList.Show();
            this.Close();
        }
    }
}