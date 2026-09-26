# My Pokemon App

## Introduction

A simple app to view, search, and manage Pokemon. Built with .NET for fast and reliable performance.

## Requirements / Tech-stack

- **.NET 8** (or higher)
- **ASP.NET Core** (Web API)
- **SQLite Database** 
- **Angular** (Frontend)

## How to Run

### 1. Clone the repository and navigate to folder
```bash
git clone <your-repo-url>
cd pokemon-app
```

### 2. Set up the database
```bash
dotnet ef database update
```

### 3. Run the App

#### Run the API
```bash
dotnet run
```
The app will start at `https://localhost:5054`

#### Run the Web
```bash
ng serve
```

### 4. Access the app
- API: `https://localhost:5054/api/health`
- Web UI: `https://localhost:4200/` (if Blazor is used)

## Configuration

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-database-connection-string"
  }
}
```

## Features

- View all Pokemon
- Add to My Collection
- Remove from My Collection

---