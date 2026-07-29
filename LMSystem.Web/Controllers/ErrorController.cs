using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Web.Controllers;

[Route("Error")]
public class ErrorController : Controller
{
    [Route("{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        switch (statusCode)
        {
            case 404:
                return View("NotFound");
            case 403:
                return View("AccessDenied");
            default:
                return View("General");
        }
    }

    [Route("500")]
    public IActionResult Error500()
    {
        return View("General");
    }
}
