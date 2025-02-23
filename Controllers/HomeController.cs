using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VoyadoTechCheck.Models;

// Algolia API
using Algolia.Search.Clients;
using Algolia.Search.Models.Search;

// Google Custom Search API
using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.Services;

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
        return View(new SearchModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(SearchModel searchModel)
    {
        // Check if user input is valid
        if (string.IsNullOrWhiteSpace(searchModel.UserInput))
        {
            // UserInput not valid
            return View(searchModel);
        }

        string[] words = searchModel.UserInput.Split(' ');

        foreach (string word in words)
        {
            searchModel.TotalGoogleSearchHits += await SearchGoogle(word);
            searchModel.TotalAlgoliaSearchHits += await SearchAlgolia(word);
        }
        return View(searchModel);
    }

    // SearchAlgolia - Calls Algolia Search API and return number of hits
    // Doc: https://www.algolia.com/doc/libraries/csharp/v7/
    // Setup Algolia API to get AppID and api key
    // https://dashboard.algolia.com/apps/1SQES8SXAC/dashboard
    // Index is loaded with the following dataset
    // https://www.kaggle.com/datasets/krishnanshverma/imdb-movies-dataset
    private async Task<long> SearchAlgolia(string word)
    {
        const string ApplicationId = "1SQES8SXAC";
        const string apiKey = "1c3400b473280bce2cbf095fb170baa1";
        const string IndexName = "VoyadoTechCheck";

        var client = new SearchClient(ApplicationId, apiKey);
        var searchResults = await client.SearchSingleIndexAsync<Hit>(IndexName, 
            new SearchParams(new SearchParamsObject { Query = word }));

        return searchResults.Hits.Count;
    }

    // SearchGoogle - Calls Google Custom Search API and return parsed result
    // Doc: https://developers.google.com/custom-search/v1/overview
    // https://developers.google.com/custom-search/v1/reference/rest
    // https://blog.expertrec.com/google-custom-search-in-c/
    // Setup Custom Search JSON API to get apiKey and cx and url
    // https://developers.google.com/custom-search/v1/overview
    private async Task<long> SearchGoogle(string word)
    {
        const string apiKey = "AIzaSyBS4B-68jVxnpB02oksdHopckI_wl8jPPU";
        const string ApplicationId = "a1b368a2a39734cba";

        var customSearch = new CustomSearchAPIService(new BaseClientService.Initializer { 
            ApiKey = apiKey, 
            ApplicationName = ApplicationId
        });

        var listRequest = customSearch.Cse.List();
        listRequest.Cx = ApplicationId;
        listRequest.Q = word; // Query

        var search = await listRequest.ExecuteAsync();

        return long.TryParse(search.SearchInformation.TotalResults, out var result) ? result : 0; 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
