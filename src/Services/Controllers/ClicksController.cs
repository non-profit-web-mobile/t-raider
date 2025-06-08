using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers;

[Route("clicks")]
public class ClicksController : Controller
{
	public async Task<RedirectResult> Index([FromQuery] string url)
	{
		return Redirect(url);
	}
}