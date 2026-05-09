using AISqlAssistant.Business.Services.Interface;
using Newtonsoft.Json;
using System.Text;

namespace AISqlAssistant.Business.Services
{
    public class OllamaService : IAIService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }

        public async Task<string> GenerateSql(string question)
        {
            /*
            var prompt = $@"
            You are an expert in Microsoft SQL Server.

            ### DATABASE SCHEMA
            Employee(EmployeeID, Name, Salary, CountryId, DesignationId)
            Country(CountryID, Name)
            Designation(DesignationID, Title)

            ### RELATIONSHIPS
            Employee.CountryId = Country.CountryID
            Employee.DesignationId = Designation.DesignationID

            ### RULES
            - ONLY generate SELECT queries
            - NEVER use INSERT, UPDATE, DELETE
            - ALWAYS use proper JOINs when needed
            - Use aliases:
              Employee → e
              Country → c
              Designation → d
            - DO NOT invent columns
            - RETURN ONLY SQL (no explanation, no markdown)

            ### EXAMPLES
            Q: Show all employees
            A: SELECT * FROM Employee;

            Q: Show employees from India
            A: SELECT e.Name FROM Employee e
               JOIN Country c ON e.CountryId = c.CountryID
               WHERE c.Name = 'India';

            Q: Show employees with designation Manager
            A: SELECT e.Name FROM Employee e
               JOIN Designation d ON e.DesignationId = d.DesignationID
               WHERE d.Title = 'Manager';

            Q: Show employees with country and designation
            A: SELECT e.Name, c.Name AS Country, d.Title AS Designation
               FROM Employee e
               JOIN Country c ON e.CountryId = c.CountryID
               JOIN Designation d ON e.DesignationId = d.DesignationID;

            ### USER QUESTION
            {question}

            ### OUTPUT
            ";
            */

            var prompt = $@"
            You are a senior Microsoft SQL Server expert.

            ### DATABASE SCHEMA

            Employee(EmployeeID, Name, Salary, CountryId, DesignationId)
            Country(CountryID, Name)
            Designation(DesignationID, Title)

            ### RELATIONSHIPS

            Employee.CountryId = Country.CountryID
            Employee.DesignationId = Designation.DesignationID

            ### STRICT RULES

            - ONLY generate valid SQL Server SELECT queries
            - NEVER use INSERT, UPDATE, DELETE, DROP, ALTER
            - ALWAYS use proper JOINs when required
            - ALWAYS use table aliases:
              Employee → e
              Country → c
              Designation → d
            - NEVER invent columns or tables
            - ALWAYS qualify columns with aliases
            - RETURN ONLY SQL (no explanation, no markdown)

            ### ADVANCED SQL CAPABILITIES (USE WHEN NEEDED)

            - Use GROUP BY + HAVING for aggregations
            - Use subqueries for comparisons (e.g., above average)
            - Use CTE (WITH clause) for complex logic
            - Use window functions:
              ROW_NUMBER(), RANK(), DENSE_RANK()
            - Use CASE WHEN for categorization

            ### EXAMPLES

            Q: Show top 2 highest paid employees in each country
            A:
            WITH RankedEmployees AS (
                SELECT e.Name, c.Name AS Country, e.Salary,
                       ROW_NUMBER() OVER (PARTITION BY c.Name ORDER BY e.Salary DESC) AS rn
                FROM Employee e
                JOIN Country c ON e.CountryId = c.CountryID
            )
            SELECT Name, Country, Salary
            FROM RankedEmployees
            WHERE rn <= 2;

            Q: Show employees earning above their country average
            A:
            SELECT e.Name, c.Name AS Country, e.Salary
            FROM Employee e
            JOIN Country c ON e.CountryId = c.CountryID
            WHERE e.Salary > (
                SELECT AVG(e2.Salary)
                FROM Employee e2
                WHERE e2.CountryId = e.CountryId
            );

            Q: Show employee count and avg salary per country
            A:
            SELECT c.Name, COUNT(*) AS TotalEmployees, AVG(e.Salary) AS AvgSalary
            FROM Employee e
            JOIN Country c ON e.CountryId = c.CountryID
            GROUP BY c.Name;

            ### USER QUESTION
            {question}

            ### OUTPUT
            ";

            var request = new
            {
                model = "llama3.1",
                prompt = prompt,
                stream = false
            };

            var response = await _httpClient.PostAsync(
                "http://localhost:11434/api/generate",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(result);

            var sql = json.response.ToString();

            return sql;
        }
    }
}
