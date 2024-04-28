using CsvHelper.Configuration.Attributes;

namespace DatabaseBenchmarking.Benchmarking.Models
{
    public class BenchmarkingResult
    {
        [Name("Database")]
        [NullValues("NA")]
        public string Database { get; set; }

        [Name("Runtime")]
        [NullValues("NA")]
        public string Runtime { get; set; }

        [Name("MethodName")]
        [NullValues("NA")]
        public string MethodName { get; set; }

        [Name("Op/s")]
        [NullValues("NA")]
        public float? OperationsSec { get; set; }

        [Name("Framework")]
        [NullValues("NA")]
        public string Framework { get; set; }

        [Name("Batch")]
        [NullValues("NA")]
        public int Batch { get; set; } 
    }
}
