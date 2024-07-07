using GuestList.Web.Data;
using GuestList.Web.Models.Entities;
using GuestList.Web.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GuestList.Web.Controllers
{
    public class GuestsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public GuestsController(ApplicationDbContext dbContext) 
        { 
            this.dbContext = dbContext;
        }


        //Wyciąganie listy gości
        public async Task<IActionResult> List(string sortOrder)
        {
            //Pobieranie danych według ID użytkownika
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.UserId = userId;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PhoneSortParm = sortOrder == "Phone" ? "phone_desc" : "Phone";
            ViewBag.ConfirmedSortParm = sortOrder == "Confirmed" ? "confirmed_desc" : "Confirmed";

            var guests = from g in dbContext.Guests
                         where g.UserId == userId
                         select g;

            switch (sortOrder)
            {
                case "name_desc":
                    guests = guests.OrderByDescending(g => g.Name);
                    break;
                case "Phone":
                    guests = guests.OrderBy(g => g.PhoneNumber);
                    break;
                case "phone_desc":
                    guests = guests.OrderByDescending(g => g.PhoneNumber);
                    break;
                case "Confirmed":
                    guests = guests.OrderBy(g => g.Confirmed);
                    break;
                case "confirmed_desc":
                    guests = guests.OrderByDescending(g => g.Confirmed);
                    break;
                default:
                    guests = guests.OrderBy(g => g.Name);
                    break;
            }

            return View(await guests.ToListAsync());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }



        //Dodawanie gości do listy
        [HttpPost]
        public async Task<IActionResult> Add(Guest model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            model.UserId = userId;

            dbContext.Guests.Add(model);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }



        //Edycja gości

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var guest = await dbContext.Guests.FindAsync(id);
            if (guest == null || guest.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return NotFound();
            }

            return View(guest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guest model)
        {
            if (model.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return Unauthorized();
            }

            dbContext.Guests.Update(model);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
        public async Task<IActionResult> Cancel()
        {
            return RedirectToAction("List");
        }



        //Usuwanie gości z listy
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var guest = await dbContext.Guests.FindAsync(id);
            if (guest == null || guest.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return NotFound();
            }

            dbContext.Guests.Remove(guest);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
    }
}
