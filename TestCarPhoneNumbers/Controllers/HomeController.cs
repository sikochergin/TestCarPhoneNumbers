using Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TestCarPhoneNumbers.Models;

namespace TestCarPhoneNumbers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
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

        public IActionResult Privacy()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Challenge();
            var user = context.users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            ViewBag.PhoneNumber = user.Phone;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
