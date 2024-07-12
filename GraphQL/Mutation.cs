using Library.PublishSubscribe2;
using System.Text.Json;

namespace Belajar2.GraphQL
{
    public class Mutation
    {
        private readonly IPub2 _pub;

        public Mutation(IPub2 pub) 
        { 
            _pub = pub; 
        }
        public async Task<WeatherForecast> SaveWeatherAsync([Service] ApplicationDbContext context,
            WeatherForecast newWeather)
        {
            context.WeatherForecasts.Add(newWeather);
            await context.SaveChangesAsync();
            await _pub.PublishAsync("Data Prakiraan Cuaca Terbaru Sudah Ditambahkan");
            //await _pub.PublishAsync("CreateWeather", JsonSerializer.Serialize(newWeather));
            return newWeather;
        }

        public async Task<WeatherForecast> UpdateWeatherForcastAsync([Service] ApplicationDbContext context,
            WeatherForecast updateWeather)
        {
            context.WeatherForecasts.Update(updateWeather);
            await context.SaveChangesAsync();
            await _pub.PublishAsync($"Data Prakiraan Cuaca Sudah Diupdate");

            return updateWeather;
        }


        public async Task<WeatherForecast> SaveWeather2Async(WeatherForecast input, [Service] ApplicationDbContext context)
        {
            var newWeather = new WeatherForecast()
            {
                Date = input.Date,
                Summary = input.Summary,
                TemperatureC = input.TemperatureC,
            };
            context.WeatherForecasts.Add(newWeather);
            await context.SaveChangesAsync();
            await _pub.PublishAsync("Data Prakiraan Cuaca Terbaru Sudah Ditambahkan");
            return newWeather;
        }

        public async Task<WeatherForecast> UpdateWeather2Async(int id, WeatherForecast input, [Service] ApplicationDbContext context)
        {
            var data = await context.WeatherForecasts.FindAsync(id);
            if (data == null)
            {
                throw new GraphQLException("Data not found");
            }

            data.TemperatureC = input.TemperatureC;
            data.Summary = input.Summary;
            await context.SaveChangesAsync();
            await _pub.PublishAsync($"Data Prakiraan Cuaca Dengan id: {id} Sudah Diupdate");
            return data;
        }

        
        public async Task<string> DeleteWeatherForcastAsync([Service] ApplicationDbContext context,
            int id)
        {
            var weatherToDelete = await context.WeatherForecasts.FindAsync(id);
            if (weatherToDelete == null)
            {
                return "Invalid Operation";
            }
            context.WeatherForecasts.Remove(weatherToDelete);
            await context.SaveChangesAsync();
            await _pub.PublishAsync($"Data Prakiraan Cuaca Dengan id: {id} Sudah Dihapus");

            return "Record Deleted!";
        }
    }
}
