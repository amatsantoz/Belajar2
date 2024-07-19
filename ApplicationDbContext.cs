using Microsoft.EntityFrameworkCore;

namespace Belajar2
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<OutboxEvent> Outbox { get; set; }
        public DbSet<CarPurchase> CarPurchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeatherForecast>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<WeatherForecast>()
                .Property(x => x.Date)
                .IsRequired();
            modelBuilder.Entity<WeatherForecast>()
                .Property(x => x.TemperatureC)
                .IsRequired();
            modelBuilder.Entity<WeatherForecast>()
                .Property(x => x.Summary)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Car>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Car>()
                .Property(x => x.Merk)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<Car>()
                .Property(x => x.Model)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<OutboxEvent>()
                .Property(x => x.EventType)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<CarPurchase>()
                .HasKey(cp => cp.Id);
            // Configure the relationship between Car and CarPurchase
            modelBuilder.Entity<CarPurchase>()
                .HasOne(cp => cp.Car)
                .WithMany(c => c.CarPurchases)
                .HasForeignKey(cp => cp.CarId);

        }
    }
}
