using FreelancerWorkTracker.Data;
using FreelancerWorkTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelancerWorkTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var today = DateTime.Today;


            // TOPLAM PROJE
            ViewBag.TotalProjects = await _context.Projects
                .CountAsync(p => p.UserId == userId);


            // AKTİF PROJE
            ViewBag.ActiveProjects = await _context.Projects
                .CountAsync(p =>
                    p.UserId == userId &&
                    p.Status == ProjectStatus.DevamEdiyor);


            // TOPLAM MÜŞTERİ
            ViewBag.TotalCustomers = await _context.Customers
                .CountAsync(c => c.UserId == userId);


            // TOPLAM TAHSİLAT
            ViewBag.TotalRevenue = await _context.Projects
                .Where(p => p.UserId == userId)
                .SumAsync(p => (decimal?)p.PaidAmount) ?? 0;


            // BEKLEYEN ÖDEME
            ViewBag.PendingPayment = await _context.Projects
                .Where(p => p.UserId == userId)
                .SumAsync(p => (decimal?)(p.Price - p.PaidAmount)) ?? 0;


            // GECİKEN PROJELER
            ViewBag.OverdueProjects = await _context.Projects
                .CountAsync(p =>
                    p.UserId == userId &&
                    p.Deadline.Date < today &&
                    p.Status != ProjectStatus.Tamamlandi &&
                    p.Status != ProjectStatus.IptalEdildi);


            // YAKLAŞAN TESLİMLER
            var upcomingProjects = await _context.Projects
                .Include(p => p.Customer)
                .Where(p =>
                    p.UserId == userId &&
                    p.Deadline.Date >= today &&
                    p.Status != ProjectStatus.Tamamlandi &&
                    p.Status != ProjectStatus.IptalEdildi)
                .OrderBy(p => p.Deadline)
                .Take(5)
                .ToListAsync();


            // SON PROJELER
            var recentProjects = await _context.Projects
                .Include(p => p.Customer)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToListAsync();


            ViewBag.UpcomingProjects = upcomingProjects;

            return View(recentProjects);
        }
    }
}