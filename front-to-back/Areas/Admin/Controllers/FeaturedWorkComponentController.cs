using front_to_back.Areas.Admin.ViewModels.FeaturedWorkComponent;
using front_to_back.Areas.Admin.ViewModels.FeaturedWorkComponent.FeaturedWorkComponentPhoto;
using front_to_back.DAL;
using front_to_back.Helpers;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeaturedWorkComponentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFilService _filService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FeaturedWorkComponentController(AppDbContext appDbContext,IFilService filService,IWebHostEnvironment webHostEnvironment)
        {
           _appDbContext = appDbContext;
            _filService = filService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task <IActionResult> Index()
        {
            var model = new FeaturedWorkComponentIndexViewModel
            {
                FeaturedWorkComponent = await _appDbContext.FeaturedWorkComponent.FirstOrDefaultAsync()
            };
            return View(model);
        }
        #region Create
        [HttpGet]
        
        public async Task<IActionResult> Create()
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent.FirstOrDefaultAsync();
            if(featuredWorkComponent != null) return NotFound();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FeaturedWorkComponentCreateViewModel model)
        {
            if(!ModelState.IsValid) return View(model);

            var featuredWorkComponent = new FeaturedWorkComponent
            {
                Title = model.Title,
                Description = model.Description
            };

            await _appDbContext.FeaturedWorkComponent.AddAsync(featuredWorkComponent);
            await _appDbContext.SaveChangesAsync();
                int maxSize = 400;

            bool hasError = false;

            foreach (var photo in model.Photos)
            {
                if (!_filService.IsImage(photo))
                {
                    ModelState.AddModelError("Photo", $"The photo{photo.FileName} must be in image format");
                    hasError = true;                   
                }
                else if(!_filService.CheckSize(photo, maxSize))
                {
                    ModelState.AddModelError("Photo", $"{photo.FileName} is over than{maxSize} kb");
                    hasError=true;
                }
            }
            if (hasError)
            {
                return View(model);
            }
            int order = 1;
            foreach (var photo in model.Photos)
            {
                var featuredWorkComponentPhoto = new FeaturedWorkComponentPhoto
                {
                    Name = await _filService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                    FeaturedWorkComponentId = featuredWorkComponent.Id
                };
                await _appDbContext.FeaturedWorkComponentPhotos.AddAsync(featuredWorkComponentPhoto);
                await _appDbContext.SaveChangesAsync();
                order++;    
            }
            return RedirectToAction("Index");
        }
        #endregion
        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent.
                                      Include(fwc=>fwc.FeaturedWorkComponentPhotos)
                                                 .FirstOrDefaultAsync();
            if (featuredWorkComponent == null) return NotFound();

            foreach (var photo in featuredWorkComponent.FeaturedWorkComponentPhotos)
            {
                 _filService.Delete(_webHostEnvironment.WebRootPath, photo.Name);
            }
             _appDbContext.FeaturedWorkComponent.Remove(featuredWorkComponent);
             await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        #endregion
        #region Update
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent.
                                                   Include(fwc=>fwc.FeaturedWorkComponentPhotos).
                                                        FirstOrDefaultAsync();
            if (featuredWorkComponent == null) return NotFound();

            var model = new FeaturedWorkComponentUpdateViewModel
            {
                Title = featuredWorkComponent.Title,
                Description = featuredWorkComponent.Description,
                FeaturedWorkComponentPhotos = featuredWorkComponent.FeaturedWorkComponentPhotos.ToList()
            };
          return View (model);
        }
        [HttpPost]

        public async Task<IActionResult> Update(FeaturedWorkComponentUpdateViewModel model)
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent.
                                        Include(fcw=>fcw.FeaturedWorkComponentPhotos).
                                               FirstOrDefaultAsync();

            model.FeaturedWorkComponentPhotos = featuredWorkComponent.FeaturedWorkComponentPhotos.ToList();
            if (featuredWorkComponent == null) return NotFound();      
            if (!ModelState.IsValid) return View(model);

            featuredWorkComponent.Title = model.Title;
            featuredWorkComponent.Description = model.Description;

            await _appDbContext.SaveChangesAsync();

            bool hasError = false;
            int maxSize = 400;
            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_filService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photos", $"The photo{photo.FileName} must be in image format");
                        hasError = true;

                    }
                    else if (!_filService.CheckSize(photo, maxSize))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} is over than{maxSize} kb");
                        hasError = true;
                    }

                }
            if (hasError) return View(model);



            int order = 1;
            foreach (var photo in model.Photos)
            {
                var featuredWorkComponentPhoto = new FeaturedWorkComponentPhoto
                {
                    Name = await _filService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                    FeaturedWorkComponentId=featuredWorkComponent.Id
                };
                await _appDbContext.FeaturedWorkComponentPhotos.AddAsync(featuredWorkComponentPhoto);
                await _appDbContext.SaveChangesAsync();
                order++;
            }
            }
            return RedirectToAction("index");
        }

        #endregion

        #region Details
        public async Task <IActionResult> Details ()
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent.
                                                   Include(fwc => fwc.FeaturedWorkComponentPhotos).
                                                        FirstOrDefaultAsync();
            if (featuredWorkComponent == null) return NotFound();

            var model = new FeaturedWorkComponentDetailsViewModel
            {
                Title = featuredWorkComponent.Title,
                Description = featuredWorkComponent.Description,
                FeaturedWorkComponentPhotos = featuredWorkComponent.FeaturedWorkComponentPhotos.ToList()
            };
            return View(model);
        }

        #endregion
        #region FeaturedWorkComponent Photo
        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var featuredWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(id);
            if (featuredWorkComponentPhoto == null) return NotFound();

            var model = new FeaturedWorkComponentPhotoUpdateViewModel
            {
                Order = featuredWorkComponentPhoto.Order
            };
            return View(model);
           
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id,FeaturedWorkComponentPhotoUpdateViewModel model)
        {
            if(!ModelState.IsValid) return View(model);
            if(id != model.Id) return BadRequest();
            var featuredWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(id);
            if (featuredWorkComponentPhoto == null) return NotFound();
             
            featuredWorkComponentPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "featuredWorkComponent", new { id = featuredWorkComponentPhoto.FeaturedWorkComponentId });
        }

        public async Task<IActionResult> DeletePhoto(int id)
        {
            var featureWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(id);
            if (featureWorkComponentPhoto == null) return NotFound();
            _filService.Delete(_webHostEnvironment.WebRootPath, featureWorkComponentPhoto.Name);
            _appDbContext.Remove(featureWorkComponentPhoto);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("update","featuredWorkComponent",new {id = featureWorkComponentPhoto.FeaturedWorkComponentId});
        }
        #endregion
    }
}
