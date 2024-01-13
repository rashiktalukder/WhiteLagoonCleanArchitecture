using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaRepository _villaRepo;
        public VillaController(IVillaRepository villaRepo)
        {
            _villaRepo = villaRepo;
        }
        public IActionResult Index()
        {
            var villas = _villaRepo.GetAll();
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
                _villaRepo.Add(villaObj);
                _villaRepo.Save();
                TempData["success"] = "The villa created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(villaObj);
            }
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa=_villaRepo.Get(v=>v.Id== villaId);
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
                _villaRepo.Update(villaObj);
                _villaRepo.Save();
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
            Villa? villa = _villaRepo.Get(v => v.Id == villaId);
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
            Villa? villaFromDB=_villaRepo.Get(v=>v.Id == villaObj.Id);
            if (villaFromDB is not null)
            {
                _villaRepo.Remove(villaFromDB);
                _villaRepo.Save();
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
