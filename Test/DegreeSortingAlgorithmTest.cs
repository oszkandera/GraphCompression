using GraphCompression.Core.Code;
using System.Collections.Generic;
using Xunit;

namespace Test
{
    public class DegreeSortingAlgorithmTest
    {

        [Theory]
        [MemberData(nameof(Data))]
        public void GetSortedGraphStructure_Test(Dictionary<int, List<int>> originalGraphStructure, List<KeyValuePair<int, List<int>>> expectedGraphStructure)
        {
            var sortingAlgorithm = new DegreeSortingAlgorithm();

            var result = sortingAlgorithm.GetSortedGraphStructure(originalGraphStructure);

            CheckValidity(result, expectedGraphStructure);
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                var dataList = new List<object[]>();

                var (withNullStructure, withNullExpected) = CreateOriginalGraphStructure("NULL");
                var (withNullNeighborList, withNullNeighborListExpected) = CreateOriginalGraphStructure("WITH_NULL_NEIGHBOR_LIST");
                var (validUnsorted, validUnsortedExpected) = CreateOriginalGraphStructure("VALID_UNSORTED");

                dataList.Add(new object[] { withNullStructure, withNullExpected });
                dataList.Add(new object[] { withNullNeighborList, withNullNeighborListExpected });
                dataList.Add(new object[] { validUnsorted, validUnsortedExpected });
                dataList.Add(new object[] { withNullStructure, withNullExpected });
                dataList.Add(new object[] { withNullStructure, withNullExpected });
                dataList.Add(new object[] { withNullStructure, withNullExpected });
                dataList.Add(new object[] { withNullStructure, withNullExpected });

                return dataList;
            }
        }

        private void CheckValidity(List<KeyValuePair<int, List<int>>> originalOutput, List<KeyValuePair<int, List<int>>> expectedValue)
        {
            Assert.NotNull(originalOutput);
            Assert.Equal(originalOutput.Count, expectedValue.Count);
            
            int nodeIndex = 0;
            Assert.All(originalOutput, node =>
            {
                Assert.Equal(node.Key, expectedValue[nodeIndex].Key);
                Assert.Equal(node.Value.Count, expectedValue[nodeIndex].Value.Count);

                var neighborIndex = 0;
                Assert.All(node.Value, neighbor =>
                {
                    Assert.Equal(neighbor, expectedValue[nodeIndex].Value[neighborIndex]);
                    neighborIndex++;
                });

                nodeIndex++;
            });
        }

        private static (Dictionary<int, List<int>>, List<KeyValuePair<int, List<int>>>) CreateOriginalGraphStructure(string type)
        {
            Dictionary<int, List<int>> originalGraphStructure = null;
            List<KeyValuePair<int, List<int>>> expectedOutput = null;

            switch (type)
            {
                case "NULL":
                    originalGraphStructure = null;
                    expectedOutput = new List<KeyValuePair<int, List<int>>>();
                    break;
                case "WITH_NULL_NEIGHBOR_LIST":
                    originalGraphStructure = new Dictionary<int, List<int>>
                    {
                        { 1, null }
                    };

                    expectedOutput = new List<KeyValuePair<int, List<int>>>();
                    expectedOutput.Add(KeyValuePair.Create(1, new List<int>()));

                    break;
                case "VALID_UNSORTED":
                    originalGraphStructure = new Dictionary<int, List<int>>
                    {
                        { 3, new List<int> {  } },
                        { 2, new List<int> { 3, 5, 2 } },
                        { 1, new List<int> { 2 } },
                        { 5, new List<int> { 7, 6 } },
                        { 7, new List<int> {  } },
                    };

                    expectedOutput = new List<KeyValuePair<int, List<int>>>();
                    expectedOutput.Add(KeyValuePair.Create(3, new List<int>()));
                    expectedOutput.Add(KeyValuePair.Create(7, new List<int>()));
                    expectedOutput.Add(KeyValuePair.Create(1, new List<int> { 2 }));
                    expectedOutput.Add(KeyValuePair.Create(5, new List<int> { 6, 7 }));
                    expectedOutput.Add(KeyValuePair.Create(2, new List<int> { 2, 3, 5 }));

                    break;
            }

            return (originalGraphStructure, expectedOutput);
        }
    }
}
