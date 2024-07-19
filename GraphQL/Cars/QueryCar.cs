using Microsoft.EntityFrameworkCore;

namespace Belajar2.GraphQL.Cars
{
    public class QueryCar
    {
        public string Hello() => "Worldd";

        public async Task<List<Car>> AllCarsAsync([Service] ApplicationDbContext context)
        {
            return await context.Cars.ToListAsync();
        }

        public async Task<Car?> GetCarByIdAsync([Service] ApplicationDbContext context, int id)
        {
            var carById = await context.Cars.FindAsync(id);
            if (carById == null)
                return null;
            return carById;
        }
    }
}
