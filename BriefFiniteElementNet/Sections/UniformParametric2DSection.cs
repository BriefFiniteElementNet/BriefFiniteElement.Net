using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;



namespace BriefFiniteElementNet.Sections
{
    [Serializable]
    public class UniformParametric2DSection : Base2DSection
    {
        private double _t;

        public double T
        {
            get { return _t; }
            set { _t = value; }
        }


        public override double GetThicknessAt(params double[] isoCoords)
        {
            return T;
        }

        public override int[] GetMaxFunctionOrder()
        {
            return new[] {0, 0, 0};
        }


        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_t", _t);
            base.GetObjectData(info, context);
        }

        protected UniformParametric2DSection(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _t = info.GetDouble("_t");
        }

        public UniformParametric2DSection() : base()
        {

        }
    }
}
