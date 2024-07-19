using Belajar2.GraphQL.Cars;
using Microsoft.EntityFrameworkCore;
using Library.PublishSubscribe;
using Belajar2.Handler;
using System.Reflection;
using MediatR;
using Library.PublishSubscribe2;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

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
                .AddType<CarType>()
                .AddType<CarInputType>();


            // Menggunakan extension methods untuk mendaftarkan Kafka Producer dan Consumer
            services.AddKafkaServices();
            //services.AddPublishService2<Pub2>("car-purchases2");
            //services.AddSubscribeService2<Sub2>("cuaca");

            services.AddScoped<CarPurchaseService>();
                    
            // Tambahkan logging untuk diagnosa
            services.AddLogging(configure => configure.AddConsole());

            // Konfigurasi Kafka consumer
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "127.0.0.100:9092",
                GroupId = "car-purchases-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            var adminBuilder = new AdminClientBuilder(consumerConfig);

            var adminClient = adminBuilder.Build();

            try
            {
                adminClient.CreateTopicsAsync(new List<TopicSpecification>()
                {
                    new()
                    {
                        Name = "car-purchases2",
                        NumPartitions = 1,
                        ReplicationFactor = 1
                    }
                }).GetAwaiter().GetResult();

            }
            catch (CreateTopicsException ex)
            {
                Console.WriteLine("Topic already exist");
            }

            // Daftarkan Kafka consumer

            services.AddSingleton<IConsumer<string, string>>(new ConsumerBuilder<string, string>(consumerConfig).Build());

            // Daftarkan background service
            services.AddHostedService<KafkaConsumerService>();

            #region trialKafka
            /*services.AddMediatR(typeof(AddCommandHandler).GetTypeInfo().Assembly);

            services.AddPublishSubscribeService();
            services.AddKafkaProducer<string, WeatherForecast>(p =>
            {
                p.Topic = "users";
                p.BootstrapServers = "localhost:9092";
            });*/
            #endregion
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
