using DatabaseBenchmarking.Benchmarking.Models;

namespace DatabaseBenchmarking.Benchmarking
{
    public interface IBenchmarkingFilesService
    {
        IEnumerable<BenchmarkingResult> GetBenchmarkingResults();
    }
}