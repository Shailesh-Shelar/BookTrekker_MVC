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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> objCompanysList = _unitOfWork.Company.GetAll().ToList();

            return View(objCompanysList);
        }

        public IActionResult Upsert(int? id)
        {

            //ViewBag.CategoryList = CategoryList;
            // ViewData["CategoryList"] = CategoryList;

         
            if (id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company companyobj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyobj);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            /* if (obj.Name == obj.DisplayOrder.ToString())
             {
                 ModelState.AddModelError("name", "The Display Order and Name Should not exactly match");
             }*/
            if (ModelState.IsValid)
            {
              

                if(obj.Id ==0)
                {
                    _unitOfWork.Company.Add(obj);
                    _unitOfWork.Save();
                    TempData["Success"] = "Company Created Successfully";
                    return RedirectToAction("Index", "Company");
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    _unitOfWork.Save();
                    TempData["Success"] = "Company Updated Successfully";
                    return RedirectToAction("Index", "Company");
                }
               
            }
            else
            {
                //ViewBag.CategoryList = CategoryList;
                // ViewData["CategoryList"] = CategoryList;
                return View(obj);
            }
        }
      
        
        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanysList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanysList } );
        }

       [HttpDelete]
        public IActionResult Delete(int? id)
            {
            var companyToBeDeleted =  _unitOfWork.Company.Get(u=>u.Id == id);
            if(companyToBeDeleted == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Error While Deleting"
                });
            }

          
            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();
            List<Company> objCompanysList = _unitOfWork.Company.GetAll().ToList();
            return Json(new
            {
                success = true,
                message = "Delete Successfull"
            });
        }
        #endregion
    }
}
