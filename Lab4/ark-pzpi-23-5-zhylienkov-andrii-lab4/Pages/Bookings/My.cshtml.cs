using AutoRent.Data;
using AutoRent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoRent.Pages.Bookings
{
    public class MyModel : PageModel
    {
        private readonly AppDbContext _db;

        public MyModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Booking> Bookings { get; set; }
        public string Message { get; set; }

        [BindProperty]
        public int BookingId { get; set; }

        private int? GetUserId()
        {
            var claim = User.FindFirst("UserId")?.Value;
            return claim == null ? null : int.Parse(claim);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            await LoadBookings(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            var booking = await _db.Bookings
                .FirstOrDefaultAsync(b => b.Id == BookingId && b.UserId == userId);

            if (booking == null)
            {
                Message = "Бронювання не знайдено.";
                await LoadBookings(userId.Value);
                return Page();
            }

            if (booking.Status == "Cancelled")
            {
                Message = "Це бронювання вже скасоване.";
                await LoadBookings(userId.Value);
                return Page();
            }

            booking.Status = "Cancelled";
            await _db.SaveChangesAsync();

            Message = "Бронювання скасовано.";
            await LoadBookings(userId.Value);
            return Page();
        }

        private async Task LoadBookings(int userId)
        {
            Bookings = await _db.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
    }
}
