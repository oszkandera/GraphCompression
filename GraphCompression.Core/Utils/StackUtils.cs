using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Utils
{
    public static class StackUtils
    {
        public static void Shuffle<T>(this Stack<T> stack)
        {
            var values = stack.ToArray();
            stack.Clear();

            var random = new Random();

            var orderedValues = values.OrderBy(x => random.Next());

            foreach (var value in orderedValues)
            {
                stack.Push(value);
            }
        }
    }
}
