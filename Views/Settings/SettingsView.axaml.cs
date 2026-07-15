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
        if (string.IsNullOrEmpty(NewMasterPassword.Text))
        {
            confirmLabel.Content = "Master Password cannot be empty!";
            return;
        }
        else
        {
            ChangeMasterPassword();
        } 
    }

    private void SaveEmergencyPasswordClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(NewEmergencyPassword.Text))
        {
            confirmLabel.Content = "Emergency Password cannot be empty!";
            return;
        }
        else
        {
            ChangeEmergencyPassword();
        } 
            
    }

    private void ChangeMasterPassword()
    {
        (string storedEmergencyPasswordHash, string storedMasterPasswordHash, string storedSalt) = Views.Login.LoginView.GetMasterPasswordData();
        string newEnteredPass = NewMasterPassword.Text ?? "";
        string newHash = Views.Login.LoginView.ComputeHash(newEnteredPass, storedSalt);
        byte[] newMasterKey = Views.Login.LoginView.DeriveKey(newEnteredPass, storedSalt);
        var (reEncryptedDEK,newNonce, newTag) = ReEncryptDEK(newMasterKey);
        if (newHash == storedMasterPasswordHash)
        {
            confirmLabel.Content = "New MP cannot be the same as the current one!";
            return;
        }
        else if (newHash == storedEmergencyPasswordHash)
        {
            confirmLabel.Content = "MP cannot be the same as the EP!";
            return;
        }
        else
        {
            var connectionString = "Data Source=passwords.db";
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText =
                """
                UPDATE MasterPassword
                SET EmergencyPasswordHash = $emergecyhash, PasswordHash = $hash, Salt = $salt, EncryptedDEK = $dek, Nonce = $nonce, Tag = $tag
                WHERE Salt = $oldSalt;
                """;
                command.Parameters.AddWithValue("$emergecyhash", storedEmergencyPasswordHash);
                command.Parameters.AddWithValue("$hash", newHash);
                command.Parameters.AddWithValue("$salt", storedSalt);
                command.Parameters.AddWithValue("$dek", reEncryptedDEK);
                command.Parameters.AddWithValue("$nonce", newNonce);
                command.Parameters.AddWithValue("$tag", newTag);
                command.Parameters.AddWithValue("$oldSalt", storedSalt);
                command.ExecuteNonQuery();

                NewMasterPasswordPanel.IsVisible = false;
                FirstButtonsPanel.IsVisible = true;
                confirmLabel.Content = "Master password changed successfully!";
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.txt", ex.ToString());
            }
        }
    }

    private void ChangeEmergencyPassword()
    {
        string newEnteredPass = NewEmergencyPassword.Text ?? "";
        (string storedEmergencyPasswordHash, string storedMasterPasswordHash, string storedSalt) = Views.Login.LoginView.GetMasterPasswordData();
        string newEmergencyPasswordHash = Views.Login.LoginView.ComputeHash(newEnteredPass, storedSalt);
        if (newEmergencyPasswordHash == storedMasterPasswordHash)
        {
            confirmLabel.Content = "EP cannot be the same as the MP!";
            return;
        }
        else if (newEmergencyPasswordHash == storedEmergencyPasswordHash)
        {
            confirmLabel.Content = "New EP cannot be the same as the current one!";
            return;
        }
        else
        {
            var connectionString = "Data Source=passwords.db";
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText =
                """
                UPDATE MasterPassword
                SET EmergencyPasswordHash = $hash
                WHERE Salt = $salt;
                """;
                command.Parameters.AddWithValue("$hash", newEmergencyPasswordHash);
                command.Parameters.AddWithValue("$salt", storedSalt);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.txt", ex.ToString());
            }
        }
            NewEmergencyPasswordPanel.IsVisible = false;
            FirstButtonsPanel.IsVisible = true;
            confirmLabel.Content = "Emergency password changed successfully!";
    } 

    private (byte[] encdek, byte[] nonce, byte[] tag) ReEncryptDEK(byte[] _newMaterKey)
    {
        byte[] newMasterKey = _newMaterKey;
        byte[] dek = Views.Login.LoginView.Session.DEK!;
        var (reEncryptedDEK, newNonce, newTag) = Views.Login.LoginView.Encrypt(dek, newMasterKey);
        return (reEncryptedDEK, newNonce, newTag);  
    }
}