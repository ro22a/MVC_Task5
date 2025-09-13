using ETicketsSystem.Data;
using ETicketsSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ETicketsSystem.Areas.Admin.Controllers
{
	[Area(SD.AdminArea)]
	public class CinemaController : Controller
	{
		//ApplicationDbContext _context=new ApplicationDbContext();
		private readonly IRepository<Cinema> _cinemaRepository;
		private readonly IRepository<Movie> _movieRepository;

		public CinemaController(IRepository<Cinema> cinemaRepository, IRepository<Movie> movieRepository)
		{
			_cinemaRepository = cinemaRepository;
			_movieRepository = movieRepository;
		}

		public async Task<IActionResult> IndexAsync(int page=1)
		{
			var cinemas =await _cinemaRepository.GetAsync(includes: e => e.Movies);

			//Pagination
			var TotalNumOfPages = Math.Ceiling(cinemas.Count() / 10.0);
			ViewBag.TotalNumOfPages = TotalNumOfPages;
			var currentpage = page;
			ViewBag.CurrentPage = currentpage;
			cinemas =cinemas.Skip((page - 1) * 10).Take(10).ToList();

			return View(cinemas.ToList());
		}

		[HttpGet]
		public IActionResult Create()
		{

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(Cinema cinema, IFormFile? CinemaLogo)
		{


			if (CinemaLogo != null && CinemaLogo.Length > 0)
			{
				//Save img in wwwroot
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CinemaLogo.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

				using (var stream = System.IO.File.Create(filePath))
				{
					await CinemaLogo.CopyToAsync(stream);
				}

				//Save img in DB
				cinema.CinemaLogo = fileName;
			}

			await _cinemaRepository.CreateAsync(cinema);
			await _cinemaRepository.CommitAsync();

			TempData["success-notification"] = "Add Cinema Successfully";

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> EditAsync(int id)
		{ 
			var cinema=await _cinemaRepository.GetOneAsync(e=>e.Id == id);
			if(cinema is null)
			{
				return NotFound();
			}
			return View(cinema);

		}

		[HttpPost]
		public async Task<IActionResult> Edit(Cinema cinema, IFormFile? CinemaLogo)
		{
				var cinemaInDb = await _cinemaRepository.GetOneAsync(e => e.Id == cinema.Id,tracked:false);
			if (CinemaLogo is not null)
			{

				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CinemaLogo.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

				using (var stream = System.IO.File.Create(filePath))
				{
					CinemaLogo.CopyTo(stream);
				}
				if (!string.IsNullOrEmpty(cinemaInDb.CinemaLogo))
				{
					//Remove old img from wwwroot
					var oldFileName = cinemaInDb.CinemaLogo;
					var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldFileName);
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}
					
					

				//Save img in DB
				cinema.CinemaLogo = fileName;
			}
			else
			{
				cinema.CinemaLogo = cinemaInDb.CinemaLogo;
			}

			_cinemaRepository.Update(cinema);
			await _cinemaRepository.CommitAsync();

			TempData["success-notification"] = "Update Cinema Successfully";

			return RedirectToAction(nameof(Index));

		}

		public async Task<IActionResult> Delete(int id)
		{
			var cinema=await _cinemaRepository.GetOneAsync(e=>e.Id == id);
			if(cinema is null)
			{
				return NotFound();
			}
			_cinemaRepository.Delete(cinema);
			await _cinemaRepository.CommitAsync();

			TempData["success-notification"] = "Delete Cinema Successfully";

			return RedirectToAction(nameof(Index));
		}

	}
}
