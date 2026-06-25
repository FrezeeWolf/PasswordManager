# Password Manager

A simple cross-platform password manager built with **C#**, **Avalonia UI** and **SQLite**.

The application allows you to securely store credentials for different services using a master password. User data is encrypted before being written to the database, while the encryption key (DEK) is protected separately by the master password.

## Features

* Master password authentication
* Secure storage of credentials
* AES-GCM encryption for logins and passwords
* SQLite database
* Add, edit and delete saved services
* Cross-platform desktop interface built with Avalonia

## Technologies

* C#
* .NET
* Avalonia UI
* SQLite
* PBKDF2
* AES-GCM

## Screenshots

> Add screenshots here.

## Build

```bash
git clone <repository-url>
cd PasswordManager
dotnet restore
dotnet run
```

## Project Status

This project was created as a learning project to practice desktop application development, database interaction and cryptography using the .NET platform.
