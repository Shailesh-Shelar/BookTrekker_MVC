using BookTrekker.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using BookTrekker.Models;
using BookTrekker.DataAccess.Repository;
using BookTrekker.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookTrekker.Models.ViewModels;
using BookTrekker.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BookTrekkerWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProductsList);
        }

        public IActionResult Upsert(int? id)
        {

            //ViewBag.CategoryList = CategoryList;
            // ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),

                Product = new Product()
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file )
        {
            /* if (obj.Name == obj.DisplayOrder.ToString())
             {
                 ModelState.AddModelError("name", "The Display Order and Name Should not exactly match");
             }*/
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if(!string.IsNullOrEmpty(obj.Product.ImageUrl)) 
                    {
                        //Delete the old path

                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if(obj.Product.Id ==0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    _unitOfWork.Save();
                    TempData["Success"] = "Product Created Successfully";
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();
                    TempData["Success"] = "Product Updated Successfully";
                    return RedirectToAction("Index", "Product");
                }
               
            }
            else
            {
                obj. CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                //ViewBag.CategoryList = CategoryList;
                // ViewData["CategoryList"] = CategoryList;
                return View(obj);
            }
        }
      
        
        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductsList } );
        }

       [HttpDelete]
        public IActionResult Delete(int? id)
            {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            var productToBeDeleted =  _unitOfWork.Product.Get(u=>u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Error While Deleting"
                });
            }

            var oldImagePath = Path.Combine(wwwRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new
            {
                success = true,
                message = "Delete Successfull"
            });
        }
        #endregion
    }
}
