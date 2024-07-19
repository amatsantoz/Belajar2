using Belajar2.GraphQL.Cars;
using Microsoft.EntityFrameworkCore;
using Library.PublishSubscribe2;

namespace Belajar2
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SQLSERVERCONNECTION"));
            });

            services.AddGraphQLServer()
                .AddQueryType<QueryCar>()
                .AddMutationType<MutationCar>()
                .AddType<CarType>();

            // Menggunakan extension methods untuk mendaftarkan Kafka Producer dan Consumer
            services.AddKafkaServices();

            services.AddScoped<CarPurchaseService>();
                    
            // Tambahkan logging untuk diagnosa
            services.AddLogging(configure => configure.AddConsole());

            // Daftarkan background service
            services.AddHostedService<KafkaConsumerService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("Configuring the application.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
                logger.LogInformation("Swagger is enabled.");
            }

            app.UseHttpsRedirection();
            logger.LogInformation("HTTPS Redirection is configured.");

            app.UseRouting();
            logger.LogInformation("Routing is configured.");

            app.UseAuthorization();
            logger.LogInformation("Authorization is configured.");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL("/api");
                logger.LogInformation("Endpoints are configured.");
            });

            try
            {
                using var scope = app.ApplicationServices.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                logger.LogInformation("Database ensured to be created.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating the database.");
            }

            logger.LogInformation("Application configuration is complete.");
        }
    }
}
