using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
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
                _unitOfWork.Villa.Add(villaObj);
                _unitOfWork.Save();
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
            Villa? villa=_unitOfWork.Villa.Get(v=>v.Id== villaId);
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
                _unitOfWork.Villa.Update(villaObj);
                _unitOfWork.Save();
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
            Villa? villa = _unitOfWork.Villa.Get(v => v.Id == villaId);
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
            Villa? villaFromDB=_unitOfWork.Villa.Get(v=>v.Id == villaObj.Id);
            if (villaFromDB is not null)
            {
                _unitOfWork.Villa.Remove(villaFromDB);
                _unitOfWork.Save();
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
