﻿namespace GraphCompression.Core.Models
{
    public class CompressParameters
    {
        public int? MaxReferenceListSize { get; set; }
        public int? MaxReferenceChainSize { get; set; }
        public bool WithTimeOptimalization { get; set; } = false;
    }
}
