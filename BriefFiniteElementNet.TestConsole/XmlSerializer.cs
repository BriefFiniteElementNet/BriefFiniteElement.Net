using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet.TestConsole
{
    public class XmlSerializer
    {
        public static void Serialize(ISerializable obj)
        {
            SerializationInfo info = new SerializationInfo(obj.GetType(), new FormatterConverter());
            StreamingContext context = new StreamingContext();

            obj.GetObjectData(info, context);


            while (true)
            {
                
            }
        }

        
    }
}
