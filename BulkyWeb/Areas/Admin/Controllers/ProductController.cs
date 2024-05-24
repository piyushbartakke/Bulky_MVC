using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();

                TempData["success"] = "Product added successfully";  //for notifications

                return RedirectToAction("Index"); //in different controller: ("Index", "Category")
            }

            return View();
        }

        //-------------------------------------------------------------------------------------
        //Edit action methods
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            if (productFromDb == null)
                return NotFound();

            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();

                TempData["success"] = "Product edited successfully";   //for notifications

                return RedirectToAction("Index");
            }

            return View();
        }

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
