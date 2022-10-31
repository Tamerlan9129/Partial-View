using front_to_back.Areas.Admin.ViewModels;
using front_to_back.Areas.Admin.ViewModels.CategoryComponent;
using front_to_back.DAL;
using front_to_back.Helpers;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryComponentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFilService _filService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryComponentController(AppDbContext appDbContext, IFilService filService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _filService = filService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new CategoryComponentIndexViewModel
            {
                CategoryComponents = await _appDbContext.CategoryComponents.
                                       Include(cc => cc.Category).ToListAsync()
            };
            return View(model);
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CategoryComponentCreateViewModel
            {
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Create(CategoryComponentCreateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (!ModelState.IsValid) return View(model);

            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if ( category == null)
            {
                ModelState.AddModelError("CategoryId", "This category does  not exist");
                return View(model);
            }

            bool isExist = await _appDbContext.CategoryComponents.
                AnyAsync(cc => cc.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "This title is already exixst");
                return View(model);
            }
            if (!_filService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "The photo must be in image format");
                return View(model);

            }
            int maxSize = 300;
            if (!_filService.CheckSize(model.Photo, maxSize))
            {
                ModelState.AddModelError("Photo", $"The photo size over than {maxSize} kb ");
                return View(model);
            }
            var categoryComponent = new CategoryComponent
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                FilePath = await _filService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.AddAsync(categoryComponent);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        #endregion


        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if (categoryComponent == null) return NotFound();

            var model = new CategoryComponentUpdateViewModel
            {
                Title = categoryComponent.Title,
                Description = categoryComponent.Description,
                CategoryId = categoryComponent.CategoryId,
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync(),
                PhotoPath = categoryComponent.FilePath
            };

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id,CategoryComponentUpdateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();
            if(!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();
            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(model.Id);
            if( categoryComponent == null) return NotFound();
            bool isExist = await _appDbContext.CategoryComponents.
                AnyAsync(cc => cc.Title.ToLower().Trim() == model.Title.ToLower().Trim()
                                                && cc.Id!=categoryComponent.Id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "This category component is already exist");
                return View(model);
            }
            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if(category == null) return NotFound();
           categoryComponent.Title = model.Title;
            categoryComponent.Description = model.Description;
            if (model.Photo != null)
            {
                if (!_filService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "File must be in img format");
                    return View(model);
                }
                int maxSize = 300;
                if (!_filService.CheckSize(model.Photo, maxSize))
                {
                    ModelState.AddModelError("Photo", $"File over than {maxSize} kb");
                    return View(model);
                }
                
                    _filService.Delete(_webHostEnvironment.WebRootPath, model.PhotoPath);
           categoryComponent.FilePath= await _filService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }


           await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if(categoryComponent == null) return NotFound();

            _filService.Delete(_webHostEnvironment.WebRootPath,categoryComponent.FilePath);
            _appDbContext.CategoryComponents.Remove(categoryComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");

        }
    }
}
