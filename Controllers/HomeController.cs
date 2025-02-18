using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using VoyadoTechCheck.Models;

namespace VoyadoTechCheck.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
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

        ViewBag.Result = SearchGoogle(inputModel.UserInput); // inputModel.UserInput;
        return View(inputModel);
    }

    private async Task<long> SearchGoogle(string word)
    {
        string apiKey = "AIzaSyBS4B-68jVxnpB02oksdHopckI_wl8jPPU";
        string cx = "a1b368a2a39734cba";
        string url = $"https://www.googleapis.com/customsearch/v1?q={word}&key={apiKey}&cx={cx}";

        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        return json.RootElement.GetProperty("searchInformation").GetProperty("totalResults").GetInt64();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
