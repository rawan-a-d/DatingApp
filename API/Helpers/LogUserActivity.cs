using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
	// Change lastActive status for a user
	public class LogUserActivity : IAsyncActionFilter
	{
        /// <summary>
        /// After the request is completed, update last active for current user
        /// </summary>
        /// <param name="context">context before a request is executed</param>
        /// <param name="next">context after a request is executed</param>
        /// <returns></returns>
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var resultContext = await next();

            // if user is not authenticated (no token)
            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) {
				return;
			}

            // get userId
			var userId = resultContext.HttpContext.User.GetUserId();

            // user repository
			var repo = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();

            // get user
			var user = await repo.UserRepository.GetUserByIdAsync(userId);

			// update last active
			user.LastActive = DateTime.UtcNow;

			// save to db
			await repo.Complete();
		}
	}
}