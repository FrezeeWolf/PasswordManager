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
<img width="1440" height="757" alt="image" src="https://github.com/user-attachments/assets/f9428eb1-5e2d-4aa9-9024-9f99e3026b29" />
<img width="1440" height="755" alt="image" src="https://github.com/user-attachments/assets/db5563c0-0906-473b-b5c1-d2c7c7f405f1" />
<img width="1440" height="756" alt="image" src="https://github.com/user-attachments/assets/601b4c3d-5a38-4ea5-b8c3-8c0ab7c89661" />
<img width="1440" height="757" alt="image" src="https://github.com/user-attachments/assets/89c5cbe9-e85b-4f8b-8d86-2571e584921b" />

## Build

```bash
git clone https://github.com/FrezeeWolf/PasswordManager.git
cd PasswordManager
dotnet restore
dotnet run
```

## Project Status

This project was created as a learning project to practice desktop application development, database interaction and cryptography using the .NET platform.
