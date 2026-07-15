using System;
using System.IO;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using PasswordManager.Models;

namespace PasswordManager.Views.PassList;

public partial class PassListView : UserControl
{

    private void BackClick(object? sender, RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as MainWindow;
            window?.Navigate(new Views.Login.LoginView());
    }
    private void SettingsClick(object? sender, RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as MainWindow;
        window?.Navigate(new Views.Settings.SettingsView());
    }
    private void AddClick(object? sender, RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as MainWindow;
        window?.Navigate(new Views.Add. AddInfoView());
    }

    private List<ServiceItem> GetServiceData()
    {
        var connectionSring = "Data Source=passwords.db";

        using var connection = new SqliteConnection(connectionSring);
        connection.Open();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText =  """
                SELECT Id, Name, Login, EncryptedPassword, Description, PassNonce, PassTag, LoginNonce, LoginTag
                FROM Services
                """;

            var services = new List<ServiceItem>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                services.Add(new ServiceItem
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Login = reader.GetFieldValue<byte[]>(2),
                    Password = reader.GetFieldValue<byte[]>(3),
                    Description = reader.GetString(4),
                    passNonce = reader.GetFieldValue<byte[]>(5),
                    passTag = reader.GetFieldValue<byte[]>(6),
                    loginNonce = reader.GetFieldValue<byte[]>(7),
                    loginTag = reader.GetFieldValue<byte[]>(8)
                });
            }
            return services;
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            return new List<ServiceItem>();
        }
    }

    private void ServiceButtonClick(object? sender, RoutedEventArgs e)
    {
        ServiceItem? selected = null;

        if (sender is Button button)
        {
            selected = button.DataContext as ServiceItem;
        }

        if (selected == null && ServicesList is ListBox servicesListBox)
        {
            selected = servicesListBox.SelectedItem as ServiceItem;
        }

        if (selected == null)
        {
            return;
        }

        var window = TopLevel.GetTopLevel(this) as MainWindow;
        window?.Navigate(new Views.ServiceDetails.ServiceDetailsView(selected));
    }

    // Коллекция, которая автоматически обновляет UI при добавлении элементов
    public ObservableCollection<ServiceItem> Services { get; set; } = new();

    public PassListView()
    {
        InitializeComponent();
        DataContext = this;

        var serviceData = GetServiceData();
        int _textId = 1;
        foreach (var service in serviceData)
        {
            service.TextId = _textId;
            _textId++;
            Services.Add(service);
        }

        if (ServicesList != null)
        {
            ServicesList.ItemsSource = Services;
        }
    }
}