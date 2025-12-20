using AutoRent.Data;
using AutoRent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoRent.Pages.Admin
{
    public class BookingsModel : PageModel
    {
        private readonly AppDbContext _db;

        public BookingsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Booking> Bookings { get; set; }

        [BindProperty]
        public int BookingId { get; set; }

        [BindProperty]
        public string NewStatus { get; set; }

        public async Task OnGetAsync()
        {
            Bookings = await _db.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var booking = await _db.Bookings.FindAsync(BookingId);
            if (booking != null)
            {
                booking.Status = NewStatus;
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
