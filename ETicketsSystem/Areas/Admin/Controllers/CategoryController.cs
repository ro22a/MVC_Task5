
using ETicketsSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ETicketsSystem.Areas.Admin.Controllers
{
	[Area(SD.AdminArea)]
	public class CategoryController : Controller
	{
		//ApplicationDbContext _context = new ApplicationDbContext();
		private readonly IRepository<Category> _categoryRepository;
		private readonly IRepository<Movie> _movieRepository;

		public CategoryController(IRepository<Category> categoryRepository,IRepository<Movie> movieRepository) 
		{
			_categoryRepository = categoryRepository;
			_movieRepository = movieRepository;
		}

		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			var categories = await _categoryRepository.GetAsync(includes: e => e.Movies);

			//Pagination
			var TotalNumOfPages = Math.Ceiling(categories.Count() / 10.0);
			ViewBag.TotalNumOfPages = TotalNumOfPages;
			var currentpage = page;
			ViewBag.CurrentPage = currentpage;
			categories =  categories.Skip((page - 1) * 10).Take(10).ToList();

			return View(categories.ToList());
		}

		[HttpGet]
		public IActionResult Create()
		{

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(Category category)
		{
			

			
			await _categoryRepository.CreateAsync(category);
			await _categoryRepository.CommitAsync();

			TempData["success-notification"] = "Add Category Successfully";

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var category =await _categoryRepository.GetOneAsync(e=>e.Id==id);
			if (category is null)
			{
				return NotFound();
			}
			return View(category);

		}

		[HttpPost]
		public async Task<IActionResult> EditAsync(Category category)
		{
			_categoryRepository.Update(category);
			await _categoryRepository.CommitAsync();

			TempData["success-notification"] = "Update Category Successfully";

			return RedirectToAction(nameof(Index));

		}

		public async Task<IActionResult> DeleteAsync(int id)
		{
			var category = await _categoryRepository.GetOneAsync(e=>e.Id== id);
			if (category is null)
			{
				return NotFound();
			}
			_categoryRepository.Delete(category);
			await _categoryRepository.CommitAsync();

			TempData["success-notification"] = "Delete Category Successfully";

			return RedirectToAction(nameof(Index));
		}
	}
}
