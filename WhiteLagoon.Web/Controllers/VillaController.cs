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
                _context.Villas.Add(villaObj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(villaObj);
            }
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa=_context.Villas.FirstOrDefault(v=>v.Id== villaId);
            if(villa==null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(villa);
            }
        }
        [HttpPost]
        public IActionResult Update(Villa villaObj)
        {
            if (ModelState.IsValid)
            {
                _context.Villas.Update(villaObj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(villaObj);
            }
        }

        public IActionResult Delete(int villaId)
        {
            Villa? villa = _context.Villas.FirstOrDefault(v => v.Id == villaId);
            if (villa == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(villa);
            }
        }
        [HttpPost]
        public IActionResult Delete(Villa villaObj)
        {
            Villa? villaFromDB=_context.Villas.FirstOrDefault(v=>v.Id == villaObj.Id);
            if (villaFromDB is not null)
            {
                _context.Villas.Remove(villaFromDB);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
    }
}
