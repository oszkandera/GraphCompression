using System.Collections.Generic;

namespace Test.UnitTest.Models
{
    public class ReferenceImmersionTestParameters
    {
        public Dictionary<int, HashSet<int>> ReferenceChain { get; set; }
        public int ReferencingItem { get; set; }
        public int? MaxReferenceChainSize { get; set; }
        public bool ExpectedResult { get; set; }

        public object[] ToObjectArray()
        {
            return new object[] { ReferenceChain, ReferencingItem, MaxReferenceChainSize, ExpectedResult };
        }
    }
}
