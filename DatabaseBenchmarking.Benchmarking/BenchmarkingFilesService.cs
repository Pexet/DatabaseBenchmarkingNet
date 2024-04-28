using CsvHelper;
using DatabaseBenchmarking.Benchmarking.Models;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseBenchmarking.Benchmarking
{
    public class BenchmarkingFilesService: IBenchmarkingFilesService
    {
        public IEnumerable<BenchmarkingResult> GetBenchmarkingResults() 
        {
            using (var reader = new StreamReader("C:\\Users\\qwerh\\OneDrive\\Рабочий стол\\BenchmarkDotNet.Artifacts\\results\\DatabaseBenchmarking.Benchmarking.StudentRepositoryBenchmark-report.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<BenchmarkingResult>().ToList();
            }
        }

    }
}
