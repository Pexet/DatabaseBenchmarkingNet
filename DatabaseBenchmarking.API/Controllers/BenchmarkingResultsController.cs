using DatabaseBenchmarking.Benchmarking;
using DatabaseBenchmarking.Benchmarking.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseBenchmarking.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BenchmarkingResultsController : ControllerBase
    {
        private readonly ILogger<BenchmarkingResultsController> _logger;
        private readonly IBenchmarkingFilesService _benchmarkingFilesService;

        public BenchmarkingResultsController(ILogger<BenchmarkingResultsController> logger, IBenchmarkingFilesService benchmarkingFilesService)
        {
            _logger = logger;
            _benchmarkingFilesService = benchmarkingFilesService;
        }

        [HttpGet(Name = "get")]
        public async Task<IEnumerable<BenchmarkingResult>> Get()
        {
            return _benchmarkingFilesService.GetBenchmarkingResults();
        }
    }
}
