using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters;

namespace DatabaseBenchmarking.Benchmarking.Models
{
    public class CustomBenchmarkingConfig : ManualConfig
    {
        public CustomBenchmarkingConfig()
        {
            AddColumn(
                StatisticColumn.OperationsPerSecond,
                StatisticColumn.P0,
                StatisticColumn.P25,
                StatisticColumn.P67,
                StatisticColumn.P100);
            AddExporter(MarkdownExporter.Default);
            AddExporter(CsvMeasurementsExporter.Default);
            AddExporter(RPlotExporter.Default);

            AddColumn(new TagColumn("MethodName", name => name.Split("_")[0]));
            AddColumn(new TagColumn("Batch", name => name.Split("_")[1]));
            AddColumn(new TagColumn("Framework", name => name.Split("_")[2]));
            AddColumn(new TagColumn("Database", name => name.Split("_")[3]));
        }
    }
}
