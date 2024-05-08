﻿using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        //-------------------------------------------------------------------------------------
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db) 
        {
            _db = db;   //basically dependency injection
        }

        //-------------------------------------------------------------------------------------
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj);    //inserting into the database
                _db.SaveChanges();          //only after this are the changes saved to database
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
            
            Category? categoryFromDb = _db.Categories.Find(id); //only works with primary key
            if (categoryFromDb == null)
                return NotFound();

            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj) 
        {
            return View();
        }
    }
}
