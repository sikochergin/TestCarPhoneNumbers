using Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestCarPhoneNumbers.Services;
using Data.Models;

namespace TestCarPhoneNumbers.Controllers
{
    public class LoginController : Controller
    {
        private const string TestCode = "999999";
        private readonly ApplicationContext context;
        private readonly INotificationService notifier;

        public LoginController(ApplicationContext context, INotificationService notifier)
        {
            this.context = context;
            this.notifier = notifier;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPhone(string number)
        {
            try
            {
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

        public string TryNormalizePhone(string maskedInput)
        {
            if (String.IsNullOrEmpty(maskedInput))
                return "";
            var digits = new string(maskedInput.Where(char.IsDigit).ToArray());
            if (digits.Length == 11 && digits.StartsWith("7"))
            {
                TempData["PhoneForVerification"] = digits;
                return digits; // например "+79161234567"
            }
            return "";
        }

        // POST /Login/VerifyCode
        // Проверяет код и, если всё ок, создаёт/находит пользователя, ставит куки и возвращает HTTP 200
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> VerifyCode(string code)
        {
            var phone = TempData["PhoneForVerification"] as string;
            if (phone == null)
            {
                TempData["PhoneForVerification"] = phone;
                return Json(new { status = false, message = "Ошибка 1" }); // например, 400
            }

            if (code != TestCode)
            {
                TempData["PhoneForVerification"] = phone;
                return Json(new { status = false, message = "Неверный код" });
            }
            // ищем или создаём пользователя
            var user = context.users.FirstOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                Phone? cPhone = context.phones.FirstOrDefault(p => p.Number == phone);
                if (cPhone == null)
                {
                    var resultPhoneAdding = await AddPhone(phone);
                    cPhone = context.phones.FirstOrDefault(p => p.Number == phone);
                }
                user = new User { Id = Guid.NewGuid(), Phone = phone, IsActive = true, CreationDateTime = DateTime.UtcNow, PhoneId = cPhone.Id};
                context.users.Add(user);
                await context.SaveChangesAsync();
            }

            // ставим cookie-auth
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return Json(new { status = true, message = "Успешный вход" });
        }

        // при желании: выход
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}
