using AISqlAssistant.Business.Services;
using AISqlAssistant.Business.Services.Interface;
using AISqlAssistant.Data.Contexts;
using AISqlAssistant.Data.Services;
using AISqlAssistant.Data.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AISqlAssistants
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register EF Core
            builder.Services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register services
            builder.Services.AddHttpClient<GoogleAIService>();
            builder.Services.AddHttpClient<OllamaService>();

            // DI
            builder.Services.AddScoped<ISqlService, SqlService>();
            builder.Services.AddScoped<IAIService, OllamaService>();

            // Enable CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.UseCors("AllowAll");

            app.Run();
        }
    }
}
