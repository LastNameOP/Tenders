using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Testovoe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TendersController : ControllerBase
    {
        private readonly TenderService _tenderService;
        private readonly ILogger<TendersController> _logger;

        public TendersController(TenderService tenderService, ILogger<TendersController> logger)
        {
            _tenderService = tenderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var tenders = await _tenderService.GetAllAsync(ct);
            return Ok(tenders);
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "ok" });
    }
}
