using Microsoft.EntityFrameworkCore;

namespace Belajar2.GraphQL
{
    public class Query
    {
        public string Hello() => "Worldd";

        public async Task<List<WeatherForecast>> AllWeatherAsync([Service] ApplicationDbContext context)
        {
            return await context.WeatherForecasts.ToListAsync();
        }

        public async Task<WeatherForecast?> GetWeatherByIdAsync([Service] ApplicationDbContext context, int id)
        {
            var weatherById = await context.WeatherForecasts.FindAsync(id);
            if (weatherById == null)
                return null;
            return weatherById;
        } 
    }
}
