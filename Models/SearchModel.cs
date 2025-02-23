using System.ComponentModel.DataAnnotations;

namespace VoyadoTechCheck.Models
{
    public class SearchModel
    {
        [Required(ErrorMessage = "Please enter valid input.")]
        public string UserInput { get; set; } = "";

        public long TotalGoogleSearchHits { get; set; } = 0;
        public long TotalAlgoliaSearchHits { get; set; } = 0;
    }
}
