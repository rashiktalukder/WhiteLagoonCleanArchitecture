using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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
                if(villaObj.Image!=null)
                {
                    string fileName=Guid.NewGuid().ToString()+Path.GetExtension(villaObj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    villaObj.Image.CopyTo(fileStream);

                    villaObj.ImageUrl = @"\images\VillaImage\" + fileName;
                }
                else
                {
                    villaObj.ImageUrl = "https://placehold.co/600x400";
                }

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
            if (ModelState.IsValid && villaObj.Id>0)
            {
                if (villaObj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villaObj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                    if(!string.IsNullOrEmpty(villaObj.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, villaObj.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    villaObj.Image.CopyTo(fileStream);

                    villaObj.ImageUrl = @"\images\VillaImage\" + fileName;
                }

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
                if (!string.IsNullOrEmpty(villaFromDB.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, villaFromDB.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

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
