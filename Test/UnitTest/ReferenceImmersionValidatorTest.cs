using GraphCompression.Core.Algorithms;
using System.Collections.Generic;
using Test.UnitTest.ParametrizedTestClasses;
using Xunit;

namespace Test.UnitTest
{
    public class ReferenceImmersionValidatorTest
    {
        [Theory]
        [ClassData(typeof(ReferenceImmersionTestData))]
        public void ValidateReferenceImmersion_Test(Dictionary<int, HashSet<int>> referenceChain, int referencingItem, 
            int? maxReferenceChainSize, bool expectedResult)
        {
            var referenceImmersionValidator = new ReferenceImmersionValidator();

            var result = referenceImmersionValidator.ValidateReferenceImmersion(referenceChain, referencingItem, maxReferenceChainSize);

            Assert.Equal(expectedResult, result);
        }
    }
}
