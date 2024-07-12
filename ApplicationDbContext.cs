using Microsoft.EntityFrameworkCore;

namespace Belajar2
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }

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
        }
    }
}
