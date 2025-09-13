using ETicketsSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ETicketsSystem.Areas.Admin.Controllers
{
	[Area(SD.AdminArea)]
	public class ActorController : Controller
	{
		//ApplicationDbContext _context = new ApplicationDbContext();
		private readonly IRepository<Actor> _actorRepository;
		private readonly IRepository<Category> _categoryRepository;
		private readonly IRepository<Movie> _movieRepository;

		public ActorController(IRepository<Actor> actorRepository, IRepository<Category> categoryRepository, IRepository<Movie> movieRepository)
		{
			_actorRepository = actorRepository;
			_categoryRepository = categoryRepository;
			_movieRepository = movieRepository;
		}

		public async Task<IActionResult> IndexAsync(int page=1)
		{
			var actors = await _actorRepository.GetAsync(includes: e => e.Movies);

			//Pagination
			var TotalNumOfPages = Math.Ceiling(actors.Count() / 10.0);
			ViewBag.TotalNumOfPages = TotalNumOfPages;
			var currentpage = page;
			ViewBag.CurrentPage = currentpage;
			actors = actors.Skip((page - 1) * 10).Take(10).ToList();
			return View(actors.ToList());
		}
		[HttpGet]
		public IActionResult Create() 
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateAsync(Actor actor, IFormFile? ProfilePicture)
		{


			if (ProfilePicture is null)
			{
				return BadRequest();
			}
			if (ProfilePicture.Length > 0)
			{
				//Save img in wwwroot
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", fileName);

				using (var stream = System.IO.File.Create(filePath))
				{
					ProfilePicture.CopyTo(stream);
				}

				//Save img in DB
				actor.ProfilePicture = fileName;
			}



			await _actorRepository.CreateAsync(actor);
			await _actorRepository.CommitAsync();

			TempData["success-notification"] = "Add Actor Successfully";

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]

		public async Task<IActionResult> EditAsync(int id)
		{
			var actor = await _actorRepository.GetOneAsync(e=>e.Id==id);
			if (actor is null)
			{
				return NotFound();
			}
			return View(actor);
		}

		[HttpPost]
		public async Task<IActionResult> EditAsync(Actor actor, IFormFile?  ProfilePicture)
		{
			var actorInDb = await _actorRepository.GetOneAsync(e => e.Id == actor.Id, tracked: false);
			if (actorInDb == null)
			{
				return NotFound();
			}
			if (ProfilePicture is not null)
			{

				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", fileName);

				using (var stream = System.IO.File.Create(filePath))
				{
					ProfilePicture.CopyTo(stream);
				}

				//Remove old img from wwwroot
				var oldFileName = actorInDb.ProfilePicture;
				var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", oldFileName);
				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
				}

				//Save img in DB
				actor.ProfilePicture = fileName;
			}
			else
			{
				actor.ProfilePicture = actorInDb.ProfilePicture;
			}

			_actorRepository.Update(actor);
			await _actorRepository.CommitAsync();

			TempData["success-notification"] = "Update Actor Successfully";

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(int id)
		{
			var actor =await  _actorRepository.GetOneAsync(e=>e.Id == id);
			if (actor is null)
			{
				return NotFound();
			}
			_actorRepository.Delete(actor);
			await _actorRepository.CommitAsync();

			TempData["success-notification"] = "Delete Actor Successfully";

			return RedirectToAction(nameof(Index));
		}


	}
}
