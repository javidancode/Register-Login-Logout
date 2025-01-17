﻿using Entity_Frame_Work_Project.Data;
using Entity_Frame_Work_Project.Helpers;
using Entity_Frame_Work_Project.Models;
using Entity_Frame_Work_Project.ViewModels.SliderViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Entity_Frame_Work_Project.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(m => !m.IsDeleted).ToListAsync();
            return View(sliders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM slider)
        {
            if (!ModelState.IsValid) return View();

            //if (!slider.Photo.ContentType.Contains("image/"))
            //{
            //    ModelState.AddModelError("Photo", "Please choose correct image type");
            //    return View();
            //}

            foreach (var photo in slider.Photos)
            {
                if (!photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image type");
                    return View();
                }

                if (!photo.CheckFileSize(400))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image size");
                    return View();
                }
            }

            //if ((slider.Photo.Length / 1024) > 400)
            //{
            //     ModelState.AddModelError("Photo", "Please choose correct image size");
            //     return View();
            //}

            foreach (var photo in slider.Photos)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                //Path.Combine(_env.WebRootPath, "img", fileName);
                string path = Helper.GetFilePath(_env.WebRootPath, "img", fileName);

                await Helper.SaveFile(path, photo);

                Slider newSlider = new Slider
                {
                    Image = fileName
                };

                await _context.Sliders.AddAsync(newSlider);
            }
          
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return BadRequest();

            Slider slider = await GetById((int)id);

            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Slider slider)
        {
            if (id is null) return BadRequest();

            if (slider.Photo == null) return RedirectToAction(nameof(Index));

            Slider dbSlider = await GetById((int)id);

            if (dbSlider == null) return NotFound();

            if (!slider.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Please choose correct image type");
                return View(dbSlider);
            }

            if (!slider.Photo.CheckFileSize(400))
            {
                ModelState.AddModelError("Photo", "Please choose correct image size");
                return View(dbSlider);
            }

            string oldPath = Helper.GetFilePath(_env.WebRootPath, "img", dbSlider.Image);

            Helper.SliderImgDelete(oldPath);

            string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;

            string newPath = Helper.GetFilePath(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new FileStream(newPath, FileMode.Create))
            {
                await slider.Photo.CopyToAsync(stream);
            }

            dbSlider.Image = fileName;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Slider slider = await GetById(id);

            if (slider is null) return NotFound();

            string path = Helper.GetFilePath(_env.WebRootPath, "img", slider.Image);

            //if (System.IO.File.Exists(path))
            //{
            //    System.IO.File.Delete(path);
            //}

            Helper.SliderImgDelete(path);

            _context.Sliders.Remove(slider);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<Slider> GetById(int id)
        {
            return await _context.Sliders.FindAsync(id);
        }
    }
}
