using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MahiruLauncher.Utils
{
    public class Utf8StringWriter : StringWriter
    {
        // Use UTF8 encoding but write no BOM to the wire
        public override Encoding Encoding => new UTF8Encoding(true);
    }
    
    public static class Serializer
    {
        public static string Serialize<T>(this T obj)
        {
            var xsSubmit = new XmlSerializer(typeof(T));
            using var sww = new Utf8StringWriter();
            using var writer = new XmlTextWriter(sww) { Formatting = Formatting.Indented };
            xsSubmit.Serialize(writer, obj);
            return sww.ToString();
        }
        
        public static T Load<T>(string fileName) where T : class
        {
            using var stream = File.OpenRead(fileName);
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(stream) as T;
        }
    }
}