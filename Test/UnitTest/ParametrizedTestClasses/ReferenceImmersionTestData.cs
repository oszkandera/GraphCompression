using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.UnitTest.Models;

namespace Test.UnitTest.ParametrizedTestClasses
{
    public class ReferenceImmersionTestData : IEnumerable<object[]>
    {
        private readonly List<ReferenceImmersionTestParameters> _data;

        public ReferenceImmersionTestData()
        {
            _data = new List<ReferenceImmersionTestParameters>();

            PrepareDataForTest();
        }

        #region IEnumerable

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach(var instance in _data)
            {
                yield return instance.ToObjectArray();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion 

        private void PrepareDataForTest()
        {
            //ReferenceChain is null
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = null,
                ReferencingItem = 1,
                MaxReferenceChainSize = 10,
                ExpectedResult = true
            });

            //ReferenceChain is empty
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = new Dictionary<int, HashSet<int>> { },
                ReferencingItem = 1,
                MaxReferenceChainSize = 10,
                ExpectedResult = true
            });

            //Value in ReferenceChain dictionary is null
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = new Dictionary<int, HashSet<int>>
                {
                    { 1, null }
                },
                ReferencingItem = 1,
                MaxReferenceChainSize = 10,
                ExpectedResult = true,
            });

            //Value in ReferenceChain dictionary is empty
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = new Dictionary<int, HashSet<int>>
                {
                    { 1, new HashSet<int> { } }
                },
                ReferencingItem = 1,
                MaxReferenceChainSize = 10,
                ExpectedResult = true,
            });

            //Item not found in ReferenceChaing
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = new Dictionary<int, HashSet<int>>
                {
                    { 1, new HashSet<int> { } }
                },
                ReferencingItem = 2,
                MaxReferenceChainSize = 10,
                ExpectedResult = true,
            });

            //MaxReferenceChain is not specified
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = new Dictionary<int, HashSet<int>>
                {
                    { 1, new HashSet<int> { } }
                },
                ReferencingItem = 2,
                MaxReferenceChainSize = null,
                ExpectedResult = true,
            });

            var referenceChainTree = new Dictionary<int, HashSet<int>>
            {
                { 1, new HashSet<int>{ 2, 3 } },
                { 2, new HashSet<int>{ 4, 7 } },
                { 4, new HashSet<int>{ 5 } },
                { 3, new HashSet<int>{ 6 } },
            };

            //MaxReferenceChain is oversteeped 
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = referenceChainTree,
                MaxReferenceChainSize = 3,
                ReferencingItem = 1,
                ExpectedResult = false
            });

            //ChainReference immersion is OK 
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = referenceChainTree,
                MaxReferenceChainSize = 3,
                ReferencingItem = 2,
                ExpectedResult = true
            });

            //MaxReferenceChain is oversteeped inside tree 
            _data.Add(new ReferenceImmersionTestParameters
            {
                ReferenceChain = referenceChainTree,
                MaxReferenceChainSize = 2,
                ReferencingItem = 2,
                ExpectedResult = false
            });
        }
    }
}
