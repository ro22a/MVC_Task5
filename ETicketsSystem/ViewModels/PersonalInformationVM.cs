﻿using System.ComponentModel.DataAnnotations;

namespace ETicketsSystem.ViewModels
{
	public class PersonalInformationVM
	{
		[Required]
		public string Name { get; set; } = string.Empty;
		[Required]
		public string Email { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
		public string? State { get; set; }
		public string? City { get; set; }
		public string? Street { get; set; }
		public string? ZipCode { get; set; }
		[DataType(DataType.Password)]
		public string? CurrentPassword { get; set; }
		[DataType(DataType.Password)]
		public string? NewPassword { get; set; }
	}
}
