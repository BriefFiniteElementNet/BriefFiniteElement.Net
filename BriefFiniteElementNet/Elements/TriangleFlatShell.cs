using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a flat shell element who is combination of a triangle membrane element and a triangle plate bending element.
    /// </summary>
    [Serializable]
    public class TriangleFlatShell : Element3D
    {
        private double _thickness;

        private MembraneFormulation _membraneFormulation = MembraneFormulation.PlaneStress;

        /// <summary>
        /// Gets or sets the membrane formulation.
        /// </summary>
        /// <value>
        /// The membrane formulation of this element.
        /// </value>
        public MembraneFormulation MembraneFormulation
        {
            get { return _membraneFormulation; }
            set { _membraneFormulation = value; }
        }

        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness of this member, in [m] diemnsion.
        /// </value>
        public double Thickness
        {
            get { return _thickness; }
            set { _thickness = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriangleFlatShell"/> class.
        /// </summary>
        public TriangleFlatShell() : base(3)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriangleFlatShell"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected TriangleFlatShell(SerializationInfo info, StreamingContext context) : base(info, context)
        {


            _membraneFormulation = (MembraneFormulation)info.GetInt32("_membraneFormulation");
            _thickness = info.GetDouble("_thickness");
            throw new NotImplementedException();
        }

       

        /// <inheritdoc />
        public override Matrix GetGlobalStifnessMatrix()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the stiffness matrix of membrane element in a 18x18 matrix.
        /// </summary>
        /// <returns></returns>
        private Matrix GetMembraneStiffnessMatrix()
        {
            var ex = 210e9; //ortho tropic fomulation, in local coord system
            var ey = 210e9; //ortho tropic fomulation, in local coord system

            var nox = 0.2;//ortho tropic fomulation, in local coord system (poison ratio)
            var noy = 0.2;//ortho tropic fomulation, in local coord system (poison ratio)

            var t = this._thickness;

            var x1 = 0.0;
            var x2 = 0.0;
            var x3 = 0.0;

            var y1 = 0.0;
            var y2 = 0.0;
            var y3 = 0.0;


            Matrix d ;//= new Matrix(3, 3); //the D matrix

            switch (_membraneFormulation)
            {
                case MembraneFormulation.PlaneStress:
                    d = new Matrix(
                        new[]
                        {
                            new[] {0.0, 0.0, 0.0}, //row 1
                            new[] {0.0, 0.0, 0.0}, //row 2
                            new[] {0.0, 0.0, 0.0} //row 3
                        }); //the B matrix

                    break;
                case MembraneFormulation.PlaneStrain:
                    d = new Matrix(
                       new[]
                       {
                            new[] {0.0, 0.0, 0.0}, //row 1
                            new[] {0.0, 0.0, 0.0}, //row 2
                            new[] {0.0, 0.0, 0.0} //row 3
                       }); //the B matrix

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var a = 0.5*Math.Abs(x1*y2 + x2*y3 + x3*y1 - y1*x2 - y2*x3 - y3*x1); //the ara

            var b = 1/(2*a)*new Matrix(
                new[]
                {
                    new[] {y2 - y3, 0, y3 - y1, 0, y1 - y2, 0}, //row 1
                    new[] {0, x3 - x2, 0, x1 - x3, 0, x2 - x1},
                    new[] {x3 - x2, y2 - y3, x1 - x3, y3 - y1, x2 - x1, y1 - y2}
                }); //the B matrix


            var k = a*t*(b.Transpose()*d*b);

            return k;
        }

        /// <inheritdoc />
        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_membraneFormulation", (int)_membraneFormulation);
            info.AddValue("_thickness", _thickness);
            base.GetObjectData(info, context);
        }
    }
}