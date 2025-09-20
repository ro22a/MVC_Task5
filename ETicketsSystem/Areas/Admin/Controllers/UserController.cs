﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETicketsSystem.Areas.Admin.Controllers
{
	[Area(SD.AdminArea)]
	[Authorize(Roles = $"{SD.SuperAdminRole}")]
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		public UserController(UserManager<ApplicationUser> userManager) 
		{
			_userManager = userManager;
		}
		public IActionResult Index()
		{
			var users=_userManager.Users;
			return View(users.ToList());
		}
	}
}
