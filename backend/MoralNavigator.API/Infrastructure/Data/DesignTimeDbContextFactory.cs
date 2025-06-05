// File: backend/MoralNavigator.API/Infrastructure/Data/DesignTimeDbContextFactory.cs

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MoralNavigator.API.Infrastructure.Data
{
    /// <summary>
    /// Фабрика, которую использует EF Core CLI для создания AppDbContext
    /// на этапе design-time (миграции, update-database и т. п.).
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Настраиваем ConfigurationBuilder, чтобы считать appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // как раз папка backend\MoralNavigator.API
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Берём строку подключения
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Конструируем DbContextOptionsBuilder с нужным провайдером (PostgreSQL)
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
