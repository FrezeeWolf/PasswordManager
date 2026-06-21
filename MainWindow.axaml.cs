using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;


namespace PasswordManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SetOrEnter();
    }

    public static class Session
    {
        public static byte[]? DEK {get; set;}
        public static void Clear()
        {
            if (DEK != null)
            {
                Array.Clear(DEK, 0, DEK.Length);
                DEK = null;
            }
        }
    }

    //Шапка приложения
    private void HeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        inputMasterKey.Focus();
    }

    private void CheckBoxChanged(object? sender, RoutedEventArgs e)
    {
        var checkBox = (CheckBox)sender!;
        inputMasterKey.PasswordChar = checkBox.IsChecked == true ? '\0' : '*';
    }
    string Salt = Guid.NewGuid().ToString();

    private static bool IsMasterPasswordSet()
    {
        var connectionSring = "Data Source=passwords.db";

        try
        {
            using var connection = new SqliteConnection(connectionSring);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
            """
            SELECT COUNT(*)
            FROM MasterPassword;
            """;
            var count = Convert.ToInt32(command.ExecuteScalar());
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            return false;
        }
    }

    private static (string Hash, string Salt) GetMasterPasswordData()
    {
        var connectionString = "Data Source=passwords.db";

        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
            """
            SELECT PasswordHash, Salt
            FROM MasterPassword
            """;
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (
                    reader.GetString(0),
                    reader.GetString(1)
                );
            }
            else
            {
                throw new Exception("Master password not set.");
            }
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            return ("", "");
        }
    }

    private string ComputeHash(string text, string salt)
    {
        using var sha = SHA256.Create();

        byte[] bytes = Encoding.UTF8.GetBytes(text + salt);

        byte[] hash = sha.ComputeHash(bytes);

        return Convert.ToHexString(hash);
    }
    private void ValidateMasterPassword()
    {
        string enteredPass = inputMasterKey.Text ?? "";
        var (storedHash, storedSalt) = GetMasterPasswordData();
        var hashOfEntered = ComputeHash(enteredPass, storedSalt);
        byte[] masterKey = DeriveKey(enteredPass, storedSalt);
        var (encryptedDEK, nonce, tag) = DEKInfo();
        if (hashOfEntered == storedHash)
        {
            //декрипт для дальнейшего использования
            byte[] dek = Decrypt(encryptedDEK, nonce, tag, masterKey);
            Session.DEK = dek; 

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



    private byte[] DeriveKey(string password, string salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            password,
            Encoding.UTF8.GetBytes(salt),
            600000,
            HashAlgorithmName.SHA256,
            32);
    }
    private (byte[] encdek, byte[] nonce, byte[] tag) Encrypt(byte[] DEK, byte[] masterKey)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(12);

        byte[] encryptedDek = new byte[DEK.Length];
        byte[] tag = new byte[16];

        using var aes = new AesGcm(masterKey, 16);
        aes.Encrypt(nonce, DEK, encryptedDek, tag);
        return (encryptedDek, nonce, tag);
    }
    private (byte[] encdek, byte[] nonce, byte[] tag) EncryptDEK(byte[] masterKey)
    {
        byte[] dek = RandomNumberGenerator.GetBytes(32);
        var (encryptedDEK, nonce, tag) = Encrypt(dek, masterKey);
        return (encryptedDEK, nonce, tag);
    }
    private void SetMasterPassword(string Salt)
    {
        string enteredPass = inputMasterKey.Text ?? "";
        string salt = Salt;
        string hash = ComputeHash(enteredPass, salt);
        byte[] masterKey = DeriveKey(enteredPass, salt);
        var (encryptedDEK, nonce, tag) = EncryptDEK(masterKey);

        var connectionString = "Data Source=passwords.db";
        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
            """
            INSERT INTO MasterPassword (PasswordHash, Salt, EncryptedDEK, Nonce, Tag)
            VALUES ($hash, $salt, $dek, $nonce, $tag);
            """;
            command.Parameters.AddWithValue("$hash", hash);
            command.Parameters.AddWithValue("$salt", salt);
            command.Parameters.AddWithValue("$dek", encryptedDEK);
            command.Parameters.AddWithValue("$nonce", nonce);
            command.Parameters.AddWithValue("$tag", tag);
            command.ExecuteNonQuery();

            //декрипт для дальнейшего использования
            byte[] dek = Decrypt(encryptedDEK, nonce, tag, masterKey);
            Session.DEK = dek;
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
        }
    }
    private void SetOrEnter()
    {
        if (IsMasterPasswordSet())
        {
            labelMasterKey.Content = "Введите мастер-пароль";
        }
        else
        {
            labelMasterKey.Content = "Установите мастер-пароль";
        }
    }

    private (byte[] encryptedDEK, byte[] nonce, byte[] tag) DEKInfo()
    {
        var connectionString = "Data Source=passwords.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText =
            """
            SELECT EncryptedDEK, Nonce, Tag
            FROM MasterPassword
            """;
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (
                    reader.GetFieldValue<byte[]>(0), 
                    reader.GetFieldValue<byte[]>(1), 
                    reader.GetFieldValue<byte[]>(2));
            }
            return (Array.Empty<byte>(), Array.Empty<byte>(), Array.Empty<byte>());
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            return (Array.Empty<byte>(), Array.Empty<byte>(), Array.Empty<byte>());
        }
    }

    private byte[] Decrypt(byte[] encryptedDEK, byte[] nonce, byte[] tag, byte[] masterKey)
    {
        byte[] decryptedDEK = new byte[encryptedDEK.Length];

        using var aes = new AesGcm(masterKey, 16);
        aes.Decrypt(nonce, encryptedDEK, tag, decryptedDEK);

        return decryptedDEK;
    }
    
    private void DoneClick(object? sender, RoutedEventArgs e)
    {
        if(IsMasterPasswordSet())
        {
            ValidateMasterPassword();
        }
        else
        {
            SetMasterPassword(Salt);
            var passList = new passList();
            passList.Show();
            this.Close();
        }
    }
}