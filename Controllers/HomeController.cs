using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VoyadoTechCheck.Models;

// Algolia API
using Algolia.Search.Clients;
using Algolia.Search.Models.Search;

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
            searchModel.TotalAlgoliaSearchHits += await SearchAlgolia(word);
        }

        return View(searchModel);
    }

    // SearchAlgolia - Calls Algolia Search API and return number of hits
    // Doc: https://www.algolia.com/doc/libraries/csharp/v7/
    private async Task<long> SearchAlgolia(string word)
    {
        // Setup Algolia API to get AppID and api key
        // https://dashboard.algolia.com/apps/1SQES8SXAC/dashboard
        string ApplicationId = "1SQES8SXAC";
        string apiKey = "1c3400b473280bce2cbf095fb170baa1";

        // Index is loaded with the following dataset
        // https://www.kaggle.com/datasets/krishnanshverma/imdb-movies-dataset
        string IndexName = "VoyadoTechCheck";

        // Setup Algolia SearchClient
        var client = new SearchClient(ApplicationId, apiKey);

        // Search for word in with Algolia Search API
        var searchResults = client.SearchSingleIndex<Hit>(IndexName, new SearchParams(new SearchParamsObject { Query = word }));

        // Return search hits
        return searchResults.Hits.Count;
    }

    // SearchGoogle - Calls Google Custom Search API and return parsed result
    // Doc: https://developers.google.com/custom-search/v1/overview
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
