using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CodingMilitia.AspNetCoreMvcActionCancellationTokenSample.Controllers
{
    [Produces("application/json")]
    [Route("api/sample")]
    public class SampleController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SampleController> _logger;

        public SampleController(HttpClient httpClient, ILogger<SampleController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [Route("thing")]
        public async Task<IActionResult> GetAThingAsync(CancellationToken ct)
        {
            try
            {
                await _httpClient.GetAsync("http://httpstat.us/204?sleep=5000", ct);
                _logger.LogInformation("Task completed!");
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Task canceled!");
                
            }
            return NoContent();
        }

        [Route("anotherthing")]
        public async Task<IActionResult> GetAnotherThingAsync(CancellationToken ct)
        {
            try
            {
                for(var i = 0; i < 5; ++i)
                {
                    ct.ThrowIfCancellationRequested();
                    //do stuff...
                    await Task.Delay(1000, ct);
                }
                _logger.LogInformation("Process completed!");
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
            {
                _logger.LogInformation("Process canceled!");
                
            }
            return NoContent();
        }
    }
}