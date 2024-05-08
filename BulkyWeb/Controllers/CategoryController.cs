using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db) 
        {
            _db = db;   //basically dependency injection
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }

        public IActionResult Create() 
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj) 
        {
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
                _db.Categories.Add(obj);    //inserting into the database
                _db.SaveChanges();          //only after this are the changes saved to database
                return RedirectToAction("Index"); //in different controller: ("Index", "Category")
            }

            return View();
        }
    }
}
