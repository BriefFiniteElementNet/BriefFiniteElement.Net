using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BriefFiniteElementNet.SerializationObsolete
{
    public class ObjectWriter
    {
        public void WriteObject(Stream stream, ObjectWrapper wrapped)
        {
            var rwtr = new XmlTextWriter(stream, Encoding.Unicode);



            throw new NotImplementedException();
        }

        private string LastOpenedElement;


        private void WriteProperty(XmlTextWriter rwtr, PropertyWrapper prp)
        {
            if (prp.PropertyValue.IsPremitive)
            {
                rwtr.WriteAttributeString(prp.PropertyName, prp.PropertyValue.PrimitiveValue.ToString());
            }

            if (prp.PropertyValue.IsNull)
            {
                rwtr.WriteAttributeString(prp.PropertyName, "x", "null");
            }

            if (prp.PropertyValue.IsArray)
            {
                rwtr.WriteAttributeString(prp.PropertyName, "x", "null");
            }

            if (prp.PropertyValue.IsArray)
            {
                
            }


            throw new NotImplementedException();
        }
    }
}
