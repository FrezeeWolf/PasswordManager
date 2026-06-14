using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Collections.Generic;
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

    private void AddData()
    {
        string serviceName = inputServiceName.Text ?? "";
        string login = inputLogin.Text ?? "";
        string password = inputPassword.Text ?? "";
        string description = inputDescription.Text ?? "";

        var connectionString = "Data Source=passwords.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Services (Name, Login, EncryptedPassword, Description)
            VALUES ($name, $login, $password, $description);
            """;
        command.Parameters.AddWithValue("$name", serviceName);
        command.Parameters.AddWithValue("$login", login);
        command.Parameters.AddWithValue("$password", password);
        command.Parameters.AddWithValue("$description", description);
        command.ExecuteNonQuery();
    }
}