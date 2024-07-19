using Library.PublishSubscribe2;

namespace Belajar2.GraphQL.Cars
{
    public class MutationCar
    {
        public async Task<Car> SaveCarAsync(Car input, [Service] ApplicationDbContext context)
        {
            var newCar = new Car()
            {
                Merk = input.Merk,
                Model = input.Model,
                Stock = input.Stock,
                Year = input.Year,
            };
            context.Cars.Add(newCar);
            await context.SaveChangesAsync();
            //await _pub.PublishAsync("Data Prakiraan Cuaca Terbaru Sudah Ditambahkan");
            //await _pub.PublishAsync("CreateWeather", JsonSerializer.Serialize(newWeather));
            //await _pub.PublishAsync("CreateCar", newCar.Id.ToString(), outboxEvent.Payload, headers);
            return newCar;
        }

        public async Task<Car> UpdateCarAsync(int id, Car input, [Service] ApplicationDbContext context)
        {
            var data = await context.Cars.FindAsync(id);
            if (data == null)
            {
                throw new GraphQLException("Data not found");
            }

            data.Merk = input.Merk;
            data.Model = input.Model;
            data.Stock = input.Stock;
            data.Year = input.Year;
            context.Cars.Update(data);
            await context.SaveChangesAsync();
            //await _pub.PublishAsync($"Data Prakiraan Cuaca Sudah Diupdate");

            return data;
        }

        public async Task<string> DeleteCarAsync([Service] ApplicationDbContext context,
            int id)
        {
            var carToDelete = await context.Cars.FindAsync(id);
            if (carToDelete == null)
            {
                throw new GraphQLException("Data not found");
            }
            context.Cars.Remove(carToDelete);
            await context.SaveChangesAsync();
            // await _pub.PublishAsync($"Data Prakiraan Cuaca Dengan id: {id} Sudah Dihapus");

            return "Record Deleted!";
        }

        public async Task<string> BuyCarAsync (int id, int quantity, [Service] CarPurchaseService carPurchase)
        {
            await carPurchase.PurchaseCarAsync(id, quantity);
            return "ok";

        }
    }
}
