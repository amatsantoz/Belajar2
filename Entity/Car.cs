namespace Belajar2
{
    public class Car
    {
        public int Id { get; set; }
        public string Merk { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Stock { get; set; }

        // Navigation property for related CarPurchases
        public ICollection<CarPurchase> CarPurchases { get; set; } = new List<CarPurchase>();
    }
}
