using GraphCompression.Core.Code;
using GraphCompression.Core.Interfaces.Algorithms;
using System;
using System.Collections.Generic;

namespace GraphCompression.Core.Algorithms
{
    public class ReferenceImmersionValidator : IReferenceImmersionValidator
    {
        public bool ValidateReferenceImmersion(Dictionary<int, HashSet<int>> referenceChain, int referencingItem, int? maxReferenceChainSize = null)
        {
            if (referenceChain == null) return true;
            if (!maxReferenceChainSize.HasValue) return true;

            var referenceChainDepth = Depth(referenceChain, referencingItem, 0);

            return referenceChainDepth < maxReferenceChainSize.Value;
        }

        private int Depth(Dictionary<int, HashSet<int>> referenceChain, int referencingItem, int depth)
        {
            if (ConditionalExpressions.ContainsReferenceChainDictionaryReferencingItemWithValue(referenceChain, referencingItem))
            {
                return 0;
            }

            int result = depth + 1;

            foreach (var referencingNode in referenceChain[referencingItem])
            {
                result = Math.Max(result, Depth(referenceChain, referencingNode, depth + 1));
            }

            return result;
        }
    }
}
