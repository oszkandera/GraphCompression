using System;

namespace GraphCompression.Models
{
    public class AnalyzeResult
    {
        public AnalyzeData AnalyzeData { get; set; }
        public int OriginalSize { get; set; }
        public int CompressedSize { get; set; }
        public int? MaxReferenceListSize { get; set; }
        public int? MaxReferenceChainSize { get; set; }
        public bool TimeOptimalizationUsed { get; set; }
        public TimeSpan CompressTime { get; set; }
    }
}
