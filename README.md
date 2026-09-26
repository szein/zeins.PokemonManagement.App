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
git clone https://github.com/szein/zeins.PokemonManagement.App.git
cd zeins.PokemonManagement.App
```

### 2. Set up and run the Api the database
Navigate to api folder and create the database (database can also auto created when running the api (in step 3))
```bash
cd src/api
dotnet ef database update
```

### 3. Run the App

#### Run the API
Run the following command in the Terminal (**make sure you are in the src/api directory**)
```bash
cd src/api
```

```bash
dotnet run
```
The app will start at `http://localhost:5054`

Test the api by calling:
```bash
curl http://localhost:5054/health
```

#### Run the Web

1- In another terminal navigate to web folder (if you still in repo root directory)

```bash
cd src/web
```
2- Install packages
```bash
npm install
```
3- When install is done run the web app
```bash
ng serve
```

### 4. Access the app
- API: `http://localhost:5054/api/health`
- Web UI: `http://localhost:4200/`

## Configuration

If you wish to change database name update `appsettings.json`:
```json
"DefaultConnectionName": "SqliteConnection",
  "ConnectionStrings":{
    "SqliteConnection": "Data Source=PokemonManagement.db"
  },
```

## Features

- View all Pokemon
- Add to My Collection
- Remove from My Collection

---