using AutoRent.Data;
using AutoRent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AutoRent.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;

        public RegisterModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public RegisterInput Input { get; set; }

        public string ErrorMessage { get; set; }

        public class RegisterInput
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Input.Password != Input.ConfirmPassword)
            {
                ErrorMessage = "Паролі не співпадають.";
                return Page();
            }

            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
            if (existingUser != null)
            {
                ErrorMessage = "Користувач з таким email вже існує.";
                return Page();
            }

            var user = new User
            {
                FullName = Input.FullName,
                Email = Input.Email,
                PasswordHash = HashPassword(Input.Password),
                Role = "User",
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToPage("/Cars/Index");
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
