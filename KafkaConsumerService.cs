using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Belajar2
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly SemaphoreSlim _semaphoreSlim;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConsumer<string, string> consumer, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _consumer = consumer;
            _serviceProvider = serviceProvider;
            _semaphoreSlim = new SemaphoreSlim(1, 1); // Membatasi maksimal 5 thread
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("car-purchases2");

            _logger.LogInformation("Kafka consumer service is starting.");
           
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Attempting to consume Kafka message...");

                    var consumeResult = _consumer.Consume(stoppingToken);

                    _logger.LogInformation("Message consumed successfully.");

                    if (consumeResult != null)
                    {
                        await _semaphoreSlim.WaitAsync(stoppingToken); // Mendapatkan akses ke bagian kritis

                        await ProcessMessageAsync(consumeResult, stoppingToken); // Memproses pesan secara asynchronous
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Consume error: {ex.Error.Reason}");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Service cancellation requested.");
                    break; // Tangani ketika service dihentikan
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming Kafka message");
                }
            }

            _consumer.Close();
            _logger.LogInformation("Kafka consumer service is stopping.");
        }

        private async Task ProcessMessageAsync(ConsumeResult<string, string> consumeResult, CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var purchaseEvent = JsonSerializer.Deserialize<CarPurchase>(consumeResult.Message.Value);

                    // Menampilkan header
                    foreach (var header in consumeResult.Message.Headers)
                    {
                        var headerKey = header.Key;
                        var headerValue = System.Text.Encoding.UTF8.GetString(header.GetValueBytes());
                        _logger.LogInformation($"{headerKey}: {headerValue}");
                    }
                    _logger.LogInformation("Sudah mendapatkan header...");
                    _logger.LogInformation("Akan melakukan pengurangan stock....");
                    var car = await dbContext.Cars.FindAsync(purchaseEvent.CarId);
                    if (car != null && car.Stock >= purchaseEvent.Quantity && car.Stock > 0)
                    {
                        car.Stock -= purchaseEvent.Quantity;
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"Stock Car dengan id {car.Id} berhasil dikurangi sebanyak {purchaseEvent.Quantity}");
                        _logger.LogInformation("Berhasil!!!!");
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Kafka message");
            }
            finally
            {
                _semaphoreSlim.Release(); // Melepaskan akses ke bagian kritis
            }
        }

        public class CarPurchase
        {
            public int Id { get; set; }
            public int CarId { get; set; } // Foreign key to Car
            public int Quantity { get; set; }
            public DateTime PurchaseDate { get; set; }
        }
    }
}
