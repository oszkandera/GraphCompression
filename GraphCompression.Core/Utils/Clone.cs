using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GraphCompression.Core.Utils
{
    public static class Clone
    {
        public static T Create<T>(T objectToClone)
        {
            using (MemoryStream memory_stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memory_stream, objectToClone);

                memory_stream.Position = 0;
                return (T)formatter.Deserialize(memory_stream);
            }
        }
    }
}
