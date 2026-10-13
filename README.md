# Make It Work

A task tracker for organizing tasks and keeping projects moving.


## Project Status

In development.

## About

Make It Work helps users manage their tasks, track progress, and stay organized. The project was built to practice and demonstrate.

## Features

<img width="1920" height="917" alt="image" src="https://github.com/user-attachments/assets/8ea492e0-9dc9-4d8e-823f-d12374a61c80" />

- Create, edit and delete tasks
- Organize task by status / project / priority
- Track task progress

## Tech Stack

- **Backend:** ASP.NET Core (.NET 10)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core 10

## Requirements

- .NET 10 SDK
- PostgreSQL
- Configuration values required by the application

### Installation

Clone the repository and navigate to the project directory:

```bash
git clone https://github.com/NikaVichy/Make-It-Work
cd Make-It-Work
```

Restore dependencies:

```bash
dotnet restore
```

Configure the application using the settings expected by the project. Check `appsettings.json`, `.env`, and the application startup code for the required database connection settings.

Run the application:

```bash
dotnet run
```
