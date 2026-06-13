using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;


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

        // 1. Имитируем чтение из базы данных (замените на свой код работы с БД)
        LoadDataFromDatabase();

        // 2. Привязываем коллекцию к нашему ListBox
        if (ServicesList != null)
        {
            ServicesList.ItemsSource = Services;
        }
    }

    private void LoadDataFromDatabase()
    {
        // Здесь будет ваш запрос к SQLite / PostgreSQL. 
        // Пока заполним тестовыми данными, как на макете:
        Services.Add(new ServiceItem { Id = 1, Name = "Google", Login = "user@gmail.com", Password = "password123" });
        Services.Add(new ServiceItem { Id = 2, Name = "garden.com", Login = "user@garden.com", Password = "password456" });
        Services.Add(new ServiceItem { Id = 3, Name = "2 phone", Login = "user@phone.com", Password = "password789" });
        Services.Add(new ServiceItem { Id = 4, Name = "GitHub", Login = "user@github.com", Password = "password012" });
        Services.Add(new ServiceItem { Id = 5, Name = "Notion", Login = "user@notion.com", Password = "password345" });
        Services.Add(new ServiceItem { Id = 6, Name = "Spotify", Login = "user@spotify.com", Password = "password678" });
        Services.Add(new ServiceItem { Id = 7, Name = "Telegram", Login = "user@telegram.com", Password = "password901" });
        Services.Add(new ServiceItem { Id = 8, Name = "Google", Login = "user@gmail.com", Password = "password234" });
        Services.Add(new ServiceItem { Id = 9, Name = "garden.com", Login = "user@garden.com", Password = "password567" });
        Services.Add(new ServiceItem { Id = 10, Name = "2 phone", Login = "user@phone.com", Password = "password890" });
        Services.Add(new ServiceItem { Id = 11, Name = "GitHub", Login = "user@github.com", Password = "password123" });
        Services.Add(new ServiceItem { Id = 12, Name = "Notion", Login = "user@notion.com", Password = "password456" });
        Services.Add(new ServiceItem { Id = 13, Name = "Spotify", Login = "user@spotify.com", Password = "password789" });
        Services.Add(new ServiceItem { Id = 14, Name = "Telegram", Login = "user@telegram.com", Password = "password012" });
    }
}

