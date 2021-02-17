using System.Collections;
using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class CompressedNode
    {
        public int Id { get; set; }
        public int? ReferenceId { get; set; }
        public BitArray ReferenceList { get; set; }
        public SortedSet<int> ExtraNodes { get; set; }
    }
}
