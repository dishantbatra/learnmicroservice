using Npgsql;

namespace Discount.Grpc.Extensions
{
    public static class WebApplicationExtension
    {
        public static WebApplication MigrateDatabase<TContext>(this WebApplication webApplication, int retryForAvailability = 0)
        {

            using (var scope = webApplication.Services.CreateScope()) // creates a new service scope that can be used to resolve the scoped services
            {
                var services = scope.ServiceProvider; // use to resolve depdency from the scope.
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgre sql database.");

                    using (var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                    {
                        connection.Open();

                        using (var command = new NpgsqlCommand() { Connection = connection })
                        {
                            command.CommandText = " DROP Table IF EXISTS Coupon";
                            command.ExecuteNonQuery();
                            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                            command.ExecuteNonQuery();
                            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                            command.ExecuteNonQuery();

                            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                            command.ExecuteNonQuery();
                            logger.LogInformation("Migrated postresql database.");
                        }
                    }
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postresql database");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(webApplication, retryForAvailability);
                    }
                }
            }
            return webApplication;
        }
    }
}
