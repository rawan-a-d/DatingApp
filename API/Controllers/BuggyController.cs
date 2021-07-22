using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	/// <summary>
	/// Return errors to see what we get back from different responses
	/// </summary>
	public class BuggyController : BaseApiController
	{
		private readonly DataContext _context;

		public BuggyController(DataContext context)
		{
			_context = context;
		}
		
		[Authorize]
		[HttpGet("auth")]
		public ActionResult<string> GetSecret() {
			return "secret text";
		}

		[HttpGet("not-found")]
		public ActionResult<AppUser> GetNotFound()
		{
			var thing = _context.Users.Find(-1);
			
			if(thing == null) {
				return NotFound();
			}
			return Ok(thing);
		}

		[HttpGet("server-error")]
		public ActionResult<string> GetSeverError()
		{
			var thing = _context.Users.Find(-1);

			// generate Exception (null reference exception)
			var thingToReturn = thing.ToString();

			return thingToReturn;
		}

		[HttpGet("bad-request")]
		public ActionResult<string> GetBadRequest()
		{
			return BadRequest("This was not a good request");
		}
	}
}