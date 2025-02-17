using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VoyadoTechCheck.Models;

namespace VoyadoTechCheck.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new InputModel());
    }

    [HttpPost]
    public IActionResult Index(InputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.UserInput))
        {
            ModelState.AddModelError("UserInput", "Please enter valid input.");
            return View(inputModel);
        }

        ViewBag.Result = inputModel.UserInput;
        return View(inputModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
