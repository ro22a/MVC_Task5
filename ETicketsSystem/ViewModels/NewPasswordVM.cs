using System.ComponentModel.DataAnnotations;

namespace ETicketsSystem.ViewModels
{
	public class NewPasswordVM
	{
		public int Id { get; set; }
		public string Password { get; set; } = string.Empty;
		[Required, DataType(DataType.Password), Compare(nameof(Password))]
		public string ConfirmPassword { get; set; } = string.Empty;
		public string ApplicationUserId { get; set; } = string.Empty;
	}
}