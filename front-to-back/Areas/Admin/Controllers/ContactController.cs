using front_to_back.Areas.Admin.ViewModels;
using front_to_back.DAL;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new BannerIndexViewModel
            {
              ContactBanners =  await _appDbContext.ContactBanners.ToListAsync()
            };
            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(ContactBanner contactBanner)
        {
            if (!ModelState.IsValid) return View(contactBanner);
            if (!contactBanner.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "The photo must be in image format");
                return View(contactBanner);

            }

            if (contactBanner.Photo.Length / 1024 > 260)
            {
                ModelState.AddModelError("Photo", "The photo over 60kb");
                return View(contactBanner);
            }

            var filename = $"{Guid.NewGuid()}_{contactBanner.Photo.FileName}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", filename);
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await contactBanner.Photo.CopyToAsync(fileStream);
            }
            await _appDbContext.ContactBanners.AddAsync(contactBanner);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");


        }

        [HttpGet]

        public async Task<IActionResult> Details(int id)
        {
            var contactBanner = await _appDbContext.ContactBanners.FindAsync(id);
            if (contactBanner == null) return NotFound();

            return View(contactBanner);
        }
    }
}
