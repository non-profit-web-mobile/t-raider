using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers;

[Route("c")]
public class RedirectController : Controller
{
    [HttpGet]
    public async Task RedirectAsync([FromQuery(Name = "u")] string redirectUri)
    {
        HttpContext.Response.Redirect(redirectUri, false);
        await HttpContext.Response.CompleteAsync();
    }
}