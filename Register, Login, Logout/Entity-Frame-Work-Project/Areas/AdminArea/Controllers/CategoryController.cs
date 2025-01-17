﻿using Entity_Frame_Work_Project.Data;
using Entity_Frame_Work_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity_Frame_Work_Project.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _context.Categories.Where(m=> !m.IsDeleted).AsNoTracking().OrderByDescending(m=> m.Id).ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                bool isExist = await _context.Categories.AnyAsync(m => m.Name.Trim() == category.Name.Trim());

                if (isExist)
                {
                    ModelState.AddModelError("Name", "Category already exist");
                    return View();
                }

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

                //change exist data is delete

                //var data = await _context.Categories.Where(m => m.IsDeleted == true).FirstOrDefaultAsync(m=>m.Name.Trim() == category.Name.Trim());

                //if(data is null)
                //{
                //    await _context.Categories.AddAsync(category);
                //}
                //else
                //{
                //    data.IsDeleted = false;
                //}
            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            var category = await _context.Categories.FirstOrDefaultAsync(m=> m.Id == id);

            if (category == null) return NotFound();
           
            return View(category);
        }

        //datadan silmek

        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    Category category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

        //    _context.Categories.Remove(category);

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            category.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Category category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

                if (category is null) return NotFound();

                return View(category);

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(category);
                }

                Category dbcategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m=> m.Id == id);

                if (dbcategory is null) return NotFound();

                if (dbcategory.Name.ToLower().Trim() == category.Name.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }

                //dbcategory.Name = category.Name;

                _context.Categories.Update(category);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }
    }
}
