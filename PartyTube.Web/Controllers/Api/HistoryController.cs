using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using PartyTube.Service.Interfaces;

namespace PartyTube.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/History/[action]")]
    public class HistoryController : Controller
    {
        [NotNull] private readonly IHistoryService _historyService;

        public HistoryController([NotNull] IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        [ActionName("all")]
        public async Task<IActionResult> GetAllAsync(int skip, int take)
        {
            var result = await _historyService.GetAllAsync(skip, take).ConfigureAwait(false);

            if (result.Length == 0)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        [ActionName("ById")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _historyService.DeleteByIdAsync(id).ConfigureAwait(false);
            return Ok();
        }

        [HttpDelete]
        [ActionName("ByVideoIdentifier")]
        public async Task<IActionResult> DeleteByVideoIdentifier(string id)
        {
            await _historyService.DeleteByVideoIdentifierAsync(id).ConfigureAwait(false);
            return Ok();
        }
    }
}