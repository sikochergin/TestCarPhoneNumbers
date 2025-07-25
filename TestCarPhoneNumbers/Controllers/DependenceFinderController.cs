using Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestCarPhoneNumbers.Models;

namespace TestCarPhoneNumbers.Controllers
{
    public class DependenceFinderController : Controller
    {
        private readonly ApplicationContext context;

        public DependenceFinderController(ApplicationContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NumbersShower() 
        { 
            if (TempData["NumbersToShow"] is string json)
            {
                var numbers = JsonSerializer.Deserialize<List<NumberToShow>>(json);
                ViewData["NumbersToShow"] = numbers;
            }
            ViewBag.CarNumber = TempData["CarNumber"];
            ViewBag.FailReason = TempData["FailReason"];
            return View(); 
        }

        [HttpPost]
        public IActionResult FindNumbers(string number)
        {
            try
            {
                TempData["CarNumber"] = number;
                List<NumberToShow> numbers = new List<NumberToShow>();
                var car = context.cars.FirstOrDefault(x => x.Number == number);
                if (car == null) 
                {
                    TempData["FailReason"] = "Номер в базе не найден";
                    return Json(new { status = true, message = "Номер в базе не найден", redirectUrl = Url.Action("NumbersShower") });
                }
                Guid carId = car.Id;
                var dependenciesWithCarId = context.dependencies.Where(x => x.CarId == carId && x.IsActive).ToList();
                if (dependenciesWithCarId.Count == 0) 
                {
                    TempData["FailReason"] = "К данному номеру машины не привязан ни один телефон";
                    return Json(new { status = true, message = "Номер найден в базе, но к нему не привязан ни один телефон", redirectUrl = Url.Action("NumbersShower") });
                }
                foreach(var dep in dependenciesWithCarId)
                {
                    var phoneId = dep.PhoneId;
                    if (phoneId == Guid.Empty)
                    {
                        continue;
                    }
                    var phone = context.phones.FirstOrDefault(x => x.Id == phoneId && x.IsActive);
                    if (phone == null)
                    {
                        continue;
                    }
                    NumberToShow newNumber = new NumberToShow();
                    newNumber.Number = phone.Number;
                    newNumber.CreationDate = dep.CreationDateTime;
                    newNumber.IsOwner = dep.IsOwner;
                    numbers.Add(newNumber);
                }
                TempData["NumbersToShow"] = JsonSerializer.Serialize(numbers);
                return Json(new { status = true, message = "Номера найдены", redirectUrl = Url.Action("NumbersShower") });
            }
            catch (Exception ex) 
            { 
                return Json(new { status = false, message = ex.Message }); 
            }
            
        }
    }
}
