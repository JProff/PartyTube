using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PartyTube.Service.Interfaces;

namespace PartyTube.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Search/[action]")]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        [ActionName("popup")]
        public async Task<IActionResult> GetPopupAsync(string term)
        {
            var result = await _searchService.GetSearchPopupResultsAsync(term).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("local")]
        public async Task<IActionResult> GetLocalAsync(string term)
        {
            var videoItems = await _searchService.GetLocalSearchVideoItemsAsync(term).ConfigureAwait(false);
            var result = videoItems.ToArray();

            if (result.Length == 0)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [ActionName("external")]
        public async Task<IActionResult> GetExternalAsync(string term, int count, string pageToken)
        {
            var result = await _searchService.GetYoutubeSearchResultAsync(term, count, pageToken).ConfigureAwait(false);

            if (result.Videos.Length == 0)
                return NotFound();

            return Ok(result);
        }
    }
}