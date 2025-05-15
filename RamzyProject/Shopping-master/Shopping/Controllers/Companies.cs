using A_Service.IUnitOfWork;
using A_Service.Models;
using B_EF.Business.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers
{
    [Authorize(Roles ="Admin")]
    public class Companies : Controller
    {
        private IUnitOfWork _unitOfWork;
        //private ICategoryBusiness _ICompanyBusiness;
        public Companies(IUnitOfWork unitOfWork/*, ICategoryBusiness ICompanyBusiness*/)
        {
            _unitOfWork = unitOfWork;
            //_ICompanyBusiness = ICompanyBusiness;
        }


        public async Task<IActionResult> Index()
        {
            var allCompanies = await _unitOfWork.CompanyBaseRepository.GetAll();
            if (Convert.ToInt32(TempData["errors"]) == 1)
            {
                ModelState.AddModelError("repetedName", "الأسم مكرر");
            }

            return View(allCompanies);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Company company)
        {
            var allCompanies = await _unitOfWork.CompanyBaseRepository.GetAll();
            var obj = allCompanies.FirstOrDefault(a => a.Name == company.Name);
            if (obj is not null)
            {
                TempData["error"] = "الاسم موجد بالفعل ";
                return RedirectToAction(nameof(Index));
            }
            company.Code = allCompanies.ToList().Count == 0 ? 1 : allCompanies.Max(a => a.Code) + 1;

            _unitOfWork.CompanyBaseRepository.Add(company);
            await _unitOfWork.saveChanges();
            TempData["success"] = "تم الحفظ بنجاح";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? Id)
        {
            var companyObj = await _unitOfWork.CompanyBaseRepository.GetById(Convert.ToInt32(Id));
            if (companyObj == null)
                return RedirectToAction(nameof(Index));

            return Json(new
            {
                name = companyObj.Name,
                descriptions = companyObj.Description,
            });
        }


        [HttpPost]
        public async Task<IActionResult> Update(Company company)
        {

            var allCompanies = await _unitOfWork.CompanyBaseRepository.GetAll();
            var obj = allCompanies.FirstOrDefault(a => a.Name == company.Name && a.Id != company.Id);
            if (obj is not null)
            {
                TempData["error"] = "الاسم موجد بالفعل ";
                return RedirectToAction(nameof(Index));
            }


            var companyObj = await _unitOfWork.CompanyBaseRepository.GetById(Convert.ToInt32(company.Id));

            if (companyObj == null)
            {
                TempData["error"] = "لا يوجد بيانات";

                return RedirectToAction(nameof(Index));


            }

            companyObj.Name = company.Name;
            companyObj.Description = company.Description;

            await _unitOfWork.saveChanges();
            TempData["success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int? Id)
        {
            var companyObj = await _unitOfWork.CompanyBaseRepository.GetById(Convert.ToInt32(Id));

            bool ch1 = _unitOfWork.CategoryBaseRepository.Exists(a => a.CompanyId == companyObj.Id);
            bool ch2 = _unitOfWork.PriceListBaseRepository.Exists(a => a.CompanyId == companyObj.Id);
            bool ch3 = _unitOfWork.ProductbBaseRepository.Exists(a => a.CompanyId == companyObj.Id);

            if (ch1 || ch2 || ch3)
            {
                TempData["errors"] = 1;
                TempData["error"] = "لا يمكن الحذف ";
                return RedirectToAction(nameof(Index));

            }
            try
            {
                _unitOfWork.CompanyBaseRepository.Delete(Convert.ToInt32(Id));
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
