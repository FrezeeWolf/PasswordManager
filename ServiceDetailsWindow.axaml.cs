using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;

namespace PasswordManager;

public partial class ServiceDetailsWindow : Window
{
    public ServiceDetailsWindow()
    {
        InitializeComponent();
    }
    public ServiceDetailsWindow(ServiceItem? selected)
    {
        InitializeComponent();
        ServiceName.Text = selected?.Name ?? "";
        Login.Text = selected?.Login ?? "";
        Password.Text = selected?.Password ?? "";
        Description.Text = selected?.Description ?? "";
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

    private void DeleteClick(object? sender, RoutedEventArgs e)
    {
        var passList = new passList();
        passList.Show();
        this.Close();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
}