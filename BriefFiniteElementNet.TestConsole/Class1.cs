using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace BriefFiniteElementNet.TestConsole
{
    public static partial class DataContractSerializerHelper
    {
        public static string SerializeXml<T>(T obj, DataContractSerializer serializer = null, XmlWriterSettings settings = null)
        {
            serializer = serializer ?? new DataContractSerializer(obj.GetType());
            using (var textWriter = new StringWriter())
            {
                settings = settings ?? new XmlWriterSettings { Indent = true, IndentChars = "    " };
                using (var xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.WriteObject(xmlWriter, obj);
                }
                return textWriter.ToString();
            }
        }

        public static T DeserializeXml<T>(string xml, DataContractSerializer serializer = null)
        {
            using (var textReader = new StringReader(xml ?? ""))
            using (var xmlReader = XmlReader.Create(textReader))
            {
                return (T)(serializer ?? new DataContractSerializer(typeof(T))).ReadObject(xmlReader);
            }
        }
    }

    public static partial class DataContractJsonSerializerHelper
    {
        private static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.Unicode.GetBytes(value ?? ""));
        }

        public static string SerializeJson<T>(T obj, DataContractJsonSerializer serializer = null)
        {
            serializer = serializer ?? new DataContractJsonSerializer(obj.GetType());
            using (var memory = new MemoryStream())
            {
                serializer.WriteObject(memory, obj);
                memory.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(memory))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static T DeserializeJson<T>(string json, DataContractJsonSerializer serializer = null)
        {
            serializer = serializer ?? new DataContractJsonSerializer(typeof(T));
            using (var stream = GenerateStreamFromString(json))
            {
                var obj = serializer.ReadObject(stream);
                return (T)obj;
            }
        }
    }
}
