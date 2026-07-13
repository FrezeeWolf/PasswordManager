using System;
using System.IO;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;

namespace PasswordManager.Models;
public class ServiceItem
{
    public int TextId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public byte[]? Login { get; set; }
    public byte[]? Password { get; set; }
    public string Description { get; set; } = "";
    public byte[]? passNonce { get; set; }
    public byte[]? passTag { get; set; }
    public byte[]? loginNonce { get; set; }
    public byte[]? loginTag { get; set; }

}
