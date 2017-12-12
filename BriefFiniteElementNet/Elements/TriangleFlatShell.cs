using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Geometry;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a triangle flat shell which internally consists of a membrane (CST) and plate bending (dkt) element.
    /// </summary>
    [Serializable]
    public class TriangleFlatShell : Element2D
    {
        #region field and properties

        private double _thickness;

        private double _poissonRatio;

        private double _elasticModulus;

        private FlatShellBehaviour _behaviour = FlatShellBehaviours.FullThinShell;

        private bool _addDrillingDof = true;

        private MembraneFormulation _formulationType = MembraneFormulation.PlaneStress;


        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness of this member, in [m] dimension.
        /// </value>
        public double Thickness
        {
            get { return _thickness; }
            set { _thickness = value; }
        }

        /// <summary>
        /// Gets or sets the Poisson ratio.
        /// </summary>
        /// <value>
        /// The Poisson ratio.
        /// </value>
        public double PoissonRatio
        {
            get { return _poissonRatio; }
            set { _poissonRatio = value; }
        }

        /// <summary>
        /// Gets or sets the elastic modulus.
        /// </summary>
        /// <value>
        /// The elastic modulus in [Pa] dimension.
        /// </value>
        public double ElasticModulus
        {
            get { return _elasticModulus; }
            set { _elasticModulus = value; }
        }

        /// <summary>
        /// Gets or sets the behavior.
        /// </summary>
        /// <value>
        /// The behavior of shell element.
        /// </value>
        public FlatShellBehaviour Behavior
        {
            get { return _behaviour; }
            set { _behaviour = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether should add drilling DoF to the element.
        /// Recommended to be [true]
        /// </summary>
        /// <value>
        ///   <c>true</c> if [add drilling dof]; otherwise, <c>false</c>.
        /// </value>
        public bool AddDrillingDof
        {
            get { return _addDrillingDof; }
            set { _addDrillingDof = value; }
        }

        /// <summary>
        /// Gets or sets the type of the formulation.
        /// </summary>
        /// <value>
        /// The type of the formulation.
        /// </value>
        public MembraneFormulation MembraneFormulationType
        {
            get { return _formulationType; }
            set { _formulationType = value; }
        }

        #endregion

        /// <inheritdoc/>
        public override Point IsoCoordsToGlobalLocation(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DktElement"/> class.
        /// </summary>
        public TriangleFlatShell() : base(3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DktElement"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected TriangleFlatShell(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._thickness = info.GetDouble("_thickness");
            this._poissonRatio = info.GetDouble("_poissonRatio");
            this._elasticModulus = info.GetDouble("_elasticModulus");
            this._addDrillingDof = info.GetBoolean("_addDrillingDof");
            this._behaviour = (FlatShellBehaviour)info.GetInt32("_behaviour");
            this._formulationType = (MembraneFormulation)info.GetInt32("_formulationType");
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_thickness", this._thickness);
            info.AddValue("_poissonRatio", this._poissonRatio);
            info.AddValue("_elasticModulus", this._elasticModulus);
            info.AddValue("_addDrillingDof", this._addDrillingDof);
            info.AddValue("_formulationType", (int)this._formulationType);
            info.AddValue("_behaviour", (int)this._behaviour);

            base.GetObjectData(info, context);
        }

        #region stiffness

        public override Matrix GetGlobalStifnessMatrix()
        {
            var kl = new Matrix(18,18);

            if ((this._behaviour & FlatShellBehaviour.ThinPlate) != 0)
                kl += GetLocalPlateBendingStiffnessMatrix();

            if ((this._behaviour & FlatShellBehaviour.Membrane) != 0)
                kl += GetLocalMembraneStiffnessMatrix();

            /*
            if (_behaviour == FlatShellBehaviour.Membrane || _behaviour == FlatShellBehaviour.ThinShell)
                kl += GetLocalMembraneStiffnessMatrix();

            if (_behaviour == FlatShellBehaviour.ThinPlate || _behaviour == FlatShellBehaviour.ThinShell)
                kl += GetLocalPlateBendingStiffnessMatrix();
            */
            if ((this._behaviour & FlatShellBehaviour.DrillingDof) != 0)
            {
                var dd = new int[] {3, 4, 9, 10, 15, 16};

                var max = dd.Max(i => kl[i, i])/1e3;

                kl[5, 5] = kl[11, 11] = kl[17, 17] = max;
            }

            var lambda = GetTransformationMatrix();

            var t = Matrix.DiagonallyRepeat(lambda.Transpose(), 6); // eq. 5-16 page 78 (87 of file)

            var buf = t.Transpose() * kl * t; //eq. 5-15 p77

            return buf;
        }

        private Matrix GetLocalMembraneStiffnessMatrix()
        {
            //cst

            //step 1 : get points in local system
            //step 2 : get local stiffness matrix
            //step 3 : expand local stiffness matrix
            //step 4 : get global stiffness matrix

            //step 1
            var ls = GetLocalPoints();

            var xs = new[] { ls[0].X, ls[1].X, ls[2].X };
            var ys = new[] { ls[0].Y, ls[1].Y, ls[2].Y };

            //step 2
            var kl = CstElement.GetStiffnessMatrix(xs, ys, this._thickness, this.ElasticModulus, this.PoissonRatio,
                this._formulationType);

            //step 3
            var currentOrder = new[]
            {
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dy),

                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dy),

                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Dy),
            };

            var kle = FluentElementPermuteManager.FullyExpand(kl, currentOrder, 3);

            return kle;
        }

        public Matrix GetLocalPlateBendingStiffnessMatrix()
        {
            //dkt

            //step 1 : get points in local system
            //step 2 : get local stiffness matrix
            //step 3 : expand local stiffness matrix
            //step 4 : get global stiffness matrix

            //step 1
            var ls = GetLocalPoints();

            var xs = new double[] { ls[0].X, ls[1].X, ls[2].X };
            var ys = new double[] { ls[0].Y, ls[1].Y, ls[2].Y };

            //step 2
            var kl = DktElement.GetStiffnessMatrix(xs, ys, this._thickness, this.ElasticModulus, this.PoissonRatio);

            //step 3
            var currentOrder = new FluentElementPermuteManager.ElementLocalDof[]
            {
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dz),
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Rx),
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Ry),

                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dz),
                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Rx),
                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Ry),

                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Dz),
                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Rx),
                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Ry),
            };

            var kle = FluentElementPermuteManager.FullyExpand(kl, currentOrder, 3);


            return kle;
        }

        #endregion

        public Matrix GetTransformationMatrix()
        {
            return DktElement.GetTransformationMatrix(nodes[0].Location, nodes[1].Location, nodes[2].Location);
        }

        #region internal forces

        /// <summary>
        /// Gets the internal force at defined location.
        /// tensor is in local coordinate system. 
        /// </summary>
        /// <param name="localX">The X in local coordinate system (see remarks).</param>
        /// <param name="localY">The Y in local coordinate system (see remarks).</param>
        /// <param name="combination">The load combination.</param>
        /// <returns>Stress tensor of flat shell, in local coordination system</returns>
        /// <remarks>
        /// for more info about local coordinate of flat shell see page [72 of 166] (page 81 of pdf) of "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara freely available on the web
        /// </remarks>
        public FlatShellStressTensor GetInternalForce(double localX, double localY, LoadCombination combination)
        {
            var buf = new FlatShellStressTensor();

            if ((this._behaviour & FlatShellBehaviour.ThinPlate) != 0)
                buf.MembraneTensor = GetMembraneInternalForce(combination);

            if ((this._behaviour & FlatShellBehaviour.Membrane) != 0)
                buf.BendingTensor = GetBendingInternalForce(localX, localY, combination);

            return buf;
        }

        private MembraneStressTensor GetMembraneInternalForce( LoadCombination combination)
        {
            //Note: membrane internal force is constant

            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U
            //Note : Steps changed...

            var trans = this.GetTransformationMatrix();

            var lp = GetLocalPoints();

            var g2l = new Func<Vector, Vector>(glob => (trans.Transpose() * glob.ToMatrix()).ToVector());
            //var l2g = new Func<Vector, Vector>(local => (trans*local.ToMatrix()).ToPoint());


            var d1g = this.nodes[0].GetNodalDisplacement(combination);
            var d2g = this.nodes[1].GetNodalDisplacement(combination);
            var d3g = this.nodes[2].GetNodalDisplacement(combination);

            //step 3
            var d1l = new Displacement(g2l(d1g.Displacements), g2l(d1g.Rotations));
            var d2l = new Displacement(g2l(d2g.Displacements), g2l(d2g.Rotations));
            var d3l = new Displacement(g2l(d3g.Displacements), g2l(d3g.Rotations));

            var uCst =
                   new Matrix(new[]
                   {d1l.DX, d1l.DY, d2l.DX, d2l.DY, /**/d3l.DX, d3l.DY});

            var dbCst = CstElement.GetDMatrix(_elasticModulus,_poissonRatio,_formulationType);

            var bCst = CstElement.GetBMatrix(lp.Select(i => i.X).ToArray(),
                lp.Select(i => i.Y).ToArray());

            var sCst = dbCst* bCst * uCst; 

            var buf = new MembraneStressTensor();

            buf.Sx = sCst[0, 0];
            buf.Sy = sCst[1, 0];
            buf.Txy = sCst[2, 0];

            return buf;
        }

        private PlateBendingStressTensor GetBendingInternalForce(double localX, double localY, LoadCombination cmb)
        {
            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U
            //Note : Steps changed...

            var trans = this.GetTransformationMatrix();

            var lp = GetLocalPoints();

            var g2l = new Func<Vector, Vector>(glob => (trans.Transpose()*glob.ToMatrix()).ToVector());
            //var l2g = new Func<Vector, Vector>(local => (trans*local.ToMatrix()).ToPoint());


            var d1g = this.nodes[0].GetNodalDisplacement(cmb);
            var d2g = this.nodes[1].GetNodalDisplacement(cmb);
            var d3g = this.nodes[2].GetNodalDisplacement(cmb);

            //step 3
            var d1l = new Displacement(g2l(d1g.Displacements), g2l(d1g.Rotations));
            var d2l = new Displacement(g2l(d2g.Displacements), g2l(d2g.Rotations));
            var d3l = new Displacement(g2l(d3g.Displacements), g2l(d3g.Rotations));

            var uDkt =
                   new Matrix(new[]
                   {d1l.DZ, d1l.RX, d1l.RY, /**/ d2l.DZ, d2l.RX, d2l.RY, /**/ d3l.DZ, d3l.RX, d3l.RY});


            var dbDkt = DktElement.GetDMatrix(this._thickness, this._elasticModulus, this._poissonRatio);



            var b = DktElement.GetBMatrix(localX, localY,
                lp.Select(i => i.X).ToArray(),
                lp.Select(i => i.Y).ToArray());

            var mDkt = dbDkt * b * uDkt; //eq. 32, batoz article

            var buf = new PlateBendingStressTensor();

            buf.Mx = mDkt[0, 0];
            buf.My = mDkt[1, 0];
            buf.Mxy = mDkt[2, 0];

            return buf;
        }

        #endregion

        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Force[] GetEquivalentNodalLoads(Load load)
        {
            if (load is UniformLoadForPlanarElements)
            {
                //lumped approach is used as used in several references
                var ul = load as UniformLoadForPlanarElements;

                var u = new Vector();

                u.X = ul.Ux;
                u.Y = ul.Uy;
                u.Z = ul.Uz;

                if (ul.CoordinationSystem == CoordinationSystem.Local)
                {
                    var trans = GetTransformationMatrix();
                    u = (trans*u.ToMatrix()).ToVector(); //local to global
                }

                if (_behaviour == FlatShellBehaviour.Membrane)
                    u.Z = 0;//remove one for plate bending

                if (_behaviour == FlatShellBehaviour.ThinPlate)
                    u.Y = u.X = 0;//remove those for membrane


                var area = CalcUtil.GetTriangleArea(nodes[0].Location, nodes[1].Location, nodes[2].Location);

                var f = u*(area/3);
                var frc = new Force(f, Vector.Zero);
                return new[] {frc, frc, frc};
            }


            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets node coordinates in local coordination system.
        /// Z component of return values should be ignored.
        /// </summary>
        public Point[] GetLocalPoints()
        {
            var t = GetTransformationMatrix().Transpose(); //transpose of t

            var g0 = nodes[0].Location.ToMatrix();
            var g1 = nodes[1].Location.ToMatrix();
            var g2 = nodes[2].Location.ToMatrix();


            var p0 = (t * g0).ToPoint();
            var p1 = (t * g1).ToPoint();
            var p2 = (t * g2).ToPoint();

            return new[] { p0, p1, p2 };
        }

        

        ///<inheritdoc/>
        public override Matrix ComputeBMatrix(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeDMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeNMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeJMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }
    }
}
