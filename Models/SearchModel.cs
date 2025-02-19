namespace VoyadoTechCheck.Models
{
    public class SearchModel
    {
        public string UserInput { get; set; } = "";

        public long TotalGoogleSearchHits { get; set; } = 0;
        public long TotalAlgoliaSearchHits { get; set; } = 0;
    }
}
