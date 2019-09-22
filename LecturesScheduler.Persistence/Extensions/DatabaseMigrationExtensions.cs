using Microsoft.EntityFrameworkCore;
using System;

namespace LecturesScheduler.Persistence.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static void MigrateDatabase<TDbContext>(this TDbContext dbContext) where TDbContext : DbContext
        {
            var db = dbContext.Database;           

            var migrate = false;
            foreach (var pendingMigration in db.GetPendingMigrations())
            {
                migrate = true;
                Console.WriteLine($"These are the migration that will be applied: {pendingMigration}.");
            }

            if (migrate)
            {
                Console.WriteLine("Applying DB migrations ...");
                db.SetCommandTimeout(3 * 60);
                db.Migrate();
            }
            else
            {
                Console.WriteLine("No pending DB migrations.");
            }
        }
    }
}
