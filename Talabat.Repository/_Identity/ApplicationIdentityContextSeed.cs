using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository._Identity
{
	public static class ApplicationIdentityContextSeed
	{
		public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
		{
			if(!userManager.Users.Any())
			{
				var user = new ApplicationUser()
				{
					DisplayName = "Nahla Hesham",
					Email = "nahla@gmail.com",
					UserName = "nahla.hesham",
					PhoneNumber = "02223"
				};

				await userManager.CreateAsync(user,"P@ssw0rd");
			}
		}
	}
}
