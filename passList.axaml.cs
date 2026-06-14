using System;
using System.IO;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;


namespace PasswordManager;

public class ServiceItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public string Description { get; set; } = "";
}
public partial class passList : Window
{
    //Шапка приложения
    private void HeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
    }

    //Кнопачкииии
    private void BackClick(object? sender, RoutedEventArgs e)
    {
        var MainWindow = new MainWindow();
        MainWindow.Show();
        this.Close();
    }
    private void LockClick(object? sender, RoutedEventArgs e)
    {
        var MainWindow = new MainWindow();
        MainWindow.Show();
        this.Close();
    }
    private void AddClick(object? sender, RoutedEventArgs e)
    {
        var addWindow = new addWindow();
        addWindow.Show();
        this.Close();
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
                SELECT Id, Name, Login, EncryptedPassword, Description
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
                    Login = reader.GetString(2),
                    Password = reader.GetString(3),
                    Description = reader.GetString(4)
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

        var serviceDetailsWindow = new ServiceDetailsWindow(selected);
        serviceDetailsWindow.Show();
        this.Close();
    }
    // private void ServiceButtonClick(object? sender, RoutedEventArgs e)
    // {
    //     if (sender is Button button && ServiceButton.TryGetValue(button, out var serviceItem))
    //     {
    //          var serviceDetailsWindow = new ServiceDetailsWindow(serviceItem);
    //         serviceDetailsWindow.Show();
    //         this.Close();
    //     }
    //     // var serviceDetailsWindow = new ServiceDetailsWindow();
    //     // serviceDetailsWindow.Show();
    //     // this.Close();
    // }



    // Коллекция, которая автоматически обновляет UI при добавлении элементов
    public ObservableCollection<ServiceItem> Services { get; set; } = new();

    public passList()
    {
        InitializeComponent();
        DataContext = this;

        var serviceData = GetServiceData();
        foreach (var service in serviceData)
        {
            Services.Add(service);
        }

        if (ServicesList != null)
        {
            ServicesList.ItemsSource = Services;
        }
    }
}

