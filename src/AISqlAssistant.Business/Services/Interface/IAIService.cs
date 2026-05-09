namespace AISqlAssistant.Business.Services.Interface
{
    public interface IAIService
    {
        Task<string> GenerateSql(string question);
    }
}
