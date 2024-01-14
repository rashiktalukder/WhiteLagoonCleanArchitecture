using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties: "Villa");
            return View(amenities);
        }
        public IActionResult Create()
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityVM amenityObj)
        {
            //ModelState.Remove("Villa"); use this / Villa prop needs to nullable / use [ValidateNever] annotation on Villa Prop.
            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Add(amenityObj.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "The Amenity has been created successfully!";
                return RedirectToAction("Index");
            }
            
            amenityObj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(amenityObj);

        }

        public IActionResult Update(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList =_unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(u=>u.Id==amenityId)
            };
            if(amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(amenityVM);
        }
        [HttpPost]
        public IActionResult Update(AmenityVM amenityObj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Update(amenityObj.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "The Amenity has been Updated successfully!";
                return RedirectToAction("Index");
            }

            amenityObj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(amenityObj);
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
            };
            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(amenityVM);
        }
        [HttpPost]
        public IActionResult Delete(AmenityVM amenityObj)
        {
            Amenity? amenityFromDB = _unitOfWork.Amenity.Get(v => v.Id == amenityObj.Amenity.Id);
            if (amenityFromDB is not null)
            {
                _unitOfWork.Amenity.Remove(amenityFromDB);
                _unitOfWork.Save();
                TempData["success"] = "The Amenity has been deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "The Amenity could not be deleted!";
                return View();
            }
        }
    }
}
