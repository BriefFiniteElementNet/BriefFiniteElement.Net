using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class SerializationTest
    {
        public static void Test1()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(2, 2, 2);

            //BriefFiniteElementNet.XamlSerialization.XamlSerializer.Serialize(model);
        }
    }
}
