using AutoRent.Data;
using AutoRent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoRent.Pages.Cars
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Car> Cars { get; set; }
        public string Message { get; set; }

        [BindProperty]
        public int CarId { get; set; }

        [BindProperty]
        public DateTime StartDate { get; set; }

        [BindProperty]
        public DateTime EndDate { get; set; }

        public async Task OnGetAsync()
        {
            Cars = await _db.Cars.Include(c => c.Status).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            if (StartDate >= EndDate)
            {
                Message = "Неправильний проміжок дат.";
                await OnGetAsync();
                return Page();
            }

            var car = await _db.Cars.FindAsync(CarId);
            if (car == null)
            {
                Message = "Автомобіль не знайдено.";
                await OnGetAsync();
                return Page();
            }

            if (car.StatusId != 1)
            {
                Message = "Це авто наразі недоступне для бронювання.";
                await OnGetAsync();
                return Page();
            }

            var exists = await _db.Bookings.AnyAsync(b =>
                b.CarId == CarId &&
                b.Status != "Cancelled" &&
                ((StartDate >= b.StartDate && StartDate < b.EndDate) ||
                 (EndDate > b.StartDate && EndDate <= b.EndDate) ||
                 (StartDate <= b.StartDate && EndDate >= b.EndDate))
            );

            if (exists)
            {
                Message = "Автомобіль вже заброньований у цей період.";
                await OnGetAsync();
                return Page();
            }

            var booking = new Booking
            {
                CarId = CarId,
                UserId = int.Parse(userId),
                StartDate = StartDate,
                EndDate = EndDate,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            Message = "Бронювання успішно створено!";
            await OnGetAsync();
            return Page();
        }
    }
}
