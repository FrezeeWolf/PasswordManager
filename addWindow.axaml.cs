using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Input;
using Avalonia.Interactivity;

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
        // Здесь надо добавить код для обработки добавления нового сервиса
        var passList = new passList();
        passList.Show();
        this.Close();
    }
}