using front_to_back.Areas.Admin.ViewModels;
using front_to_back.DAL;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RecentWorkController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public RecentWorkController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new RecentWorkViewModel
            {
                RecentWorkComponents = await _appDbContext.RecentWorkComponents.ToListAsync()
            };


            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RecentWorkComponent recentWorkComponent)
        {
            if (!ModelState.IsValid) return View(recentWorkComponent);
            bool isExixst = await _appDbContext.RecentWorkComponents.AnyAsync(c => c.Title.ToLower().Trim() == recentWorkComponent.Title.ToLower().Trim());

            if (isExixst)
            {
                ModelState.AddModelError("Title", "Bu adda artiq movcuddur");
                return View(recentWorkComponent);
            }

            await _appDbContext.RecentWorkComponents.AddAsync(recentWorkComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }
    }
}

