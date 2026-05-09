using AISqlAssistant.Data.Contexts;
using AISqlAssistant.Data.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AISqlAssistant.Data.Services
{
    public class SqlService : ISqlService
    {
        private readonly AppDbContext _context;

        public SqlService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQuery(string sql)
        {
            var result = new List<Dictionary<string, object>>();

            var conn = _context.Database.GetDbConnection();

            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var columnName = reader.GetName(i);

                        // Convert to camelCase
                        var camelCaseName = char.ToLowerInvariant(columnName[0]) + columnName.Substring(1);

                        row[camelCaseName] = reader.GetValue(i);
                    }

                    result.Add(row);
                }
            }
            finally
            {
                await conn.CloseAsync();
            }

            return result;
        }
    }
}
