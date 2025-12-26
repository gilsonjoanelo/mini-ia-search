using ChatApp.Data;
using ChatApp.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatApp.Services.Internal
{
    public class MigrationAcao
    {
        public bool ExecutarMigracaoDataCempro(AppDbContext context, ILogger<MigrationAcao> logger)
        {
            var migrationAplicada = string.Empty;
            try
            {
                Console.WriteLine("Migrations started");

                var migrator = context.Database.GetService<IMigrator>();
                var appliedMigrations = context.Database.GetAppliedMigrations().ToHashSet();
                var allMigrations = context.Database.GetMigrations();
                var newMigrations = new List<string>();

                foreach (var migration in allMigrations)
                {
                    if (!appliedMigrations.Contains(migration))
                    {
                        newMigrations.Add(migration);
                    }
                }

                if (newMigrations.Any())
                {
                    foreach (var migration in newMigrations)
                    {
                        migrationAplicada = $"Applying migration: {migration}";
                        Console.WriteLine(migrationAplicada);

                        // Aplicando cada migração fora de uma transação global
                        migrator.Migrate(migration);
                    }
                    _ = context.Set<User>().Take(1).ToList();
                }
                _ = context.Model; //force the model creation

                Console.WriteLine("Migrations completed");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error applying migration: {migrationAplicada}");
                Console.WriteLine($"Error applying migrations: {ex.Message}");
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
