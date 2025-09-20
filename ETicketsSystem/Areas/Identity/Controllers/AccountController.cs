using ETicketsSystem.Areas.Admin.Controllers;
using ETicketsSystem.Models;
using ETicketsSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;

namespace ETicketsSystem.Areas.Identity.Controllers
{
	[Area(SD.IdentityArea)]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IEmailSender _emailSender;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IRepository<UserOTP> _userOTP;
		public AccountController(UserManager<ApplicationUser> userManager,IEmailSender emailSender,
			SignInManager<ApplicationUser> signInManager,IRepository<UserOTP> userOTP)
		{
			_userManager=userManager;
			_emailSender=emailSender;
			_signInManager=signInManager;
			_userOTP=userOTP;
			
		}
		[HttpGet]
		public IActionResult Register()
		{
			if (User.Identity is not null && User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home", new { area = "User" });
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM registerVM)
		{
			if (!ModelState.IsValid)
			{
				return View(registerVM);
			}

			ApplicationUser applicationUser = new()
			{
				Name = registerVM.Name,
				UserName = registerVM.UserName,
				Email = registerVM.Email,
			};

			var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);

			if (!result.Succeeded)
			{
				foreach (var item in result.Errors)
				{
					ModelState.AddModelError(string.Empty, item.Description);
				}
				return View(registerVM);
			}

			//Send Email Confirmation
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
			var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", UserId = applicationUser.Id ,Token=token},Request.Scheme);
			await _emailSender.SendEmailAsync(applicationUser.Email,"Confirm Your Account!", $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

			TempData["success-notification"] = "Add User Successfully,Please Confirm Your Account";
			return RedirectToAction("Login");
		}
		public async Task<IActionResult> ConfirmEmail(string userId,string token)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if(user is null)
			{
				return NotFound();
			}
			var result=await _userManager.ConfirmEmailAsync(user, token);

			if (!result.Succeeded)
			{
				TempData["error-notification"] = "Invalid Token ,Resend Email Confirmation";
			}
			else
			{
				TempData["success-notification"] = "Activate Account Successfully";
			}
			return RedirectToAction("Login");
		}

		[HttpGet]
		public IActionResult Login()
		{
			if (User.Identity is not null && User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home", new { area = "User" });
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginVM loginVM)
		{
			if (!ModelState.IsValid)
			{
				return View(loginVM);
			}

			var user=await _userManager.FindByNameAsync(loginVM.EmailORUserName) ?? 
				await _userManager.FindByEmailAsync(loginVM.EmailORUserName);


			if (user is null)
			{
				TempData["error-notification"] = "Invalid User Name/Email Or Password!";
				return View(loginVM);
			}

			var result=await _userManager.CheckPasswordAsync(user, loginVM.Password);
			if (!result)
			{
				TempData["error-notification"] = "Invalid User Name/Email Or Password!";
				return View(loginVM);
			}

			if (!user.EmailConfirmed)
			{
				TempData["error-notification"] = "Please Confirm Your Account!";
				return View(loginVM);

			}

			if (!user.LockoutEnabled)
			{
				TempData["error-notification"] =$"You have a block till {user.LockoutEnd}";
				return View(loginVM);
			}

			await _signInManager.SignInAsync(user, loginVM.RememberMe);

			TempData["success-notification"] = "Login Successfully";

			return RedirectToAction("Index", "Home", new { area="User" });

		}

		public IActionResult LogOut()
		{
			_signInManager.SignOutAsync();
			TempData["Success-notification"] = "Logout Successfully";
			return RedirectToAction("Login", "Account", new {area="Identity"});
		}

		[HttpGet]
		public IActionResult ResendEmailConfirmation()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ResendEmailConfirmationAsync(ResendEmailConfirmationVM resendEmailConfirmationVM)
		{
			if (!ModelState.IsValid)
			{
				return View(resendEmailConfirmationVM);

			}

			var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailORUserName) ??
				await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailORUserName);


			if (user is null)
			{
				TempData["error-notification"] = "Invalid User Name/Email!";
				return View(resendEmailConfirmationVM);
			}

			//Send Email Confirmation
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", UserId = user.Id, Token = token }, Request.Scheme);
			await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Account!", $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

			TempData["success-notification"] = "Send Email Successfully,Please Confirm Your Account";
			return RedirectToAction("Login");
		}

		[HttpGet]
		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
		{
			if (!ModelState.IsValid)
			{
				return View(forgetPasswordVM);

			}

			var user = await _userManager.FindByNameAsync(forgetPasswordVM.EmailORUserName) ??
				await _userManager.FindByEmailAsync(forgetPasswordVM.EmailORUserName);


			if (user is null)
			{
				TempData["error-notification"] = "Invalid User Name/Email!";
				return View(forgetPasswordVM);
			}

			//Send Email Confirmation
			//var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var OTPNumber=new Random().Next(1000,9999);


			await _emailSender.SendEmailAsync(user.Email!, "Reset Your Password!",
				$"Use this OTP Number: <b>{OTPNumber}</b> to reset your password. Don't Share it.");

			await _userOTP.CreateAsync(new UserOTP()
			{
				ApplicationUserId = user.Id,
				OTPNumber=OTPNumber.ToString(),
				ValidTo=DateTime.UtcNow.AddDays(1)
			});
			await _userOTP.CommitAsync();

			TempData["success-notification"] = "Send OTP to your Email Successfully,Please Check Your Email";
			return RedirectToAction("ConfirmOTP","Account",new {area="Identity",userId=user.Id});
		}

		[HttpGet]
		public IActionResult ConfirmOTP(string userId)
		{
			return View(new ConfirmOTPVM()
			{
				ApplicationUserId = userId
			});
		}
		[HttpPost]
		public async Task<IActionResult> ConfirmOTP(ConfirmOTPVM confirmOTPVM)
		{
			if (!ModelState.IsValid)
			{
				return View(confirmOTPVM);

			}
			var user = await _userManager.FindByIdAsync(confirmOTPVM.ApplicationUserId);

			if (user is null)
			{
				return NotFound();
			}

			var lastOTP = (await _userOTP.GetAsync(e => e.ApplicationUserId ==
			confirmOTPVM.ApplicationUserId)).OrderBy(e => e.Id).LastOrDefault();

			if (lastOTP is null)
			{
				return NotFound();
			}
			if (lastOTP.OTPNumber == confirmOTPVM.OTPNumber && lastOTP.ValidTo > DateTime.UtcNow)
			{
				return RedirectToAction("NewPassword", "Account", new
				{
					area = "Identity",
					userId = user.Id
				});
			}
			TempData["error-notification"] = "Invalid OTP Number!";
			return RedirectToAction("ConfirmOTP", "Account", new { area = "Identity",
				userId = confirmOTPVM.ApplicationUserId});
		}

		[HttpGet]
		public IActionResult NewPassword(string userId)
		{
			return View(new NewPasswordVM()
			{
				ApplicationUserId = userId
			});
		}

		[HttpPost]
		public async Task<IActionResult> NewPasswordAsync(NewPasswordVM newPasswordVM)
		{
			if (!ModelState.IsValid)
			{
				return View(newPasswordVM);

			}
			var user = await _userManager.FindByIdAsync(newPasswordVM.ApplicationUserId);
			if (user is null) 
			{
				return NotFound();

			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var result=await _userManager.ResetPasswordAsync(user, token,newPasswordVM.Password);

			if (!result.Succeeded)
			{
				foreach (var item in result.Errors)
				{
					ModelState.AddModelError(string.Empty, item.Description);
				}
				return View(newPasswordVM);
			}
			TempData["success-notification"] = "Reset Password Successfully";
			return RedirectToAction("Login");
			


		}
	}
}
