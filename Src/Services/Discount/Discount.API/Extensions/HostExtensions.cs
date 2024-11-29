using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvalibility = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migration postgres database .");
                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand { 
                    Connection = connection
                    };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,ProductName VARCHAR(24) NOT NULL,Description TEXT,Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName,Description,Amount) VALUES ('Apple','First Buy',150)";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postgres databse");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An Error occured when migrationg postgres database");
                   if(retryForAvalibility < 55)
                    {
                        retryForAvalibility++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvalibility);
                    }
                }
            }
            return host;
        }
    }
}
