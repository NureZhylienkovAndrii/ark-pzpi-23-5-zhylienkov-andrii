namespace AutoRent.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Booked";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Car Car { get; set; } = null!;
    }
}
