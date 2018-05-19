using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedXmlSerialization;

namespace BriefFiniteElementNet.XmlSerialization
{
    public class ModelSerializationConfig : ExtendedXmlSerializerConfig<Force>
    {
        public Node Deserialize(XElement element)
        {
            var force = new Force();

            
            throw new NotImplementedException();
            //return new TestClass(element.Element("String").Value);
        }

        public void Serializer(XmlWriter writer, Force obj)
        {
            writer.WriteElementString("Fx", obj.Fx.ToString());
            writer.WriteElementString("Fy", obj.Fy.ToString());
            writer.WriteElementString("Fz", obj.Fz.ToString());

            writer.WriteElementString("Mx", obj.Mx.ToString());
            writer.WriteElementString("My", obj.My.ToString());
            writer.WriteElementString("Mz", obj.Mz.ToString());
        }
    }

    public class FrameElementSerializationConfig : ExtendedXmlSerializerConfig<FrameElement2Node>
    {

        public FrameElementSerializationConfig()
        {
            CustomSerializer(Serializer, Deserialize);
        }

        public FrameElement2Node Deserialize(XElement element)
        {
            var a = element.Attribute("A").Value;

            return new FrameElement2Node() { A = double.Parse(a) };

            //return new TestClass(element.Element("String").Value);
        }

        public void Serializer(XmlWriter writer, FrameElement2Node obj)
        {
            writer.WriteAttributeString("A", obj.A.ToString());

            writer.WriteStartElement("StartNode");

            writer.WriteAttributeString("ref", obj.StartNode.Index.ToString());

            writer.WriteEndElement();



        }
    }
}
