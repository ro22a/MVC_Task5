﻿using System.ComponentModel.DataAnnotations;

namespace ETicketsSystem.ViewModels
{
	public class ConfirmOTPVM
	{
		public int Id { get; set; }
		[Required]
		public string OTPNumber { get; set; } = string.Empty;

		public string ApplicationUserId { get; set; }= string.Empty;
		
	}
}
