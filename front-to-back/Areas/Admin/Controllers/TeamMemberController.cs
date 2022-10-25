using front_to_back.Areas.Admin.ViewModels;
using front_to_back.Areas.Admin.ViewModels.TeamMember;
using front_to_back.DAL;
using front_to_back.Helpers;
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
        private readonly IFilService _filService;

        public TeamMemberController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment,IFilService filService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _filService = filService;
        }
        public async Task<IActionResult> Index()
        {
            var model = new TeamMemberIndexViewModel
            {
                TeamMembers = await _appDbContext.TeamMembers.ToListAsync()
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

        public async Task<IActionResult> Create(TeamMember teamMember)
        {
            if (!ModelState.IsValid) return View(teamMember);
            if (!_filService.IsImage(teamMember.Photo))
            {
                ModelState.AddModelError("Photo", "Photo must be img format");
                return View(teamMember);
            }

            int maxSize = 2000;
            if (!_filService.CheckSize(teamMember.Photo,maxSize))
            {
                ModelState.AddModelError("Photo", $"Photo greather than { maxSize} kb");
                return View(teamMember);
            }

          

            teamMember.PhotoName = await _filService.UploadAsync(teamMember.Photo, _webHostEnvironment.WebRootPath);
            await _appDbContext.TeamMembers.AddAsync(teamMember);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");

        }
        #endregion

        #region Update
        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            var teamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if(teamMember == null) return NotFound();
            var model = new TeamMemberUpdateVIewModel
            {
                Id = teamMember.Id,
                Name=teamMember.Name,
                Surname = teamMember.Surname,
                Position = teamMember.Position,
                PhotoName= teamMember.PhotoName
            };
            return View(model); ; 
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id,TeamMemberUpdateVIewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if(id != model.Id) return BadRequest();

            var dbteammember = await _appDbContext.TeamMembers.FindAsync(model.Id);
            if (dbteammember == null) return NotFound();

            dbteammember.Name=  model.Name;
            dbteammember.Surname= model.Surname;
            dbteammember.Position = model.Position;

            if (model.Photo != null)
            {
                _filService.Delete(_webHostEnvironment.WebRootPath, dbteammember.PhotoName);
                dbteammember.PhotoName = await _filService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }
        #endregion

        #region Delete
        [HttpGet]

        public async Task<IActionResult> Delete(int id)
        {
            var teammember = await _appDbContext.TeamMembers.FindAsync(id);
            if(teammember == null) return NotFound();

            return View(teammember);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var dbteammember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbteammember == null) return NotFound();

            _filService.Delete(_webHostEnvironment.WebRootPath, dbteammember.PhotoName);

            _appDbContext.TeamMembers.Remove(dbteammember);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }
        #endregion
    }
}
