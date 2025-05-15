using A_Service.IUnitOfWork;
using A_Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PriceListController : Controller
    {

        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public PriceListController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index()
        {
            var all =  _unitOfWork.PriceListBaseRepository.GetAll().Result.OrderByDescending(a=>a.Id).ToList();
            ViewBag.Companies = new SelectList(await _unitOfWork.CompanyBaseRepository.GetAll(), nameof(Company.Id), nameof(Company.Name));

            if (Convert.ToInt32(TempData["errors"]) == 1)
            {
                ModelState.AddModelError("repetedName", "الأسم مكرر");
            }

            return View(all);
        }


        [HttpPost]
        public async Task<IActionResult> Add(PriceList priceList, IFormFile File)
        {
            var all = await _unitOfWork.PriceListBaseRepository.GetAll();
            var obj = all.FirstOrDefault(a => a.Name == priceList.Name);
            if (obj is not null)
            {
                TempData["errors"] = 1;
                return RedirectToAction(nameof(Index));
            }
            priceList.Code = all.ToList().Count == 0 ? 1 : all.Max(a => a.Code) + 1;

            if (File is not null)
            {
                var wwwroot = _webHostEnvironment.WebRootPath;
                string modifyName = $"{Guid.NewGuid().ToString()}_{File.FileName}";
                string requiredPath = $"{wwwroot}/priceLists/{modifyName}";
                using (FileStream fileStream = new FileStream(requiredPath, FileMode.Create))
                {
                    await File.CopyToAsync(fileStream);
                }

                priceList.FilePath = modifyName;
            }



            _unitOfWork.PriceListBaseRepository.Add(priceList);
            await _unitOfWork.saveChanges();
            TempData["success"] = "تم الحفظ بنجاح";


            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? Id)
        {
            var Obj = await _unitOfWork.PriceListBaseRepository.GetById(Convert.ToInt32(Id));
            if (Obj == null)
                return RedirectToAction(nameof(Index));

            return Json(new
            {
                companyId = Obj.CompanyId,
                name = Obj.Name,
                descriptions = Obj.Description,
            });
        }


        [HttpPost]
        public async Task<IActionResult> Update(PriceList priceList, IFormFile File)
        {
            var all = await _unitOfWork.PriceListBaseRepository.GetAll();
            var obj = all.FirstOrDefault(a => a.Name == priceList.Name && a.Id != priceList.Id);
            if (obj is not null)
            {
                TempData["errors"] = 1;
                return RedirectToAction(nameof(Index));
            }

            var priceListObj = await _unitOfWork.PriceListBaseRepository.GetById(priceList.Id);

            if (priceListObj is not null)
            {
                priceListObj.CompanyId = priceList.CompanyId;
                priceListObj.Name = priceList.Name;
                priceListObj.Description = priceList.Description;
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

            if (File is not null)
            {
                var wwwroot = _webHostEnvironment.WebRootPath;
                string modifyName = $"{Guid.NewGuid().ToString()}_{File.FileName}";
                string requiredPath = $"{wwwroot}/priceLists/{modifyName}";
                using (FileStream fileStream = new FileStream(requiredPath, FileMode.Create))
                {
                    await File.CopyToAsync(fileStream);
                }


                if (System.IO.File.Exists($"{wwwroot}/priceLists/{priceListObj.FilePath}"))
                {
                    System.IO.File.Delete($"{wwwroot}/priceLists/{priceListObj.FilePath}");
                }

                priceListObj.FilePath = modifyName;
            }

            await _unitOfWork.saveChanges();
            TempData["success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int? Id)
        {
            var Obj = await _unitOfWork.PriceListBaseRepository.GetById(Convert.ToInt32(Id));
            //if (Obj.Products.Any())
            //    return RedirectToAction(nameof(Index));
            if (Id is not null)
            {
                _unitOfWork.PriceListBaseRepository.Delete(Convert.ToInt32(Id));
                var wwwroot = _webHostEnvironment.WebRootPath;
                //string requiredPath = $"{wwwroot}/priceLists/{Obj.FilePath}";

                if (System.IO.File.Exists($"{wwwroot}/priceLists/{Obj.FilePath}"))
                {
                    System.IO.File.Delete($"{wwwroot}/priceLists/{Obj.FilePath}");
                }
                await _unitOfWork.saveChanges();
                TempData["success"] = "تم الحذف بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }


        
        public async Task<ActionResult> DownLoad(int id)
        {
            var Obj = await _unitOfWork.PriceListBaseRepository.GetById(Convert.ToInt32(id));
            if (Obj==null)
                return RedirectToAction(nameof(Index));
            var wwwroot = _webHostEnvironment.WebRootPath;
            string requiredPath = $"{wwwroot}/priceLists/{Obj.FilePath}";


            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(requiredPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(requiredPath);
            return File(bytes, contentType, Path.GetFileName(Obj.FilePath));


            
        }

    }
}
