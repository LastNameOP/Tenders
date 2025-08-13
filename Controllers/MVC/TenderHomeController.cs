using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Testovoe.Models;

namespace Testovoe.Controllers
{
    public class TenderHomeController : Controller
    {
        private readonly ILogger<TenderHomeController> _logger;
        private readonly TenderClientService _clientService;

        public TenderHomeController(ILogger<TenderHomeController> logger, TenderClientService clientService)
        {
            _logger = logger;
            _clientService = clientService;
        }
        public async Task<IActionResult> Index()
        {
            var tenders = await _clientService.GetTendersAsync();
            return View(tenders);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
