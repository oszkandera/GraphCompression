namespace GraphCompression.Models
{
    public class AnalyzeData
    {
        public string Name { get; set; }
        public int NodeCount { get; set; }
        public int EdgeCount { get; set; }
        public int MaxDegree { get; set; }
        public int AverageDegree { get; set; }
        public string FilePath { get; set; }
    }
}
