namespace AISqlAssistant.Data.Services.Interface
{
    public interface ISqlService
    {
        Task<List<Dictionary<string, object>>> ExecuteQuery(string sql);
    }
}
