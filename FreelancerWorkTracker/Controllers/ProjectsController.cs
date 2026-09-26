using FreelancerWorkTracker.Data;
using FreelancerWorkTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProjectsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectsController(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    // ==========================================
    // PROJECT LIST
    // ==========================================

    public async Task<IActionResult> Index(
        string? search,
        ProjectStatus? status)
    {
        var userId = _userManager.GetUserId(User);

        var projects = _context.Projects
            .Include(p => p.Customer)
            .Where(p => p.UserId == userId)
            .AsQueryable();


        // ARAMA
        if (!string.IsNullOrWhiteSpace(search))
        {
            projects = projects.Where(p =>
                p.Name.Contains(search) ||
                (p.Customer != null &&
                 p.Customer.Name.Contains(search)));
        }


        // DURUM FİLTRESİ
        if (status.HasValue)
        {
            projects = projects.Where(
                p => p.Status == status.Value);
        }


        ViewBag.Search = search;
        ViewBag.Status = status;


        var result = await projects
            .OrderByDescending(p => p.Id)
            .ToListAsync();


        return View(
            "~/Views/Projects/Index.cshtml",
            result);
    }


    // ==========================================
    // DETAILS
    // ==========================================

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();


        var userId = _userManager.GetUserId(User);


        var project = await _context.Projects
            .Include(p => p.Customer)
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);


        if (project == null)
            return NotFound();


        return View(
            "~/Views/Projects/Details.cshtml",
            project);
    }


    // ==========================================
    // CREATE GET
    // ==========================================

    public IActionResult Create()
    {
        var userId = _userManager.GetUserId(User);


        ViewData["CustomerId"] = new SelectList(
            _context.Customers
                .Where(c => c.UserId == userId),
            "Id",
            "Name");


        return View(
            "~/Views/Projects/Create.cshtml");
    }


    // ==========================================
    // CREATE POST
    // ==========================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,Name,Description,Price,PaidAmount,StartDate,Deadline,Status,CustomerId")]
        Project project)
    {
        var userId = _userManager.GetUserId(User);


        // Seçilen müşteri gerçekten bu kullanıcıya mı ait?
        var customerExists = await _context.Customers
            .AnyAsync(c =>
                c.Id == project.CustomerId &&
                c.UserId == userId);


        if (!customerExists)
        {
            return Forbid();
        }


        if (ModelState.IsValid)
        {
            // Projeyi giriş yapan kullanıcıya bağla
            project.UserId = userId!;


            _context.Add(project);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Proje başarıyla oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }


        // Formda hata varsa dropdown'u tekrar oluştur
        ViewData["CustomerId"] = new SelectList(
            _context.Customers
                .Where(c => c.UserId == userId),
            "Id",
            "Name",
            project.CustomerId);


        return View(
            "~/Views/Projects/Create.cshtml",
            project);
    }


    // ==========================================
    // EDIT GET
    // ==========================================

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();


        var userId = _userManager.GetUserId(User);


        var project = await _context.Projects
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);


        if (project == null)
            return NotFound();


        ViewData["CustomerId"] = new SelectList(
            _context.Customers
                .Where(c => c.UserId == userId),
            "Id",
            "Name",
            project.CustomerId);


        return View(
            "~/Views/Projects/Edit.cshtml",
            project);
    }


    // ==========================================
    // EDIT POST
    // ==========================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("Id,Name,Description,Price,PaidAmount,StartDate,Deadline,Status,CustomerId")]
        Project project)
    {
        if (id != project.Id)
        {
            return NotFound();
        }


        var userId = _userManager.GetUserId(User);


        // Bu proje gerçekten giriş yapan kullanıcıya mı ait?
        var existingProject = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);


        if (existingProject == null)
        {
            return NotFound();
        }


        // Seçilen müşteri de bu kullanıcıya mı ait?
        var customerExists = await _context.Customers
            .AnyAsync(c =>
                c.Id == project.CustomerId &&
                c.UserId == userId);


        if (!customerExists)
        {
            return Forbid();
        }


        if (ModelState.IsValid)
        {
            try
            {
                // UserId formdan değil sunucudan geliyor
                project.UserId = userId!;


                _context.Update(project);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Proje başarıyla güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.Id, userId))
                {
                    return NotFound();
                }

                throw;
            }


            return RedirectToAction(nameof(Index));
        }


        ViewData["CustomerId"] = new SelectList(
            _context.Customers
                .Where(c => c.UserId == userId),
            "Id",
            "Name",
            project.CustomerId);


        return View(
            "~/Views/Projects/Edit.cshtml",
            project);
    }


    // ==========================================
    // DELETE GET
    // ==========================================

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();


        var userId = _userManager.GetUserId(User);


        var project = await _context.Projects
            .Include(p => p.Customer)
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);


        if (project == null)
            return NotFound();


        return View(
            "~/Views/Projects/Delete.cshtml",
            project);
    }


    // ==========================================
    // DELETE POST
    // ==========================================

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var userId = _userManager.GetUserId(User);


        var project = await _context.Projects
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);


        if (project == null)
        {
            return NotFound();
        }


        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Proje başarıyla silindi.";

        return RedirectToAction(nameof(Index));
    }


    // ==========================================
    // PROJECT EXISTS
    // ==========================================

    private bool ProjectExists(int? id, string? userId)
    {
        return _context.Projects.Any(p =>
            p.Id == id &&
            p.UserId == userId);
    }
}