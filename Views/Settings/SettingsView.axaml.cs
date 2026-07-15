using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Security.Cryptography;
using System.Text;
using PasswordManager.Models;

namespace PasswordManager.Views.Settings;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void MPCheckBoxChanged(object? sender, RoutedEventArgs e)
    {
        var checkBox = (CheckBox)sender!;
        NewMasterPassword.PasswordChar = checkBox.IsChecked == true ? '\0' : '*';
    }

    private void EPCheckBoxChanged(object? sender, RoutedEventArgs e)
    {
        var checkBox = (CheckBox)sender!;
        NewEmergencyPassword.PasswordChar = checkBox.IsChecked == true ? '\0' : '*';
    }

    private void FinalBackClick(object? sender, RoutedEventArgs e)
    {
        confirmLabel.Content = "";
        confirmLabel.IsVisible = false;
        NewMasterPasswordPanel.IsVisible = false;
        NewEmergencyPasswordPanel.IsVisible = false;
        FirstButtonsPanel.IsVisible = true;
    }

    private void BackClick(object? sender, RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as MainWindow;
        window?.Navigate(new Views.PassList.PassListView());
    }

    private void ChangeMasterPasswordClick(object? sender, RoutedEventArgs e)
    {
        confirmLabel.Content = "Are you sure you want to change your MP?";
        confirmLabel.IsVisible = true;
        FirstButtonsPanel.IsVisible = false;
        ConfirmChangeMasterPasswordPanel.IsVisible = true;
    }

    private void ChangeEmergencyPasswordClick(object? sender, RoutedEventArgs e)
    {
        confirmLabel.Content = "Are you sure you want to change your EP?";
        confirmLabel.IsVisible = true;
        FirstButtonsPanel.IsVisible = false;
        ConfirmChangeEmergencyPasswordPanel.IsVisible = true;
    }

    private void ConfirmChangeMasterPasswordClick(object? sender, RoutedEventArgs e)
    {
        confirmLabel.Content = "Enter a new master password.";
        NewMasterPassword.Text = "";
        ConfirmChangeMasterPasswordPanel.IsVisible = false;
        NewMasterPasswordPanel.IsVisible = true;

    }
    private void CancelChangeMasterPasswordClick(object? sender, RoutedEventArgs e)
    {
        confirmLabel.Content = "";
        confirmLabel.IsVisible = false;
        ConfirmChangeMasterPasswordPanel.IsVisible = false;
        FirstButtonsPanel.IsVisible = true;
    }

    private void ConfirmChangeEmergencyPasswordClick(object? sender, RoutedEventArgs e)
    {
        confirmLabel.Content = "Enter a new emergency password.";
        NewEmergencyPassword.Text = "";
        ConfirmChangeEmergencyPasswordPanel.IsVisible = false;
        NewEmergencyPasswordPanel.IsVisible = true;
    }
    private void CancelChangeEmergencyPasswordClick(object? sender, RoutedEventArgs e)
    {
        confirmLabel.Content = "";
        confirmLabel.IsVisible = false;
        ConfirmChangeEmergencyPasswordPanel.IsVisible = false;
        FirstButtonsPanel.IsVisible = true;
    }
    private void SaveNewMasterPasswordClick(object? sender, RoutedEventArgs e)
    {
        ChangeMasterPassword();
        NewMasterPasswordPanel.IsVisible = false;
        FirstButtonsPanel.IsVisible = true;
        confirmLabel.Content = "Master password changed successfully!";
    }

    private void SaveEmergencyPasswordClick(object? sender, RoutedEventArgs e)
    {
        // Implement the logic to save the new emergency password
        // For example, you might want to hash the password and store it securely
        // string newMasterPassword = NewMasterPassword.Text;
        // Save the new master password securely
        NewEmergencyPasswordPanel.IsVisible = false;
        FirstButtonsPanel.IsVisible = true;
        confirmLabel.Content = "Emergency password changed successfully!";
    }

    private void ChangeMasterPassword()
    {
        string newSalt = Guid.NewGuid().ToString();
        string oldSalt = Views.Login.LoginView.Session.Salt!;
        string newEnteredPass = NewMasterPassword.Text ?? "";
        string newHash = Views.Login.LoginView.ComputeHash(newEnteredPass, newSalt);
        byte[] newMasterKey = Views.Login.LoginView.DeriveKey(newEnteredPass, newSalt);
        var (reEncryptedDEK,newNonce, newTag) = ReEncryptDEK(newMasterKey);
        
        var connectionString = "Data Source=passwords.db";
        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
            """
            UPDATE MasterPassword
            SET PasswordHash = $hash, Salt = $salt, EncryptedDEK = $dek, Nonce = $nonce, Tag = $tag
            WHERE Salt = $oldSalt;
            """;
            command.Parameters.AddWithValue("$hash", newHash);
            command.Parameters.AddWithValue("$salt", newSalt);
            command.Parameters.AddWithValue("$dek", reEncryptedDEK);
            command.Parameters.AddWithValue("$nonce", newNonce);
            command.Parameters.AddWithValue("$tag", newTag);
            command.Parameters.AddWithValue("$oldSalt", oldSalt);
            command.ExecuteNonQuery();

            //декрипт для дальнейшего использования
            byte[] dek = Views.Login.LoginView.Decrypt(reEncryptedDEK, newNonce, newTag, newMasterKey);
            Views.Login.LoginView.Session.Salt = newSalt;
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
        }
    }

    private (byte[] encdek, byte[] nonce, byte[] tag) ReEncryptDEK(byte[] _newMaterKey)
    {
        byte[] newMasterKey = _newMaterKey;
        byte[] dek = Views.Login.LoginView.Session.DEK!;
        var (reEncryptedDEK, newNonce, newTag) = Views.Login.LoginView.Encrypt(dek, newMasterKey);
        return (reEncryptedDEK, newNonce, newTag);  
    }
}