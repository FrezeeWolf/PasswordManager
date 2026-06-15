using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;

namespace PasswordManager;

public partial class addWindow : Window
{
    public addWindow()
    {
        InitializeComponent();
    }

    private void HeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
    }

    private void BackClick(object? sender, RoutedEventArgs e)
    {
        var passList = new passList();
        passList.Show();
        this.Close();
    }

    private void DoneClick(object? sender, RoutedEventArgs e)
    {
        AddData();
        var passList = new passList();
        passList.Show();
        this.Close();
    }

    private (byte[] encPassByte, byte[] tag, byte[] nonce) encryptPass()
    {
        string password = inputPassword.Text ?? "";

        byte[] dek = MainWindow.Session.DEK!;
        byte[] nonce = RandomNumberGenerator.GetBytes(12);
        byte[] tag = new byte[16];

        byte[] passByte = Encoding.UTF8.GetBytes(password);
        byte[] encPassByte = new byte[passByte.Length];

        using var aes = new AesGcm(dek!, 16);

        //пароль
        aes.Encrypt(nonce, passByte, encPassByte, tag);
        return (encPassByte, tag, nonce);
    }

    private (byte[] encPassByte, byte[] tag, byte[] nonce) encryptLogin()
    {
        string login = inputLogin.Text ?? "";

        byte[] dek = MainWindow.Session.DEK!;
        byte[] nonce = RandomNumberGenerator.GetBytes(12);
        byte[] tag = new byte[16];

        byte[] loginByte = Encoding.UTF8.GetBytes(login);
        byte[] encLoginByte = new byte[loginByte.Length];

        using var aes = new AesGcm(dek!, 16);
        //логин
        aes.Encrypt(nonce, loginByte, encLoginByte, tag);
        return (encLoginByte, tag, nonce);
    }


    private void AddData()
    {
        string serviceName = inputServiceName.Text ?? "";
        string description = inputDescription.Text ?? "";

        var (encLoginByte, loginTag, loginNonce) = encryptLogin();
        var (encPassByte, passTag, passNonce) = encryptPass();

        var connectionString = "Data Source=passwords.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Services (Name, Login, EncryptedPassword, Description, PassNonce, PassTag, LoginNonce, LoginTag)
                VALUES ($name, $login, $password, $description, $passnonce, $passtag, $loginnonce, $logintag);
                """;
            command.Parameters.AddWithValue("$name", serviceName);
            command.Parameters.AddWithValue("$login", encLoginByte);
            command.Parameters.AddWithValue("$password", encPassByte);
            command.Parameters.AddWithValue("$description", description);
            command.Parameters.AddWithValue("$passnonce", passNonce);
            command.Parameters.AddWithValue("$passtag", passTag);
            command.Parameters.AddWithValue("$loginnonce", loginNonce);
            command.Parameters.AddWithValue("$logintag", loginTag);
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
        }
    }
}