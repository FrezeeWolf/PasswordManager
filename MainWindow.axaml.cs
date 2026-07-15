using System.IO;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Data.Sqlite;
using Avalonia.Controls.ApplicationLifetimes;

namespace PasswordManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Navigate(new Views.Login.LoginView());

    }

    //Шапка приложения
    private void HeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
    }
    public static void ClearDatabase()
    {
        var dbPath = "passwords.db";
        if (File.Exists(dbPath))
        {
            SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
            Process.Start(Process.GetCurrentProcess().MainModule!.FileName!);
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.Shutdown();
            }
        }
    }

    public void Navigate(UserControl control)
    {
        MainContent.Content = control;
    }
}