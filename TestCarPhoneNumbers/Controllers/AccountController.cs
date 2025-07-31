using Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestCarPhoneNumbers.Models;

namespace TestCarPhoneNumbers.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext context;

        public AccountController(ApplicationContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            // 1. Получаем Id из клаймов
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Challenge(); // нет клайма – редирект на логин

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            // 2. Достаём пользователя из БД
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            // 3. Кладём телефон в ViewBag
            ViewBag.PhoneNumber = user.Phone;

            ViewData["AccountNumbersToShow"] = GetUsersNumbers(userId);

            return View();
        }


        public List<NumberToShow> GetUsersNumbers(Guid userId)
        {
            List<NumberToShow> numberToShows = new List<NumberToShow>();
            
            var user = context.users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return numberToShows;

            var phoneId = user.PhoneId;

            var dependencies = context.dependencies.Where(x => x.PhoneId == phoneId && x.IsActive).ToList();
            foreach( var dependency in dependencies)
            {
                var car = context.cars.FirstOrDefault(x => x.Id == dependency.CarId);
                if (car == null)
                {
                    continue;
                }
                var carNumber = car.Number;
                var nts = new NumberToShow{ CreationDate = dependency.CreationDateTime, IsOwner = dependency.IsOwner, Number = carNumber };
                numberToShows.Add(nts);
            }
            return numberToShows;
        }
    }
}
