namespace AutoRent.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public int StatusId { get; set; }

        public CarStatus Status { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = new();
    }
}
