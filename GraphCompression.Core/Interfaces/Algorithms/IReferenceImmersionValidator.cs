using System.Collections.Generic;

namespace GraphCompression.Core.Interfaces.Algorithms
{
    public interface IReferenceImmersionValidator
    {
        bool ValidateReferenceImmersion(Dictionary<int, HashSet<int>> referenceChain, int referencingItem, int? maxReferenceChainSize = null);
    }
}
