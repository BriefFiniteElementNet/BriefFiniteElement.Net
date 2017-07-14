using ExtendedXmlSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.XmlSerialization
{
    public class Tester
    {
        public static void Test()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(2, 2, 2);

            model.ReIndexNodes();

            var toolsFactory = new SimpleSerializationToolsFactory();

            toolsFactory.Configurations.Add(new NodeConfig());
            toolsFactory.Configurations.Add(new FrameElementSerializationConfig());
            
            ExtendedXmlSerializer serializer = new ExtendedXmlSerializer(toolsFactory);
            var modelSt = serializer.Serialize(model);

            toolsFactory.Configurations.RemoveAt(1);


            var t = serializer.Deserialize<Model>(modelSt);


            t.Nodes.ToList().ForEach(o => o.Label = Guid.NewGuid().ToString());


        }
    }
}
