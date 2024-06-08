using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]     //shows the corresponding Area for this controller
    public class ProductController : Controller
    {
        //-------------------------------------------------------------------------------------

        private IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //-------------------------------------------------------------------------------------
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
                  
            return View(objProductList);
        }

        //-------------------------------------------------------------------------------------
        //Create action methods
        //I later changed it to the upsert functionality (NOTE: need to change the Create view name to Upsert)
        public IActionResult Upsert(int? id)
        {
            //using projection feature
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

            //ViewBag.CategoryList = CategoryList;  --> replaced by ViewModel

            ProductVM productVM = new() 
            {
                CategoryList = CategoryList,    //NOTE: comma instead of semicolon
                Product = new Product()
            };

            //the actual upsert functionality
            if(id == null || id == 0)   //create
            {
                return View(productVM);
            }
            else    //update 
            { 
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

            
        }

        //changed this from Create to Upsert as well
        [HttpPost]
        public IActionResult Create(ProductVM productVM, IFormFile? file)   //IFormFile because on file submission, a file is being uploaded
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();

                TempData["success"] = "Product added successfully";  //for notifications

                return RedirectToAction("Index"); //in different controller: ("Index", "Category")
            }
            else    //if the ModelState is not valid, populate the dropdown without showing exception   
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });

                return View(productVM);
            }

        }

        //-------------------------------------------------------------------------------------
        //Edit action methods   --> these are not needed once you start using Upsert above (Also dont need Edit view)
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //        return NotFound();

        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (productFromDb == null)
        //        return NotFound();

        //    return View(productFromDb);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();

        //        TempData["success"] = "Product edited successfully";   //for notifications

        //        return RedirectToAction("Index");
        //    }

        //    return View();
        //}

        //-------------------------------------------------------------------------------------
        //Delete action methods
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDb == null)
                return NotFound();

            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]    //because cant have same name action method with same parameters
        public IActionResult DeletePOST(int? id)
        {
            Product? objToBeRemoved = _unitOfWork.Product.Get(u => u.Id == id);

            if (objToBeRemoved == null)
                return NotFound();

            _unitOfWork.Product.Remove(objToBeRemoved);
            _unitOfWork.Save();

            TempData["success"] = "Product deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
