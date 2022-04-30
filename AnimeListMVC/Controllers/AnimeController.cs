using Microsoft.AspNetCore.Mvc;
using AnimeListMVC.Consts;
using AnimeListMVC.Models;
using AnimeListMVC.Models.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using NToastNotify;
using AnimeListMVC.ViewModels;
using AutoMapper;

namespace AnimeListMVC.Controllers
{
    public class AnimeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        

        private myConsts constObj = new myConsts();


        public AnimeController(AppDbContext context, IMapper mapper, IToastNotification toastNotification)
        {
            this._context = context;
            this._mapper = mapper;
            this._toastNotification = toastNotification;
        }

        //Sorting 
        public async Task<IActionResult> sortByName()
        {
            var animes = await _context.Animes.OrderBy(n => n.Title).ToListAsync();
            return View(nameof(Index), animes);
        }
        public async Task<IActionResult> sortByRate()
        {
            var animes = await _context.Animes.OrderByDescending(n => n.Rate).ToListAsync();
            return View(nameof(Index), animes);
        }
        public async Task<IActionResult> sortByOlder()
        {
            var animes = await _context.Animes.OrderBy(n => n.Year).ToListAsync();
            return View(nameof(Index), animes);

        }
        public async Task<IActionResult> sortByNewer()
        {
            var animes = await _context.Animes.OrderByDescending(n => n.Year).ToListAsync();
            return View(nameof(Index), animes);

        }

        //
        public async Task<IActionResult> Index()
        {
            var animes = await _context.Animes.OrderByDescending(n => n.Rate).ToListAsync();
            return View(animes);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new AnimeFormViewModel
            {
                Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync()
            };
            return View("AnimeForm", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnimeFormViewModel model)
        {
            // check if the user select a category or not
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                return View("AnimeForm", model);
            }

            // check if user select a poster or not
            // show error msg to user
            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please Select Anime Poster");
                return View("AnimeForm", model);
            }


            // check if image jpg/ png or not
            // show error msg to user

            var poster = files.FirstOrDefault();

            // check if image is jpg / png
            //var allowedExtensions = new List<string> { ".jpg", ".png" };
            if (!constObj.allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .JPG , .PNG images are allowed.");
                return View("AnimeForm", model);
            }

            // check the size of the image : OneMegaByte = 1 MB = 1048576 B
            // show error msg
            if (poster.Length > constObj.OneMegaByte)
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster Can't be more than 1 MB.");
                return View("AnimeForm", model);
            }


            // using stream
            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);

            // Mapping ( AnimeFormViewModel ==> Anime )
            var mapper = _mapper.Map<Anime>(model);
            mapper.Poster = dataStream.ToArray();

            //var anime = new Anime
            //{
            //    Title = model.Title,
            //    CategoryId = model.CategoryId,
            //    Year = model.Year,
            //    Rate = model.Rate,
            //    StoreLine = model.StoreLine,
            //    Poster = dataStream.ToArray()
            //};

            // save data to database
            //await _context.Animes.AddAsync(anime);
            await _context.Animes.AddAsync(mapper);
            await _context.SaveChangesAsync();

            // notification
            _toastNotification.AddSuccessToastMessage(mapper.Title + " Created Successfully");

            //return RedirectToAction("Index");
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var anime = await _context.Animes.SingleOrDefaultAsync(n => n.Id == id);

            if (anime == null)
                return NotFound();

            var viewModel = new AnimeFormViewModel();
            viewModel.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
            _mapper.Map<Anime>(viewModel);

            //var viewModel = new AnimeFormViewModel
            //{
            //    Id = anime.Id,
            //    Title = anime.Title,
            //    Year = anime.Year,
            //    Rate = anime.Rate,
            //    CategoryId = anime.CategoryId,
            //    StoreLine = anime.StoreLine,
            //    Poster = anime.Poster,
            //    Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync()
            //};
            return View("AnimeForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnimeFormViewModel model)
        {
            // check if the user select a category or not
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                return View("AnimeForm", model);
            }

            // if Id is correct 
            //var anime = await _context.Animes.SingleOrDefaultAsync(n => n.Id == model.Id);
            var returned = await _context.Animes.SingleOrDefaultAsync(n => n.Id == model.Id);

            if (returned == null)
                return NotFound();

            //mapping
            _mapper.Map(model, returned);

            //var mapper = _mapper.Map<Anime>(returned);

            // edit poster
            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);

                // if image has error = it sent to model (null)
                // it will refresh the same page (AnimeForm) with the same image 
                // with the error
                model.Poster = dataStream.ToArray();

                // check if image is jpg / png
                if (!constObj.allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .JPG , .PNG images are allowed.");
                    return View("AnimeForm", model);
                }

                // check the size of the image : OneMegaByte = 1 MB = 1048576 B
                // show error msg
                if (poster.Length > constObj.OneMegaByte)
                {
                    model.Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster Can't be more than 1 MB.");
                    return View("AnimeForm", model);
                }
                //anime.Poster = model.Poster;
                //mapper.Poster = model.Poster;
                var mapper = _mapper.Map<Anime>(model);

            }
            //var mapper2 = _mapper.Map<Anime>(model);

            // mapping
            //anime.Title = model.Title;
            //anime.Year = model.Year;
            //anime.Rate = model.Rate;
            //anime.CategoryId = model.CategoryId;
            //anime.StoreLine = model.StoreLine;

            await _context.SaveChangesAsync();

            // notification
            _toastNotification.AddSuccessToastMessage( "Anime updated Successfully");

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(int? id)
        {
            // check if there is an id sent or not
            if (id == null)
                return BadRequest();
            //return anime from db
            var anime = await _context.Animes.Include(n => n.Category).SingleOrDefaultAsync(n => n.Id == id);
            //check if id in db or not
            if (anime == null)
                return NotFound();
            return View(anime);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            // check if there is an id sent or not
            if (id == null)
                return BadRequest();
            //return anime from db
            var anime = await _context.Animes.SingleOrDefaultAsync(n => n.Id == id);
            //check if id in db or not
            if (anime == null)
                return NotFound();

            _context.Animes.Remove(anime);
            _context.SaveChanges();

            return Ok();
        }



    }
}
