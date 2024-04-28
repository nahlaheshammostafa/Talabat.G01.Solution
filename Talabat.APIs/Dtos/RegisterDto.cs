using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
	public class RegisterDto
	{
		[Required]
		public string DisplayName { get; set; } = null!;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;

		[Required]
		public string Phone { get; set; } = null!;

		[Required]
		[RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{&quot;:;'?/&gt;,])(?!.*\\s).*$",
			ErrorMessage = "Password must have at least 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters" )]
		public string Password { get; set; } = null!;
	}
}
