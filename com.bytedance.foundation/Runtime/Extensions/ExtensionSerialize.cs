using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Google.Protobuf;
using Newtonsoft.Json;
using UnityEngine;

namespace ByteDance.Foundation
{
    public static class ExtensionSerialize
    {
        public static string ToJson(this object data, bool prettyPrint = true)
        {
            return JsonUtility.ToJson(data, prettyPrint);
        }

        public static T FromJson<T>(this string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public static void FromJsonOverride<T>(this string json, T objectToOverwrite)
        {
            JsonUtility.FromJsonOverwrite(json, objectToOverwrite);
        }

        public static string ToNewtonJson(this object data, bool prettyFormat = false)
        {
            return JsonConvert.SerializeObject(data, prettyFormat ? Formatting.Indented : Formatting.None);
        }

        public static T FromNewtonJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static byte[] ToBinary(this object data)
        {
            byte[] bytes;
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memStream, data);

                memStream.Seek(0, SeekOrigin.Begin);
                bytes = new byte[memStream.Length];
                int count = memStream.Read(bytes, 0, 20);

                while (count < memStream.Length)
                {
                    bytes[count++] = Convert.ToByte(memStream.ReadByte());
                }
            }
            return bytes;
        }

        public static T FromBinary<T>(this byte[] data)
        {
            T result;
            using (MemoryStream fs = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                result = (T)formatter.Deserialize(fs);
            }
            return result;
        }

        public static byte[] GoogleProtobufToBytes<T>(this T d) where T : IMessage
        {
            return d.ToByteArray();
        }
    }
}