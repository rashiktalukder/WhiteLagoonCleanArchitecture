using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

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
            var villaNumbers = _context.VillaNumbers.Include(u => u.Villa).ToList();
            return View(villaNumbers);
        }
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _context.Villas.ToList().Select(u => new SelectListItem
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
            bool roomNumberExist = _context.VillaNumbers.Any(v => v.Villa_Number == villaNumObj.VillaNumber.Villa_Number);

            //ModelState.Remove("Villa"); use this / Villa prop needs to nullable / use [ValidateNever] annotation on Villa Prop.
            if (ModelState.IsValid && !roomNumberExist)
            {
                _context.VillaNumbers.Add(villaNumObj.VillaNumber);
                _context.SaveChanges();
                TempData["success"] = "The villa Number has been created successfully!";
                return RedirectToAction("Index");
            }
            if (roomNumberExist)
            {
                TempData["error"] = "The Villa Number Already Exists.";
            }
            villaNumObj.VillaList = _context.Villas.ToList().Select(u => new SelectListItem
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
                VillaList = _context.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _context.VillaNumbers.FirstOrDefault(u=>u.Villa_Number==villaNumberId)
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
                _context.VillaNumbers.Update(villaNumberObj.VillaNumber);
                _context.SaveChanges();
                TempData["success"] = "The villa Number has been Updated successfully!";
                return RedirectToAction("Index");
            }

            villaNumberObj.VillaList = _context.Villas.ToList().Select(u => new SelectListItem
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
                VillaList = _context.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _context.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaNumberId)
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
            VillaNumber? villaNumberFromDB = _context.VillaNumbers.FirstOrDefault(v => v.Villa_Number == villaNumberObj.VillaNumber.Villa_Number);
            if (villaNumberFromDB is not null)
            {
                _context.VillaNumbers.Remove(villaNumberFromDB);
                _context.SaveChanges();
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
