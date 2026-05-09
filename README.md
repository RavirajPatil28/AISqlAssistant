# Text-to-SQL AI Application
A full-stack application that converts natural language questions into SQL queries using an LLM and executes them on a SQL Server database.

---

## Features
* Convert plain English → SQL queries
* AI-powered query generation (using Ollama)
* Executes queries on SQL Server
* Displays results in dynamic table format
* Supports complex queries (joins, CTEs, aggregations)
* Example prompts for quick testing

---

## Tech Stack
### Backend
* C#
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server (Docker / Local)

### Frontend
* Angular (Standalone Components)
* Bootstrap

### AI / LLM
* Ollama (Local LLM runtime)
* Model: `llama3.1`

---

## Project Structure
/backend   → ASP.NET Core API
/frontend  → Angular UI
/database  → SQL scripts (optional)

---

## Prerequisites
Make sure you have installed:
* .NET 6 or higher
* Node.js (v18+ recommended)
* Angular CLI
* Docker (optional, for SQL Server)
* Ollama [llama3.1]

---

## Step 1 — Setup LLM (Ollama)

### 1. Install Ollama
Download from:
https://ollama.com/download

### 2. Start Ollama
```bash
ollama serve
```

### 3. Pull LLM Model
```bash
ollama pull llama3.1
```

### 4. Verify it’s working
```bash
ollama run llama3.1
```

Try:

```
Write SQL to get all employees
```

## Step 2 — Setup Database

You can use:

### Option A: Docker (Recommended)

```bash
docker run -e "ACCEPT_EULA=Y" \
           -e "MSSQL_SA_PASSWORD=StrongPass@123" \
           -p 1433:1433 \
           --name sqlserver \
           --hostname sqlserver \
           -v sql_data:/var/opt/mssql \
           -d mcr.microsoft.com/mssql/server:2022-latest
```

### Create Tables
Run this SQL:

```sql
CREATE TABLE Country (
    CountryID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50)
);

CREATE TABLE Designation (
    DesignationID INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(50)
);

CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Salary INT,
    CountryId INT,
    DesignationId INT,
    FOREIGN KEY (CountryId) REFERENCES Country(CountryID),
    FOREIGN KEY (DesignationId) REFERENCES Designation(DesignationID)
);

INSERT INTO Designation (Title)
VALUES ('Developer'), ('TeamLead'), ('Manager');    

INSERT INTO Country (Name)
VALUES ('India'), ('USA');

INSERT INTO Employee (Name, Salary, CountryId, DesignationId)
VALUES 
('Ravi', 70000, 1, 1),
('Dhanushree', 60000, 1, 1),
('Mani', 50000, 1, 1),
('Sheethal', 100000, 2, 2),
('Nermin', 200000, 2, 3);
```

## Step 3 — Run Backend
```bash
cd backend
dotnet restore
dotnet run
```

API runs on:
```
https://localhost:7271
```

---

## Step 4 — Run Frontend
```bash
cd frontend
npm install
ng serve
```

Open:
```
http://localhost:4200
```

---

## API Endpoint
```
POST /api/query
```

### Request:
```json
{
  "question": "Top 2 highest paid employees per country"
}
```

---

### Response:
```json
{
  "question": "...",
  "generatedSql": "...",
  "result": [
    {
      "name": "Ravi",
      "salary": 70000
    }
  ]
}
```

---

## Example Prompts

* Show all employees
* Top 2 highest paid employees per country
* Employees earning above average salary
* Highest paid employee in each country
* Count employees by designation

---

## ⚠️ Important Notes
* Only **SELECT queries** are allowed (for safety)
* Ensure Ollama is running before calling API
* If UI does not update, ensure Angular is running properly
* SQL Server must be accessible from backend

---

## 🚀 Future Improvements
* 💬 ChatGPT-style UI
* 📜 Query history
* 📊 Query explanation
* 🔐 Authentication & authorization
* 🌍 Multi-database support

---

## 👨‍💻 Author
Raviraj Patil

---

## ⭐ If you like this project
Give it a star ⭐ and share!
