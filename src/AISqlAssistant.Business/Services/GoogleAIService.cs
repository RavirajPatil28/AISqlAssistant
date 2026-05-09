using AISqlAssistant.Business.Services.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace AISqlAssistant.Business.Services
{
    public class GoogleAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateSql(string question)
        {
            var prompt = $@"
            You are a SQL Server expert.

            Database schema:

            Employee(EmployeeID, Name, Salary, CountryId, DesignationId)
            Country(CountryID, Name)
            Designation(DesignationID, Title)

            Relationships:
            Employee.CountryId = Country.CountryID
            Employee.DesignationId = Designation.DesignationID

            Rules:
            - Only generate SELECT queries
            - Use JOINs when needed
            - Return only SQL
            - No explanation

            Question:
            {question}
            ";

            var requestBody = new
            {
                contents = new[] {

                    new {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var apiKey = _configuration["GoogleAI:ApiKey"];

            var response = await _httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(result);

            string sql = json.candidates[0].content.parts[0].text.ToString();

            return sql;
        }
    }
}
