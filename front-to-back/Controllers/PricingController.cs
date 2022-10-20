using front_to_back.DAL;
using front_to_back.ViewModels.Pricing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Controllers
{
    public class PricingController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public PricingController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new PricingIndexViewModel
            {
                PricingComponents = await _appDbContext.PricingComponents.
                                    OrderByDescending(p => p.Id).
                                    Take(3).
                                    ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> LoadMore(int skiprow)
        {
            var pricingComponents = await _appDbContext.PricingComponents.
                                                  OrderByDescending(p=>p.Id).
                                                  Skip(3+skiprow).
                                                  Take(1).
                                                  ToListAsync();
            return PartialView("_PricingComponentPartial",pricingComponents);
        }
    }
}
