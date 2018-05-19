using ExtendedXmlSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BriefFiniteElementNet.XmlSerialization
{
    public class NodeConfig : ExtendedXmlSerializerConfig<Node>
    {
        public NodeConfig()
        {
            ObjectReference(i => i.Index);
        }
    }
}
