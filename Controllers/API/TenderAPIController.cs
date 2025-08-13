using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Testovoe.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/tenders")]
    public class TenderAPIController : ControllerBase
    {
        private readonly TenderService _tenderService;
        private readonly ILogger<TenderAPIController> _logger;

        public TenderAPIController(TenderService tenderService, ILogger<TenderAPIController> logger)
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
