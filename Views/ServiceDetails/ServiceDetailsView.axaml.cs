using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Security.Cryptography;
using System.Text;
using PasswordManager.Models;

namespace PasswordManager.Views.ServiceDetails;

public partial class ServiceDetailsView : UserControl
{
    private ServiceItem? _selected;
    public ServiceDetailsView()
    {
        InitializeComponent();
    }

    public ServiceDetailsView(ServiceItem? Selected)
    {
        InitializeComponent();
        printData(Selected);
        _selected = Selected; 
    }

    private void BackClick(object? sender, RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as MainWindow;
        window?.Navigate(new Views.PassList.PassListView());
    }

    private void DeleteClick(object? sender, RoutedEventArgs e)
    {
        DeliteId();
        var window = TopLevel.GetTopLevel(this) as MainWindow;
        window?.Navigate(new Views.PassList.PassListView());
    }

    private void CheckBoxChanged(object? sender, RoutedEventArgs e)
    {
        var checkBox = (CheckBox)sender!;
        Password.PasswordChar = checkBox.IsChecked == true ? '\0' : '*';
    }
    private void DeliteId()
    {
        try
        {
            int id = _selected!.Id;

            using var connection = new SqliteConnection("Data Source=passwords.db");
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText =
            """
            DELETE FROM Services
            WHERE Id = $id
            """;

            command.Parameters.AddWithValue("$id", id);

            command.ExecuteNonQuery();
        }
        catch(Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
        }
        
    }

    private void DeleteFormClick(object? sender, RoutedEventArgs e)
    {
        FDel.IsVisible = false;
        SDel.IsVisible = true;
    }

    private void CancelClick(object? sender, RoutedEventArgs e)
    {
        SDel.IsVisible = false;
        FDel.IsVisible = true;
    }

    private void EditClick(object? sender, RoutedEventArgs e)
    {   
        string serviceName = ServiceName.Text ?? "";
        string login = Login.Text ?? "";
        string password = Password.Text ?? "";
        string description = Description.Text ?? "";
        int id = _selected!.Id;

        if (serviceName == "" && login == "" && password == "" && description == "")
        {
            DeliteId();
        }
        else
        {
            UpdateData(serviceName, login, password, description, id);
        }
        
        var window = TopLevel.GetTopLevel(this) as MainWindow;
        window?.Navigate(new Views.PassList.PassListView());
    }

    private void UpdateData(string _serviceName, string _login, string _password, string _description, int _id)
    {
        string serviceName = _serviceName;
        string login = _login;
        string password = _password;
        string description = _description;
        int Id = _id;

        var (encLoginByte, loginTag, loginNonce) = Views.Add.AddInfoView.encryptLogin(login);
        var (encPassByte, passTag, passNonce) = Views.Add.AddInfoView.encryptPass(password);

        var connectionString = "Data Source=passwords.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE Services
                SET
                    Name = $name,
                    Login = $login,
                    EncryptedPassword = $password,
                    Description = $description,
                    LoginNonce = $loginnonce,
                    LoginTag = $logintag,
                    PassNonce = $passnonce,
                    PassTag = $passtag
                WHERE Id = $id;
                """;
            command.Parameters.AddWithValue("$name", serviceName);
            command.Parameters.AddWithValue("$login", encLoginByte);
            command.Parameters.AddWithValue("$password", encPassByte);
            command.Parameters.AddWithValue("$description", description);
            command.Parameters.AddWithValue("$passnonce", passNonce);
            command.Parameters.AddWithValue("$passtag", passTag);
            command.Parameters.AddWithValue("$loginnonce", loginNonce);
            command.Parameters.AddWithValue("$logintag", loginTag);
            command.Parameters.AddWithValue("$id", Id);
            command.ExecuteNonQuery();
        }
        catch(Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
        }
    }


    private string? DecryptPass(ServiceItem? selected)
    {
        try
        {
            byte[] dek = Views.Login.LoginView.Session.DEK!;
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
    private string? DecryptLogin(ServiceItem? selected)
    {
        try
        {
            byte[] dek = Views.Login.LoginView.Session.DEK!;
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
        var id = selected!.Id;
        try
        {
            var login = DecryptLogin(selected);
            var password = DecryptPass(selected);
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