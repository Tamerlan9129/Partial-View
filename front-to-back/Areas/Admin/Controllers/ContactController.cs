using front_to_back.Areas.Admin.ViewModels;
using front_to_back.Areas.Admin.ViewModels.ContactBanner;
using front_to_back.DAL;
using front_to_back.Helpers;
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
        private readonly IFilService _filService;

        public ContactController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment,IFilService filService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _filService = filService;
        }
        public async Task<IActionResult> Index()
        {
            var model = new BannerIndexViewModel
            {
                ContactBanners = await _appDbContext.ContactBanners.ToListAsync()
            };
            return View(model);
        }
        #region Create
        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(ContactBanner contactBanner)
        {
            if (!ModelState.IsValid) return View(contactBanner);
            if (!_filService.IsImage(contactBanner.Photo))
            {
                ModelState.AddModelError("Photo", "The photo must be in image format");
                return View(contactBanner);

            }
            //if (!contactBanner.Photo.ContentType.Contains("image/"))
            //{
            //    ModelState.AddModelError("Photo", "The photo must be in image format");
            //    return View(contactBanner);

            //}
            int maxSize = 300;
            if (!_filService.CheckSize(contactBanner.Photo,maxSize))
            {
                ModelState.AddModelError("Photo", $"The photo over {maxSize}kb");
                return View(contactBanner);
            }
            //if (contactBanner.Photo.Length / 1024 > 260)
            //{
            //    ModelState.AddModelError("Photo", "The photo over 60kb");
            //    return View(contactBanner);
            //}
            //var filename = $"{Guid.NewGuid()}_{contactBanner.Photo.FileName}";
            //var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", filename);
            //using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            //{
            //    await contactBanner.Photo.CopyToAsync(fileStream);
            //}
              contactBanner.PhotoPath= await  _filService.UploadAsync(contactBanner.Photo, _webHostEnvironment.WebRootPath);
            await _appDbContext.ContactBanners.AddAsync(contactBanner);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");


        }
        #endregion

        [HttpGet]

        public async Task<IActionResult> Details(int id)
        {
            var contactBanner = await _appDbContext.ContactBanners.FindAsync(id);
            if (contactBanner == null) return NotFound();

            return View(contactBanner);
        }

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var contactBanner = await _appDbContext.ContactBanners.FindAsync(id);
            if(contactBanner == null) return NotFound();

            var model = new BannerUpdateViewModel
            {
                Id = contactBanner.Id,
                Name = contactBanner.Name,
                Title = contactBanner.Title,
                Description = contactBanner.Description,
                PhotoPath   = contactBanner.PhotoPath,
                
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,BannerUpdateViewModel model)
        {
            if(!ModelState.IsValid) return View(model);
            if(id != model.Id) return BadRequest();

            var contactBanner = await _appDbContext.ContactBanners.FindAsync(id);
            if(contactBanner == null) return NotFound();

            contactBanner.Name = model.Name;
            contactBanner.Title = model.Title;
            contactBanner.Description = model.Description;
            if(model.Photo != null)
            {
                _filService.Delete(_webHostEnvironment.WebRootPath, contactBanner.PhotoPath);
                contactBanner.PhotoPath = await _filService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }
        #endregion

        public async Task<IActionResult> Delete(int id)
        {
            var contactBanner = await _appDbContext.ContactBanners.FindAsync(id);
            if (contactBanner == null) return NotFound();

            return View(contactBanner);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var contactBanner = await _appDbContext.ContactBanners.FindAsync(id);
            if (contactBanner == null) return NotFound();

            _filService.Delete(_webHostEnvironment.WebRootPath, contactBanner.PhotoPath);

            _appDbContext.ContactBanners.Remove(contactBanner);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }
    }
}
