namespace Belajar2
{
    public class CarPurchase
    {
        public int Id { get; set; }
        public int CarId { get; set; } // Foreign key to Car
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }

        // Navigation property to the related Car
        public Car Car { get; set; }
    }
}
