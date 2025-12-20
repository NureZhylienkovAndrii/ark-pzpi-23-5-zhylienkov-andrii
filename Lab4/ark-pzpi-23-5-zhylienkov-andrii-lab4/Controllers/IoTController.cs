using AutoRent.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoRent.Controllers
{
    [ApiController]
    [Route("api/iot")]
    public class IoTController : ControllerBase
    {
        private readonly AppDbContext _db;

        public IoTController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings([FromQuery] int days = 30)
        {
            var fromDate = DateTime.Now.AddDays(-days);

            var bookings = await _db.Bookings
                .Where(b => b.Status != "Cancelled" && b.StartDate >= fromDate)
                .Select(b => new
                {
                    b.CarId,
                    b.StartDate,
                    b.EndDate
                })
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPost("analytics")]
        public IActionResult ReceiveAnalytics([FromBody] List<CarBookingStatDto> stats)
        {
            Console.WriteLine("IoT analytics received");

            foreach (var stat in stats)
            {
                Console.WriteLine(
                    $"Car {stat.CarId} — booking frequency {stat.BookingFrequencyPercent}%");
            }

            return Ok();
        }
    }

    public record CarBookingStatDto
    (
        int CarId,
        double BookingFrequencyPercent
    );
}
