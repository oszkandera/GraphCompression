using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class CompressedNode
    {
        public int Id { get; set; }
        public int? ReferenceId { get; set; }
        public bool[] ReferenceList { get; set; }
        public SortedSet<int> ExtraNodes { get; set; }
    }
}
