using BookTrekker.DataAccess.Repository.IRepository;
using BookTrekker.Models;
using BookTrekker.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookTrekkerWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IUnitOfWork unitOfWork,ILogger<HomeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
          
            IEnumerable<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productsList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart Cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
           
            return View(Cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
           var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = UserId;

            ShoppingCart cartfromDb = _unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId == UserId &&
            u.ProductId ==shoppingCart.ProductId);

            if(cartfromDb != null)
            {
                //shopping cart already exists
                cartfromDb.Count += shoppingCart.Count;
               _unitOfWork.ShoppingCart.Update(cartfromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add cart to DB
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId).Count());
            }
            TempData["Success"] = "Cart Updated Successfully";
         
          
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}