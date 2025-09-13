using ETicketsSystem.Data;
using ETicketsSystem.Models;
using ETicketsSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace ETicketsSystem.Areas.Admin.Controllers
{
	[Area(SD.AdminArea)]
	public class MovieController : Controller
	{
		

		//ApplicationDbContext _context=new ApplicationDbContext();

		private readonly IRepository<Movie> _movieRepository;
		private readonly IRepository<Category> _categoryRepository;
		private readonly IRepository<Cinema> _cinemaRepository;

		public MovieController(IRepository<Movie> movieRepository,IRepository<Category> categoryRepository,
			IRepository<Cinema> cinemaRepository)
		{
			_movieRepository = movieRepository;
			_categoryRepository = categoryRepository;
			_cinemaRepository = cinemaRepository;
		}

		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			var movies = await _movieRepository.GetAsync(null,true,e => e.Category, e => e.Cinema);

			//Pagination
			var TotalNumOfPages = Math.Ceiling(movies.Count() / 10.0);
			ViewBag.TotalNumOfPages = TotalNumOfPages;
			var currentpage = page;
			ViewBag.CurrentPage = currentpage;
			movies = movies.Skip((page - 1) * 10).Take(10).ToList();



			return View(movies.ToList());
		}

		[HttpGet]
		public async Task<IActionResult> CreateAsync()
		{
			var categories = await _categoryRepository.GetAsync();
			var cinemas = await _cinemaRepository.GetAsync();
		

			return View(new CategoryWithCinemaVM()
			{
				Categories = categories.ToList(),
				Cinemas = cinemas.ToList(),
				
			});
		}

		[HttpPost]

		public async Task<IActionResult> CreateAsync(Movie movie,IFormFile? ImgUrl) 
		{
		

			if (ImgUrl is null)
			{
				return BadRequest();
			}
			if (ImgUrl.Length > 0)
			{
				//Save img in wwwroot
				var fileName =Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
				var filePath=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images\\movies", fileName);
				
				using (var stream = System.IO.File.Create(filePath))
				{
					ImgUrl.CopyTo(stream);
				}

				//Save img in DB
				movie.ImgUrl = fileName;
			}

			

			await _movieRepository.CreateAsync(movie);
			await _movieRepository.CommitAsync();

		


			TempData["success-notification"] = "Add Movie Successfully";

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]

		public async Task<IActionResult> EditAsync(int id)
		{
			var movie = await _movieRepository.GetOneAsync(e =>e.Id == id);
			if (movie is null)
			{
				return NotFound();
			}
			var categories=await _categoryRepository.GetAsync();
			var cinemas=await _cinemaRepository.GetAsync();
			return View(new CategoryWithCinemaVM()
			{
				Categories = categories.ToList(),
				Cinemas = cinemas.ToList(),
				Movie=movie

			});
		}

		[HttpPost]
		public async Task<IActionResult> EditAsync(Movie movie, IFormFile? ImgUrl)
		{
			var movieInDb = await _movieRepository.GetOneAsync(e => e.Id == movie.Id, tracked: false);
			if (movieInDb == null)
			{
				return NotFound();
			}
			if (ImgUrl is not null)
			{
				
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);

				using (var stream = System.IO.File.Create(filePath))
				{
					ImgUrl.CopyTo(stream);
				}

				//Remove old img from wwwroot
				var oldFileName = movieInDb.ImgUrl;
				var oldFilePath= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", oldFileName);
				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
				}
				
					//Save img in DB
					movie.ImgUrl = fileName;
			}
			else
			{
				movie.ImgUrl = movieInDb.ImgUrl;
			}

			_movieRepository.Update(movie);
			await _movieRepository.CommitAsync();

			TempData["success-notification"] = "Update Movie Successfully";

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> DeleteAsync(int id)
		{
			var movie =await _movieRepository.GetOneAsync(e=>e.Id==id);
			if( movie is null)
			{
				return NotFound();
			}
			_movieRepository.Delete(movie);
			await _movieRepository.CommitAsync();

			TempData["success-notification"] = "Delete Movie Successfully";

			return RedirectToAction(nameof(Index));
		}

			

		
	}
}
