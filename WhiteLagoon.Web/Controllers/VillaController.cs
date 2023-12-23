using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly ApplicationDBContext _context;
        public VillaController(ApplicationDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var villas=_context.Villas.ToList();
            return View(villas);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa villaObj)
        {
            if(villaObj.Name == villaObj.Description) 
            {
                ModelState.AddModelError("name", "The Description is Exactly same with the Name.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(villaObj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(villaObj);
            }
        }
    }
}
