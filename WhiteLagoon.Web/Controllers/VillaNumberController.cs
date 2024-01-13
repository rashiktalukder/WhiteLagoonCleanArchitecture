using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa");
            return View(villaNumbers);
        }
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM villaNumObj)
        {
            bool roomNumberExist = _unitOfWork.VillaNumber.Any(v => v.Villa_Number == villaNumObj.VillaNumber.Villa_Number);

            //ModelState.Remove("Villa"); use this / Villa prop needs to nullable / use [ValidateNever] annotation on Villa Prop.
            if (ModelState.IsValid && !roomNumberExist)
            {
                _unitOfWork.VillaNumber.Add(villaNumObj.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa Number has been created successfully!";
                return RedirectToAction("Index");
            }
            if (roomNumberExist)
            {
                TempData["error"] = "The Villa Number Already Exists.";
            }
            villaNumObj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(villaNumObj);

        }

        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList =_unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(u=>u.Villa_Number==villaNumberId)
            };
            if(villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberObj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.VillaNumber.Update(villaNumberObj.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa Number has been Updated successfully!";
                return RedirectToAction("Index");
            }

            villaNumberObj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(villaNumberObj);
        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId)
            };
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberObj)
        {
            VillaNumber? villaNumberFromDB = _unitOfWork.VillaNumber.Get(v => v.Villa_Number == villaNumberObj.VillaNumber.Villa_Number);
            if (villaNumberFromDB is not null)
            {
                _unitOfWork.VillaNumber.Remove(villaNumberFromDB);
                _unitOfWork.Save();
                TempData["success"] = "The villa Number has been deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "The villa Number could not be deleted!";
                return View();
            }
        }
    }
}
