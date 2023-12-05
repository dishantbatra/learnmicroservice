using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static WebApplication MigrateDatabase<TContext>(this WebApplication webApplication, Action<TContext, IServiceProvider> seeder, int? retry = 0) where TContext:DbContext{
            int retryForAvailability = retry.Value;
            using(var scope = webApplication.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();


                var retryPolicy = Policy.Handle<SqlException>().
                     WaitAndRetry(retryCount: 5,
                     sleepDurationProvider: x => TimeSpan.FromMinutes(Math.Pow(2, 2)),
                     onRetry: (exception, retryCount, context) =>
                     {
                         logger.LogError($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                     });


                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                    retryPolicy.Execute(() =>InvokeSeeder(seeder, context, services));
                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);


                    /* We have to create a retry mechanism in here, because when the application starting on the new environment, 
                     * there is no guarantee that our SQL Server database container will be ready when the ordering API microservice start up. 
                     * So for that reason we retry when we can't reach the SQL Server databases. And also in the upcoming sections, 
                     * we create this retry mechanism with using the Poly NuGet package. */
                    //if (retryForAvailability < 50)
                    //{
                    //    retryForAvailability++;
                    //    System.Threading.Thread.Sleep(2000);
                    //    MigrateDatabase<TContext>(webApplication, seeder, retryForAvailability);
                    //}
                }
                 return webApplication;
            }

        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder,
                                                    TContext context,
                                                    IServiceProvider services)
                                                    where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services); // It is executed once we have migrated the database
        }
    }
}
