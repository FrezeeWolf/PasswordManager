using Avalonia.Controls;
using Avalonia.Input;

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

    public void Navigate(UserControl control)
    {
        MainContent.Content = control;
    }
}