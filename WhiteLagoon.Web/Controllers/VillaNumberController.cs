using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDBContext _context;
        public VillaNumberController(ApplicationDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var villaNumbers=_context.VillaNumbers.ToList();
            return View(villaNumbers);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(VillaNumber villaNumObj)
        {
            //ModelState.Remove("Villa"); use this / Villa prop needs to nullable / use [ValidateNever] annotation on Villa Prop.
            if (ModelState.IsValid)
            {
                _context.VillaNumbers.Add(villaNumObj);
                _context.SaveChanges();
                TempData["success"] = "The villa Number has been created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(villaNumObj);
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
                TempData["success"] = "The villa has been updated successfully!";
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
                TempData["success"] = "The villa has been deleted successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "The villa could not be deleted!";
                return View();
            }
        }
    }
}
