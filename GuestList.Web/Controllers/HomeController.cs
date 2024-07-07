using GuestList.Web.Data;
using GuestList.Web.Models.Entities;
using GuestList.Web.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace GuestList.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        //Rejestracja u¿ytkowników

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string login, string password)
        {
            if (password.Length < 5 || password.Length > 13)
            {
                ViewBag.ErrorMessage = "Has³o powinno zawieraæ miêdzy 5 a 13 znaków";
                return View();
            }

            var user = new User { Login = login, Password = password };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            TempData["RegisterMessage"] = "Konto zosta³o utworzone";
            return RedirectToAction("Login");
        }


        //Logowanie

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.RegisterMessage = TempData["RegisterMessage"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                ViewBag.ErrorMessage = "B³êdne dane";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("List", "Guests");
        }


        //Wylogowywanie

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
