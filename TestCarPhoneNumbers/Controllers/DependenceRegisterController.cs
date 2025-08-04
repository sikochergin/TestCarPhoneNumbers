using Microsoft.AspNetCore.Mvc;
using Data.Models;
using System;
using Data;
using System.Security.Claims;

namespace TestCarPhoneNumbers.Controllers
{
    public class DependenceRegisterController : Controller
    {
        private readonly ApplicationContext context;

        public DependenceRegisterController(ApplicationContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Challenge();
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            ViewBag.PhoneNumber = user.Phone;

            return View();
        }

        //все методы с телефонным номером

        public IActionResult PhoneAdding()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Challenge();
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            ViewBag.PhoneNumber = user.Phone;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPhone(string number)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Challenge();
                var user = context.users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return NotFound();
                ViewBag.PhoneNumber = user.Phone;

                var phones = context.phones.ToList();
                bool alreadyExists = phones.Where(x => x.Number == number && x.IsActive).Any();
                if (alreadyExists)
                {
                    return Json(new { status = false, message = "Номер уже в базе" });
                }
                context.phones.Add(new Phone { Id = Guid.NewGuid(), Number = number, IsActive = true, CreationDateTime = DateTime.UtcNow });
                await context.SaveChangesAsync();
                return Json(new { status = true, message = "Номер добавлен" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
            
        }

        public IActionResult GetAllPhones()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Challenge();
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            ViewBag.PhoneNumber = user.Phone;

            var phones = context.phones.ToList(); 
            return Json(phones);
        }

        //все методы с номером машины

        public IActionResult CarAdding()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Challenge();
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            ViewBag.PhoneNumber = user.Phone;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddCar(string number)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Challenge();
                var user = context.users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return NotFound();
                ViewBag.PhoneNumber = user.Phone;

                var cars = context.cars.ToList();
                bool alreadyExists = cars.Where(x => x.Number == number && x.IsActive).Any();
                if (alreadyExists)
                {
                    return Json(new { status = false, message = "Номер уже в базе" });
                }
                context.cars.Add(new Car { Id = Guid.NewGuid(), Number = number, CreationDateTime = DateTime.UtcNow, IsActive = true });
                await context.SaveChangesAsync();
                return Json(new { status = true, message = "Номер добавлен" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }

        }

        public IActionResult GetAllCars()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Challenge();
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            ViewBag.PhoneNumber = user.Phone;

            var cars = context.cars.ToList();
            return Json(cars);
        }

        //все методы с Зависимостями

        public IActionResult DependenceAdding()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Challenge();
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            ViewBag.PhoneNumber = user.Phone;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDependence(string phone, string car, bool isOwner=false)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Challenge();
                var user = context.users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return NotFound();
                ViewBag.PhoneNumber = user.Phone;

                var phones = context.phones.ToList();
                var cars = context.cars.ToList();
                var dependences = context.dependencies.ToList();

                bool flagOwnerExists = false;

                var phoneObj = phones.FirstOrDefault(x => x.Number == phone);
                var carObj = cars.FirstOrDefault(x => x.Number == car);
                if (carObj == null) 
                {
                    await AddCar(car);
                    cars = context.cars.ToList();
                    carObj = cars.FirstOrDefault(x => x.Number == car);
                }
                if (phoneObj == null)
                {
                    await AddPhone(phone);
                    phones = context.phones.ToList();
                    phoneObj = phones.FirstOrDefault(x => x.Number == phone);
                }


                if (carObj == null || phoneObj == null)
                {
                    return Json(new { status = false, message = "Ошибка добавления в базу" });
                }

                Guid phoneId = phoneObj.Id;
                Guid carId = carObj.Id;

                if (phoneId == Guid.Empty || carId == Guid.Empty) 
                {
                    return Json(new { status = false, message = "Ошибка записи номера в базе" });
                }

                if (isOwner)
                {
                    foreach (var c in dependences.Where(x => x.CarId == carId && x.IsActive))
                    {
                        if (c.IsOwner)
                        {
                            flagOwnerExists = true;
                            break;
                        }
                    }
                    if (flagOwnerExists)
                    {
                        return Json(new { status = false, message = "Владелец авто уже указан" });
                    }
                }
                var alrExists = context.dependencies.FirstOrDefault(x => x.CarId == carId && x.PhoneId == phoneId && x.IsActive);
                if (alrExists != null)
                {
                    return Json(new { status = false, message = "Эти номера уже связаны" });
                }
                context.dependencies.Add(new Dependence { Id = Guid.NewGuid(), CarId = carId, PhoneId = phoneId, IsActive = true, IsOwner = isOwner, CreationDateTime = DateTime.UtcNow });
                await context.SaveChangesAsync();

                return Json(new { status = true, message = "Номера связаны" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }

        }

        public IActionResult GetAllDependencies()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Challenge();
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            ViewBag.PhoneNumber = user.Phone;

            var dependences = context.dependencies.ToList();
            return Json(dependences);
        }

        async public Task<IActionResult> DeleteDependence(string phone, string car)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Challenge();
                var user = context.users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return NotFound();
                ViewBag.PhoneNumber = user.Phone;

                var carCon = context.cars.FirstOrDefault(x => x.Number == car && x.IsActive);
                var phoneCon = context.phones.FirstOrDefault(x => x.Number == phone && x.IsActive);

                if (carCon == null || phoneCon == null)
                {
                    return Json(new { status = false, message = "Номер не найден в базе" });
                }
                var dep = context.dependencies.FirstOrDefault(x => x.CarId == carCon.Id && x.PhoneId == phoneCon.Id);
                if (dep == null)
                {
                    return Json(new { status = false, message = "Связь не найдена в базе" });
                }
                dep.IsActive = false;
                await context.SaveChangesAsync();
                return Json(new { status = true, message = "Номер отвязан" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        public Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null) return Guid.Empty;

            if (!Guid.TryParse(userIdClaim, out var userId)) return Guid.Empty;

            return userId;
        }
    }
}
