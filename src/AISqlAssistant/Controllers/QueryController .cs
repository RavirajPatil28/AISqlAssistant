using AISqlAssistant.Business.Models;
using AISqlAssistant.Business.Services.Interface;
using AISqlAssistant.Data.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AISqlAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController: ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ISqlService _sqlService;

        public QueryController(IAIService aiService, ISqlService sqlService)
        {
            _aiService = aiService;
            _sqlService = sqlService;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] QueryRequest request)
        {
            var question = request?.Question;

            if (string.IsNullOrWhiteSpace(question))
            {
                return BadRequest("Question cannot be empty.");
            }

            string sqlQuery = string.Empty;

            try
            {
                // Generate SQL
                sqlQuery = await _aiService.GenerateSql(question);

                sqlQuery = CleanSql(sqlQuery);

                // Validate SQL
                if (!IsSafeQuery(sqlQuery))
                {
                    return BadRequest(new
                    {
                        message = "Only safe SELECT queries are allowed.",
                        generatedSql = sqlQuery
                    });
                }

                // Execute SQL
                var data = await _sqlService.ExecuteQuery(sqlQuery);

                return Ok(new
                {
                    question,
                    generatedSql = sqlQuery,
                    result = data
                });
            }
            catch (Exception ex)
            {
                try
                {
                    // If SQL not generated, don't retry
                    if (string.IsNullOrWhiteSpace(sqlQuery))
                    {
                        return StatusCode(500, new
                        {
                            question,
                            error = "Failed to generate SQL",
                            details = ex.Message
                        });
                    }

                    var retrySql = $@"
                    The following SQL query is incorrect.

                    Original SQL:
                    {sqlQuery}

                    Error:
                    {ex.Message}

                    Fix the SQL Server query.

                    Rules:
                    - Use correct joins
                    - Fix column/table names
                    - Ensure valid syntax
                    - Return ONLY corrected SQL
                    ";

                    retrySql = CleanSql(retrySql);

                    // Validate retry SQL
                    if (!IsSafeQuery(retrySql))
                    {
                        return BadRequest(new
                        {
                            question,
                            error = "Retry generated unsafe SQL",
                            retrySql
                        });
                    }

                    var data = await _sqlService.ExecuteQuery(retrySql);

                    return Ok(new
                    {
                        question,
                        generatedSql = retrySql,
                        note = "Auto-corrected query",
                        result = data
                    });
                }
                catch (Exception retryEx)
                {
                    return BadRequest(new
                    {
                        question,
                        originalSql = sqlQuery,
                        error = "Query failed even after retry",
                        details = retryEx.Message
                    });
                }
            }
        }

        private string CleanSql(string sql)
        {
            return sql.Replace("```sql", "")
                      .Replace("```", "")
                      .Trim();
        }

        private bool IsSafeQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            var cleaned = sql.Trim().ToUpper();

            // Allow SELECT and CTE (WITH)
            if (!(cleaned.StartsWith("SELECT") || cleaned.StartsWith("WITH")))
                return false;

            string[] blocked =
            {
                "INSERT","UPDATE","DELETE",
                "DROP","ALTER","TRUNCATE",
                "EXEC","MERGE"
            };

            return !blocked.Any(keyword => cleaned.Contains(keyword));
        }
    }
}
