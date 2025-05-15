using A_Service.IUnitOfWork;
using A_Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Categories : Controller
    {
        private IUnitOfWork _unitOfWork;

        public Categories(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<IActionResult> Index()
        {
            var all = await _unitOfWork.CategoryBaseRepository.GetAll();
            ViewBag.Companies = new SelectList(await _unitOfWork.CompanyBaseRepository.GetAll(), nameof(Company.Id), nameof(Company.Name));

            if (Convert.ToInt32(TempData["errors"]) == 1)
            {
                ModelState.AddModelError("repetedName", "الأسم مكرر");
            }

            return View(all);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            var all = await _unitOfWork.CategoryBaseRepository.GetAll();
            var obj = all.FirstOrDefault(a => a.Name == category.Name);
            if (obj is not null)
            {
                TempData["error"] = "الاسم موجد بالفعل ";
                return RedirectToAction(nameof(Index));
            }
            category.Code = all.ToList().Count == 0 ? 1 : all.Max(a => a.Code) + 1;

            _unitOfWork.CategoryBaseRepository.Add(category);
            await _unitOfWork.saveChanges();
            TempData["success"] = "تم الحفظ بنجاح";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? Id)
        {
            var Obj = await _unitOfWork.CategoryBaseRepository.GetById(Convert.ToInt32(Id));
            if (Obj == null)
                return RedirectToAction(nameof(Index));

            return Json(new
            {
                id = Obj.Id,
                name = Obj.Name,
                companyId = Obj.CompanyId,
                descriptions = Obj.Description,
            });
        }


        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            //int id = category.Id;


            var all = await _unitOfWork.CategoryBaseRepository.GetAll();
            var obj = all.FirstOrDefault(a => a.Name == category.Name && a.Id != category.Id);
            if (obj is not null)
            {
                TempData["error"] = "الاسم موجد بالفعل ";
                return RedirectToAction(nameof(Index));
            }


            var ObjData = await _unitOfWork.CategoryBaseRepository.GetById(category.Id);


            if (ObjData == null) {
                TempData["error"] = "لا يوجد بيانات";

                return RedirectToAction(nameof(Index));

            }

            ObjData.Name = category.Name;
            ObjData.Description = category.Description;
            ObjData.CompanyId = category.CompanyId;

            await _unitOfWork.saveChanges();
            TempData["success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int? Id)
        {
            var Obj = await _unitOfWork.CategoryBaseRepository.GetById(Convert.ToInt32(Id));
            bool ch3 = _unitOfWork.ProductbBaseRepository.Exists(a => a.CompanyId == Obj.Id);

            if (ch3)
            {
                TempData["errors"] = 1;
                TempData["error"] = "لا يمكن الحذف ";
                return RedirectToAction(nameof(Index));

            }
            try
            {
                _unitOfWork.CategoryBaseRepository.Delete(Convert.ToInt32(Id));
                await _unitOfWork.saveChanges();
                TempData["success"] = "تم الحذف بنجاح";
            }
            catch (Exception ex)
            {
                TempData["error"] = "لا يمكن الحذف ";
                return RedirectToAction(nameof(Index));

            }

            return RedirectToAction(nameof(Index));
        }


    }
}
