using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        //-------------------------------------------------------------------------------------
        //private readonly ApplicationDbContext _db;
        //later changed while using Repository pattern

        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository db) 
        {
            _categoryRepo = db;   //basically dependency injection
        }

        //-------------------------------------------------------------------------------------
        public IActionResult Index()
        {
            List<Category> objCategoryList = _categoryRepo.GetAll().ToList();
            return View(objCategoryList);
        }

        //-------------------------------------------------------------------------------------
        //Create action methods
        public IActionResult Create() 
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj) 
        {
            //server side validations reduce performance
            //custom validation 1
            if (obj.Name == obj.DisplayOrder.ToString()) 
            {
                ModelState.AddModelError("name", "Category Name and Display Order cannot be same");
            }

            //custom validation 2
            if(obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is an invalid input");
                //notice how there is no key mentioned here
            }
            
            if(ModelState.IsValid) 
            {
                _categoryRepo.Add(obj);    //inserting into the database
                _categoryRepo.Save();          //only after this are the changes saved to database

                TempData["success"] = "Category created successfully";  //for notifications

                return RedirectToAction("Index"); //in different controller: ("Index", "Category")
            }

            return View();
        }

        //-------------------------------------------------------------------------------------
        //Edit action methods
        public IActionResult Edit(int? id) 
        {
            if(id == null || id == 0)
                return NotFound();

            //Fetching from database
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id); --> prefer this as it can be used with non-primary key as well
            //Category? categroyFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault(); --> when a number of filtering operations is to be done
            
            Category? categoryFromDb = _categoryRepo.Get(u => u.Id == id); //only works with primary key
            if (categoryFromDb == null)
                return NotFound();

            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj) 
        {
            if(ModelState.IsValid)
            {
                _categoryRepo.Update(obj); // creates a new record if the Id is 0 or null  --> to avoid this, use hidden input property in the Edit.cshtml
                _categoryRepo.Save();

                TempData["success"] = "Category edited successfully";   //for notifications

                return RedirectToAction("Index");
            }

            return View();
        }

        //-------------------------------------------------------------------------------------
        //Delete action methods

        public IActionResult Delete(int? id) 
        {
            if(id == null || id == 0)
                return NotFound();

            Category? categoryFromDb = _categoryRepo.Get(u => u.Id ==id);

            if (categoryFromDb == null)
                return NotFound();
          
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]    //because cant have same name action method with same parameters
        public IActionResult DeletePOST(int? id)
        {
            Category? objToBeRemoved = _categoryRepo.Get(u => u.Id == id);

            if(objToBeRemoved == null)
                return NotFound(); 
            
            _categoryRepo.Remove(objToBeRemoved);
            _categoryRepo.Save();

            TempData["success"] = "Category deleted successfully";

            return RedirectToAction("Index");            
        }
    }
}
