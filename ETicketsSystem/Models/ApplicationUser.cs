﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ETicketsSystem.Models
{
	public class ApplicationUser : IdentityUser
	{

		[Required]
		public string Name { get; set; } = string.Empty;
		public string?	State { get; set; }

		public string? City { get; set; }

		public string? Street { get; set; }

		public string? Zipcode { get; set; }
	}
}
