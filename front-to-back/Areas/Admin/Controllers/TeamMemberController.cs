using front_to_back.Areas.Admin.ViewModels;
using front_to_back.DAL;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamMemberController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamMemberController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new TeamMemberIndexViewModel
            {
                TeamMembers = await _appDbContext.TeamMembers.ToListAsync()
            };
            return View(model);
        }
        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(TeamMember teamMember)
        {
            if (!ModelState.IsValid) return View(teamMember);
            if (!teamMember.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Photo must be img format");
                return View(teamMember);
            }
            if (teamMember.Photo.Length / 1024 > 60)
            {
                ModelState.AddModelError("Photo", "Photo greather than 60kb");
                return View(teamMember);
            }

            var fileName = $"{Guid.NewGuid()}_{teamMember.Photo.FileName}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", fileName);
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await teamMember.Photo.CopyToAsync(fileStream);
            }

            teamMember.PhotoName = fileName;
            await _appDbContext.TeamMembers.AddAsync(teamMember);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");

        }
    }
}
