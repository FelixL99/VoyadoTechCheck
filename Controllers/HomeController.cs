using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using VoyadoTechCheck.Models;
using System.Runtime.InteropServices;
using System.Diagnostics.Eventing.Reader;

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
        return View(new SearchModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(SearchModel searchModel)
    {
        // Check if user input is valid
        if (string.IsNullOrWhiteSpace(searchModel.UserInput))
        {
            ModelState.AddModelError("UserInput", "Please enter valid input.");
            return View(searchModel);
        }

        // Split input
        string[] words = searchModel.UserInput.Split(' ');

        // Call SearchGoogle for each word
        foreach (string word in words)
        {
            searchModel.TotalGoogleSearchHits += await SearchGoogle(word);
        }

        return View(searchModel);
    }

    // SearchGoogle - Calls Google Custom Search API and return parsed result
    private async Task<long> SearchGoogle(string word)
    {
        // Setup Custom Search JSON API to get apiKey and cx and url
        // https://developers.google.com/custom-search/v1/overview

        string apiKey = "AIzaSyBS4B-68jVxnpB02oksdHopckI_wl8jPPU";
        string cx = "a1b368a2a39734cba";
        string url = $"https://www.googleapis.com/customsearch/v1?q={word}&key={apiKey}&cx={cx}";

        // Call API and get search content
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        
        // Parse JSON result file
        var json = JsonDocument.Parse(content);
        var resultString = json.RootElement
            .GetProperty("searchInformation")
            .GetProperty("totalResults")
            .GetString();

        // Return result if valid, otherwise return 0
        return long.TryParse(resultString, out var result) ? result : 0;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
