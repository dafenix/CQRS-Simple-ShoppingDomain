using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace CQRS.Infrastructure
{
    public static class Serialization
    {
        public static string Serialize(object o)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings());
                serializer.Serialize(new JsonTextWriter(writer), o);
            }

            return stringBuilder.ToString();
        }

        public static byte[] SerializeSnaptshot(Snapshot snapy)
        {
            using (var dataStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(dataStream, snapy);
                return dataStream.ToArray();
            }
        }

        public static object Deserialize(Type type, string data)
        {
            using (var reader = new StringReader(data))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings());
                return serializer.Deserialize(new JsonTextReader(reader), type);
            }
        }

        public static Snapshot DeserializeSnapshot(byte[] snapshotData)
        {
            using (var buffer = new MemoryStream(snapshotData))
            {
                var formatter = new BinaryFormatter();
                return (Snapshot) formatter.Deserialize(buffer);
            }
        }
    }
}