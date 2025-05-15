using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using A_Service.IUnitOfWork;
using A_Service.Models;
using A_Service.ViewModels;
using B_EF.Business.Managers;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace Shopping.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductBusiness _IproductBusiness;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("01234567890123456789012345678901"); // 32 bytes for AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("0123456789012345"); // 16 bytes for AES

        public ProductController(IUnitOfWork unitOfWork, IProductBusiness IproductBusiness, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _IproductBusiness = IproductBusiness;
            _webHostEnvironment = webHostEnvironment;
        }

        // Encrypt the price using AES-256
        private string EncryptPrice(decimal price)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] priceBytes = Encoding.UTF8.GetBytes(price.ToString("F2"));
                byte[] encrypted = encryptor.TransformFinalBlock(priceBytes, 0, priceBytes.Length);
                return Convert.ToBase64String(encrypted);
            }
        }

        [HttpGet]
        public IActionResult DisplayBarcode(int id)
        {
            var product = _IproductBusiness.GetProductById(id);
            if (product == null)
                return NotFound();

            var activePrice = product.Prices.FirstOrDefault(p => p.IsActive ?? false);
            if (activePrice == null)
                return NotFound();

            string encryptedPrice = EncryptPrice(activePrice.SellingPrice);

            // Label size in pixels (38mm x 12mm at 300 DPI)
            int labelWidth = 449;
            int labelHeight = 142;

            // Barcode size (leave some margin for text)
            int barcodeHeight = 60;
            int barcodeWidth = 400;

            // Generate barcode
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = barcodeWidth,
                    Height = barcodeHeight,
                    Margin = 0
                }
            };
            var bitMatrix = barcodeWriter.Encode(encryptedPrice);

            using (var bitmap = new Bitmap(labelWidth, labelHeight))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                // Draw top text
                using (var font = new Font("Arial", 24, FontStyle.Bold))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                {
                    graphics.DrawString("معرض الطنطاوى", font, Brushes.Black, new RectangleF(0, 0, labelWidth, 36), sf);
                }

                // Draw barcode
                int barcodeX = (labelWidth - barcodeWidth) / 2;
                int barcodeY = 36;
                for (int x = 0; x < bitMatrix.Width; x++)
                {
                    for (int y = 0; y < bitMatrix.Height; y++)
                    {
                        if (bitMatrix[x, y])
                        {
                            int px = barcodeX + x;
                            int py = barcodeY + y;
                            if (px >= 0 && px < labelWidth && py >= 0 && py < labelHeight)
                            {
                                bitmap.SetPixel(px, py, Color.Black);
                            }
                        }
                    }
                }

                // Draw bottom text (product name)
                using (var font = new Font("Arial", 18, FontStyle.Regular))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far })
                {
                    graphics.DrawString(product.Name, font, Brushes.Black, new RectangleF(0, labelHeight - 32, labelWidth, 32), sf);
                }

                // Convert bitmap to base64
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] imageBytes = stream.ToArray();
                    string base64Image = Convert.ToBase64String(imageBytes);
                    ViewBag.BarcodeImage = $"data:image/png;base64,{base64Image}";
                }
            }

            return View(product);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<ProductViewModel> allProducts = await _IproductBusiness.GetAll();

            ViewBag.Companies = new SelectList(await _unitOfWork.CompanyBaseRepository.GetAll(), nameof(Company.Id), nameof(Company.Name));
            ViewBag.Categories = new SelectList(await _unitOfWork.CategoryBaseRepository.GetAll(), nameof(Category.Id), nameof(Category.Name));
            if (Convert.ToInt32(TempData["errors"]) == 1)
            {
                ModelState.AddModelError("blockDeleteActivePrice", "لا يمكن حذف اخر سعر");
            }

            return View(allProducts);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel productViewModel)
        {
            Product product = new Product
            {
                Code = 0,
                Name = productViewModel.Name,
                CompanyId = productViewModel.CompanyId,
                CategoryId = productViewModel.CategoryId,
                Description = productViewModel.Description,
                CreatedDate = DateTime.Now,
                Prices = new List<Price>
                {
                    new Price
                    {
                        PurchasingPrice = productViewModel.PurchasingPrice,
                        GomlaPrice = productViewModel.GomlaPrice,
                        NosGomlaPrice = productViewModel.NosGomlaPrice,
                        SellingPrice = productViewModel.SellingPrice,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    }
                }
            };

            if (productViewModel.photos != null && productViewModel.photos.Any())
            {
                product.ProductImages = new List<ProductImage>();
                var wwwroot = _webHostEnvironment.WebRootPath;

                try
                {
                    foreach (var item in productViewModel.photos)
                    {
                        var splitImage = item.ContentType.Split("/");
                        var exten = "." + splitImage[1];
                        string modifyImageName = $"{Guid.NewGuid().ToString()}_{exten}";
                        string requiredPath = $"{wwwroot}/productImages/{modifyImageName}";
                        using (FileStream fileStream = new FileStream(requiredPath, FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        product.ProductImages.Add(new ProductImage { ImagePath = modifyImageName });
                    }
                }
                catch (Exception ex)
                {
                    TempData["success"] = "يوجد خطا";
                    return RedirectToAction("Index");
                }
            }

            _unitOfWork.ProductbBaseRepository.Add(product);
            await _unitOfWork.saveChanges();
            TempData["success"] = "تم الحفظ بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(UserDTO userDTO)
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel productViewModel, List<IFormFile> productImages)
        {
            List<string> oldImage = new List<string>();
            if (productViewModel.oldImageString != null)
            {
                oldImage.AddRange(productViewModel.oldImageString.Split(",").ToList());
                productViewModel.oldImage = oldImage;
            }

            var wwwroot = _webHostEnvironment.WebRootPath;
            var product = _IproductBusiness.GetProducDbtById(productViewModel.Id);
            var prices = _unitOfWork.PricesBaseRepository.GetAllWithInclude(a => a.Product)
                .Where(a => a.ProductId == productViewModel.Id && (a.IsActive ?? false))
                .FirstOrDefault();

            if (product != null)
            {
                product.Name = productViewModel.Name;
                product.CompanyId = productViewModel.CompanyId;
                product.CategoryId = productViewModel.CategoryId;
                product.Description = productViewModel.Description;

                if (prices != null)
                {
                    prices.PurchasingPrice = productViewModel.PurchasingPrice;
                    prices.GomlaPrice = productViewModel.GomlaPrice;
                    prices.NosGomlaPrice = productViewModel.NosGomlaPrice;
                    prices.SellingPrice = productViewModel.SellingPrice;
                }
            }

            if (product.ProductImages.Any())
            {
                if (productViewModel.oldImage != null && productViewModel.oldImage.Count > 0)
                {
                    foreach (var item in product.ProductImages.ToList())
                    {
                        if (!productViewModel.oldImage.Contains(item.ImagePath))
                        {
                            System.IO.File.Delete($"{wwwroot}/productImages/{item.ImagePath}");
                            product.ProductImages.Remove(item);
                        }
                    }
                }
                else
                {
                    foreach (var item in product.ProductImages.ToList())
                    {
                        System.IO.File.Delete($"{wwwroot}/productImages/{item.ImagePath}");
                        product.ProductImages.Remove(item);
                    }
                }
            }

            if (productImages != null && productImages.Any())
            {
                foreach (var item in productImages)
                {
                    string modifyImageName = $"{Guid.NewGuid().ToString()}_{item.FileName}";
                    string requiredPath = $"{wwwroot}/productImages/{modifyImageName}";
                    using (FileStream fileStream = new FileStream(requiredPath, FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }
                    product.ProductImages.Add(new ProductImage { ImagePath = modifyImageName });
                }
            }

            await _unitOfWork.saveChanges();
            TempData["success"] = "تم التعديل بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id != null)
            {
                var obj = _IproductBusiness.GetProductById(Convert.ToInt32(id));
                if (obj == null)
                    return NotFound();

                var price = obj.Prices.FirstOrDefault(a => a.IsActive ?? false);

                return Json(new
                {
                    id = obj.Id,
                    companyId = obj.CompanyId,
                    categoryId = obj.CategoryId,
                    name = obj.Name,
                    purchasingPrice = price?.PurchasingPrice,
                    gomla = price?.GomlaPrice,
                    nosGomla = price?.NosGomlaPrice,
                    sellingPrice = price?.SellingPrice,
                    descriptions = obj.Description,
                    productImages = obj.ProductImages
                });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetProductForPriceHistory(int? id)
        {
            var isTrue = User.IsInRole("Admin");

            if (id != null)
            {
                var obj = _IproductBusiness.GetProductById(Convert.ToInt32(id));
                if (obj == null)
                {
                    TempData["error"] = "لا يوجد بيانات";
                    return NotFound();
                }

                var prices = obj.Prices.OrderByDescending(a => a.CreatedDate).ToList();

                return Json(new
                {
                    isTrue = isTrue,
                    code = obj.Code,
                    companyName = obj.CompanyName,
                    categoryName = obj.CategoryName,
                    name = obj.Name,
                    prices = prices.Select(a => new
                    {
                        id = a.Id,
                        purchasingPrice = a.PurchasingPrice,
                        gomla = a.GomlaPrice,
                        nosGomla = a.NosGomlaPrice,
                        sellingPrice = a.SellingPrice,
                        createdDate = a.CreatedDate
                    })
                });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var obj = _IproductBusiness.GetProducDbtById(Convert.ToInt32(id));
                var wwwroot = _webHostEnvironment.WebRootPath;
                if (obj?.ProductImages?.Count > 0)
                {
                    foreach (var item in obj.ProductImages)
                    {
                        if (System.IO.File.Exists($"{wwwroot}/productImages/{item.ImagePath}"))
                        {
                            System.IO.File.Delete($"{wwwroot}/productImages/{item.ImagePath}");
                        }
                    }
                }

                _unitOfWork.ProductbBaseRepository.Delete(Convert.ToInt32(id));
                await _unitOfWork.saveChanges();
                TempData["success"] = "تم الحذف بنجاح";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePrice(int? id)
        {
            var obj = await _unitOfWork.PricesBaseRepository.GetById(Convert.ToInt32(id));
            if (id == null || obj == null)
            {
                TempData["error"] = "لا يوجد بيانات";
                return RedirectToAction("Index");
            }
            if (obj.IsActive ?? false)
            {
                TempData["error"] = "البيانات نشطه";
                return RedirectToAction("Index");
            }
            _unitOfWork.PricesBaseRepository.Delete(Convert.ToInt32(id));
            await _unitOfWork.saveChanges();
            TempData["success"] = "تم الحذف بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetActivePrice(int? id)
        {
            if (id != null)
            {
                var obj = _IproductBusiness.GetProductById(Convert.ToInt32(id));
                if (obj == null)
                    return NotFound();

                var price = obj.Prices.FirstOrDefault(a => a.IsActive ?? false);

                return Json(new
                {
                    purchasingPrice = price?.PurchasingPrice,
                    gomla = price?.GomlaPrice,
                    nosGomla = price?.NosGomlaPrice,
                    sellingPrice = price?.SellingPrice
                });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrice(ProductViewModel productViewModel)
        {
            var obj = _unitOfWork.ProductbBaseRepository.GetAllWithInclude(a => a.Prices)
                .Where(c => c.Id == productViewModel.Id)
                .FirstOrDefault();

            if (obj == null)
            {
                TempData["error"] = "لا يوجد بيانات";
                return NotFound();
            }

            obj.Prices.ToList().ForEach(a => a.IsActive = false);
            obj.Prices.Add(new Price
            {
                PurchasingPrice = productViewModel.PurchasingPrice,
                GomlaPrice = productViewModel.GomlaPrice,
                NosGomlaPrice = productViewModel.NosGomlaPrice,
                SellingPrice = productViewModel.SellingPrice,
                CreatedDate = DateTime.Now,
                IsActive = true
            });

            await _unitOfWork.saveChanges();
            TempData["success"] = "تم التعديل بنجاح";
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id != null)
            {
                var obj = _IproductBusiness.GetProductById(Convert.ToInt32(id));
                if (obj == null || obj.Id == 0)
                    return RedirectToAction("HandleError");
                return View(obj);
            }
            return RedirectToAction("HandleError");
        }

        public async Task<IActionResult> Search(string Key)
        {
            if (Key != null)
            {
                var obj = _IproductBusiness.GetProductByName(Key);
                if (obj == null || obj.Id == 0)
                    return View(null);
                return View(obj);
            }
            return View(null);
        }

        public IActionResult HandleError()
        {
            return View();
        }
    }
}
