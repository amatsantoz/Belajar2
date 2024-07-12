using Belajar2.GraphQL;
using Microsoft.EntityFrameworkCore;
using Library.PublishSubscribe;
using Belajar2.Handler;
using System.Reflection;
using MediatR;
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
                .AddQueryType<Query>()
                .AddMutationType<Mutation>();

            // Menggunakan extension methods untuk mendaftarkan Kafka Producer dan Consumer
            services.AddKafkaServices();
            services.AddPublishService2<Pub2>("cuaca"); 
            services.AddSubscribeService2<Sub2>("cuaca");

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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL("/api");
            });
            app.UseHttpsRedirection();

            using var scope = app.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            // Menggunakan extension method untuk menjalankan Kafka Consumer di latar belakang
            app.UseKafkaConsumer();
        }
        
    }
}
