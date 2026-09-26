using FreelancerWorkTracker.Data;
using FreelancerWorkTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class CustomersController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomersController(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    // ==========================================
    // CUSTOMER LIST
    // ==========================================

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var customers = await _context.Customers
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        return View(
            "~/Views/Customers/Index.cshtml",
            customers);
    }


    // ==========================================
    // DETAILS
    // ==========================================

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.UserId == userId);

        if (customer == null)
        {
            return NotFound();
        }

        return View(
            "~/Views/Customers/Details.cshtml",
            customer);
    }


    // ==========================================
    // CREATE GET
    // ==========================================

    public IActionResult Create()
    {
        return View("~/Views/Customers/Create.cshtml");
    }


    // ==========================================
    // CREATE POST
    // ==========================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,Name,Email,Phone,Notes")]
        Customer customer)
    {
        if (ModelState.IsValid)
        {
            // Müşteriyi giriş yapan kullanıcıya bağla
            customer.UserId = _userManager.GetUserId(User)!;

            _context.Add(customer);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Müşteri başarıyla oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }

        return View(
            "~/Views/Customers/Create.cshtml",
            customer);
    }


    // ==========================================
    // EDIT GET
    // ==========================================

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.UserId == userId);

        if (customer == null)
        {
            return NotFound();
        }

        return View(
            "~/Views/Customers/Edit.cshtml",
            customer);
    }


    // ==========================================
    // EDIT POST
    // ==========================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("Id,Name,Email,Phone,Notes")]
        Customer customer)
    {
        if (id != customer.Id)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);


        // Düzenlenmek istenen müşteri gerçekten
        // giriş yapan kullanıcıya mı ait?
        var existingCustomer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.UserId == userId);

        if (existingCustomer == null)
        {
            return NotFound();
        }


        if (ModelState.IsValid)
        {
            try
            {
                // UserId formdan gelmesin.
                // Güvenli şekilde sunucudan ekliyoruz.
                customer.UserId = userId!;

                _context.Update(customer);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Müşteri başarıyla güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.Id, userId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }


        return View(
            "~/Views/Customers/Edit.cshtml",
            customer);
    }


    // ==========================================
    // DELETE GET
    // ==========================================

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.UserId == userId);

        if (customer == null)
        {
            return NotFound();
        }

        return View(
            "~/Views/Customers/Delete.cshtml",
            customer);
    }


    // ==========================================
    // DELETE POST
    // ==========================================

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {

        var userId = _userManager.GetUserId(User);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.UserId == userId);

        if (customer == null)
        {
            return NotFound();
        }



        _context.Customers.Remove(customer);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Müşteri başarıyla silindi.";

        return RedirectToAction(nameof(Index));
    }


    // ==========================================
    // CUSTOMER EXISTS
    // ==========================================

    private bool CustomerExists(int? id, string? userId)
    {
        return _context.Customers.Any(c =>
            c.Id == id &&
            c.UserId == userId);
    }
}