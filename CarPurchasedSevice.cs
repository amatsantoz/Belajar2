using Library.PublishSubscribe2;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Belajar2
{
    public class CarPurchaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPub2 _pub;

        public CarPurchaseService(ApplicationDbContext context, IPub2 pub)
        {
            _context = context;
            _pub = pub;
        }

        public async Task PurchaseCarAsync(int carId, int quantity)
        {
            try
            {
                OutboxEvent outboxEvent = null;
                CarPurchase carPurchase = null;

                var car = await _context.Cars.FindAsync(carId);

                if (car == null)
                    throw new Exception("Car not found");
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Tidak mengurangi stok di sini
                    // Logika pembelian mobil
                    carPurchase = new CarPurchase
                    {
                        CarId = carId,
                        Quantity = quantity,
                        PurchaseDate = DateTime.Now
                    };

                    _context.CarPurchases.Add(carPurchase);
                    await _context.SaveChangesAsync();

                    outboxEvent = new OutboxEvent
                    {
                        AggregateId = Guid.NewGuid(),
                        EventType = "CarPurchased",
                        Payload = SerializeWithOptions(carPurchase),
                        CreatedAt = DateTime.UtcNow,
                        Processed = false
                    };

                    _context.Outbox.Add(outboxEvent);
                    await _context.SaveChangesAsync();

                    // Commit transaksi basis data
                    await transaction.CommitAsync();

                }
                catch (Exception dbEx)
                {
                    // Rollback transaksi basis data
                    await transaction.RollbackAsync();

                    Console.WriteLine("Database error: " + dbEx.Message);

                    throw;
                }

                try
                {
                    var payload = SerializeWithOptions(carPurchase);
                    var headers = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("EventType", "CarPurchased"),
                        new KeyValuePair<string, string>("CorrelationId", Guid.NewGuid().ToString())
                    };

                    // Gunakan carId sebagai kunci dan sertakan header
                    await _pub.PublishAsync("car-purchases2", carId.ToString(), payload, headers);
                }
                catch (Exception kafkaEx)
                {
                    Console.WriteLine("Kafka error: " + kafkaEx.Message);

                    // Lempar exception agar bisa ditangani di lapisan atas
                    throw;
                }

                #region outboxEvent
                /*// Mulai transaksi baru untuk OutboxEvent
                using var outboxTransaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Buat dan simpan OutboxEvent ke basis data
                    outboxEvent = new OutboxEvent
                    {
                        AggregateId = Guid.NewGuid(),
                        EventType = "CarPurchased",
                        Payload = SerializeWithOptions(carPurchase),
                        CreatedAt = DateTime.UtcNow,
                        Processed = false
                    };

                    _context.Outbox.Add(outboxEvent);
                    await _context.SaveChangesAsync();

                    // Commit transaksi outbox
                    await outboxTransaction.CommitAsync();
                }
                catch (Exception outboxEx)
                {
                    // Rollback transaksi outbox jika ada kesalahan
                    await outboxTransaction.RollbackAsync();
                    Console.WriteLine("Outbox error: " + outboxEx.Message);

                    // Lempar exception agar bisa ditangani di lapisan atas
                    throw;
                }*/
                #endregion 
            }
            catch (Exception ex)
            {
                // Tangani exception umum
                Console.WriteLine("General error: " + ex.Message);
            }
        }

        private string SerializeWithOptions(object obj)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(obj, options);
        }
    }
}
