using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace GraphCompression.Core.Helpers
{
    public static class SizeDetector
    {
        public static long GetSizeOfObject(object value)
        {
            long size = 0;
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, value);
                size = s.Length;
            }

            return size;
        }
    }
}
