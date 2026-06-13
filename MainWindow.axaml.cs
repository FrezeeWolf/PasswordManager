using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace PasswordManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    //Шапка приложения
    private void HeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
    }

    private void DoneClick(object? sender, RoutedEventArgs e)
    {
        string enteredPass = inputMasterKey.Text ?? "";

        if (enteredPass == "123qwe123qwe")
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
}