using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet
{
    public sealed class CustomLinearLoad1D : Load1D
    {
        public override Force[] GetEquivalentNodalLoads(Element element)
        {
            throw new NotImplementedException();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        protected CustomLinearLoad1D(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public override Force GetInternalForceAt(Element1D elm, double x)
        {
            throw new NotImplementedException();
        }
    }
}
