using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

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
        printData(selected);
        
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

    private void EditClick(object? sender, RoutedEventArgs e)
    {
        var passList = new passList();
        passList.Show();
        this.Close();
    }

    private string? decryptPass(ServiceItem? selected)
    {
        try
        {
            byte[] dek = MainWindow.Session.DEK!;
            byte[] nonce = selected!.passNonce!;
            byte[] tag = selected.passTag!;
            byte[] encPassByte = selected.Password!;

            byte[] passByte = new byte[encPassByte.Length];

            using var aes = new AesGcm(dek!, 16);

            aes.Decrypt(nonce, encPassByte, tag, passByte);

            return Encoding.UTF8.GetString(passByte);
        }

        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            return null;
        }
        
    }
    private string? decryptLogin(ServiceItem? selected)
    {
        try
        {
            byte[] dek = MainWindow.Session.DEK!;
            byte[] nonce = selected!.loginNonce!;
            byte[] tag = selected.loginTag!;
            byte[] encLoginByte = selected.Login!;

            byte[] loginByte = new byte[encLoginByte.Length];

            using var aes = new AesGcm(dek!, 16);

            aes.Decrypt(nonce, encLoginByte, tag, loginByte);

            return Encoding.UTF8.GetString(loginByte);
        }

        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            return null;
        }
        
    }


    public void printData(ServiceItem? selected)
    {
        try
        {
            var login = decryptLogin(selected);
            var password = decryptPass(selected);
            ServiceName.Text = selected?.Name ?? "";
            Login.Text = login;
            Password.Text = password;
            Description.Text = selected?.Description ?? "";
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
        }
        
    }
}