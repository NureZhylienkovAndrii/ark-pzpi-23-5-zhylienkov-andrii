using AutoRent.Data;
using AutoRent.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoRent.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly AppDbContext _db;

        public UsersModel(AppDbContext db)
        {
            _db = db;
        }

        public List<User> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _db.Users
                .Where(u => u.Role == "User")
                .ToListAsync();
        }
    }
}
