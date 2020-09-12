using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using HtmlTags;
using System.Globalization;

namespace BriefFiniteElementNet.Validation
{
    public class BarElementTester:IValidator
    {
        public static void TestEndReleaseStyiffness()
        {
            //A NUMERICAL PROCEDURE FOR THE NONLINEAR ANALYSIS OF REINFORCED CONCRETE FRAMES WITH INFILL WALLS
            //by MURAT EFE GÜNEY
            //p. 51: 2.3 Condensation and Constraint Equations

            var elm = new BarElement(2);

            elm.Nodes[0] = new Node(-1, 0, 0);
            elm.Nodes[1] = new Node(0, 0, 0);
            //elm.Nodes[2] = new Node(1, 0, 0);


            //elm.StartReleaseCondition = Constraints.FixedRX;
            elm.Section = new UniformParametric1DSection(1, 2, 3, 4);
            elm.Material = new UniformIsotropicMaterial(1, 0.3);

            

            var newConds = new Constraint[elm.NodeCount];

            {

                for (int i = 0; i < newConds.Length; i++)
                {
                    newConds[i] = Constraints.Fixed;
                }

                newConds[0].DZ = DofConstraint.Released;
            }

            

            Array.Copy(newConds, elm._nodalReleaseConditions, newConds.Length);
            
            var kr2 = elm.GetLocalStifnessMatrix();

            for (var i = 0; i < elm.NodeCount; i++)
                elm.NodalReleaseConditions[i] = Constraint.Fixed;

            var kfull = elm.GetLocalStifnessMatrix();
            var kr = GetCondensedStiffness(kfull, newConds);


            var ratio = kr.PointwiseDivide(kr2);
            ratio.Replace(double.NaN, 0);

            var d = (kr - kr2);

            var dmax = d.Values.Max(i => Math.Abs(i));
        }

        private static Matrix GetCondensedStiffness(Matrix k, params Constraint[] consts)
        {
            var n = consts.Length;
            var fixes = 0;

            for (int i = 0; i < n; i++)
            {
                var cns = consts[i];

                if (cns.DX == DofConstraint.Fixed) fixes++;

                if (cns.DY == DofConstraint.Fixed) fixes++;

                if (cns.DZ == DofConstraint.Fixed) fixes++;

                if (cns.RX == DofConstraint.Fixed) fixes++;

                if (cns.RY == DofConstraint.Fixed) fixes++;

                if (cns.RZ == DofConstraint.Fixed) fixes++;
            }

            if (fixes == 6 * n)
                return k.Clone().AsMatrix();


            var pf = new Matrix(fixes, 6 * n);
            var pc = new Matrix(6 * n-fixes, 6 * n);

            var c1 = 0;
            var c2 = 0;

            for (var i = 0; i < n; i++)
            {
                var cns = consts[i];

                if (cns.DX == DofConstraint.Fixed)
                    pf[c1++, 6*i + 0] = 1;
                else
                    pc[c2++, 6*i + 0] = 1;

                if (cns.DY == DofConstraint.Fixed)
                    pf[c1++, 6*i + 1] = 1;
                else
                    pc[c2++, 6*i + 1] = 1;

                if (cns.DZ == DofConstraint.Fixed)
                    pf[c1++, 6*i + 2] = 1;
                else
                    pc[c2++, 6*i + 2] = 1;

                if (cns.RX == DofConstraint.Fixed)
                    pf[c1++, 6*i + 3] = 1;
                else
                    pc[c2++, 6*i + 3] = 1;


                if (cns.RY == DofConstraint.Fixed)
                    pf[c1++, 6*i + 4] = 1;
                else
                    pc[c2++, 6*i + 4] = 1;

                if (cns.RZ == DofConstraint.Fixed)
                    pf[c1++, 6*i + 5] = 1;
                else
                    pc[c2++, 6*i + 5] = 1;
            }

            var kff = pf * k * pf.Transpose();
            var kfc = pf * k * pc.Transpose();
            var kcf = pc * k * pf.Transpose();
            var kcc = pc * k * pc.Transpose();


            var kr = kff - kfc*kcc.Inverse()*kcf;


            var kt = pf.Transpose()*kr*pf;

            return kt;
            throw new NotImplementedException();
        }

        public ValidationResult[] DoAllValidation()
        {
            var buf = new List<ValidationResult>();

            buf.Add(TestFixedEndMoment_uniformLoad());
            //buf.Add(Test_Trapezoid_1());

            return buf.ToArray();
        }

        public static void TestFixedInternalForce1()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, w, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = w / 12 * (6 * length * x - 6 * x * x - length * length);
                var vi = w * (length / 2 - x);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) });

                var exactFrc = new Force(fx: 0, fy: 0, fz: vi, mx: 0, my: mi, mz: 0);

                var d = testFrc.FirstOrDefault(i => i.Item1 == DoF.Ry).Item2 + exactFrc.My;

                if (Math.Abs(d) > 1e-5)
                {

                }

            }




        }



        public static void TestFixedInternalForce2()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.J, w, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z,elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            hlpr.GetLoadInternalForceAt(elm, u1, new double[] { -0.5774 });

            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = w / 12 * (6 * length * x - 6 * x * x - length * length);
                var vi = w * (length / 2 - x);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) });

                var exactFrc = new Force(fx: 0, fy: vi, fz: 0, mx: 0, my: 0, mz: mi);

                var d = testFrc.FirstOrDefault(i => i.Item1 == DoF.Rz).Item2 + exactFrc.Mz;

                if (Math.Abs(d) > 1e-5)
                {

                }

            }




        }


        public static void TestEndreleaseInternalForce()
        {
            /**/
            var m1 = new Model();

            

            var I = (0.1 * 0.1 * 0.1 * 0.1) / 12;
            var A = (0.1 * 0.1 * 0.1);
            var E = 210e9;

            var sec = new Sections.UniformParametric1DSection(A, I, I, I);
            var mat = new Materials.UniformIsotropicMaterial(E, 0.3);
            var p = 1e3;

            /**/
            //var p0 = new Point(0, 0, 0);
            //var p1 = new Point(3, 4, 5);

            var l = 4.0;

            {//model 1 
                var el1 = new BarElement(3);

                el1.Nodes[0] = new Node(0, 0, 0) { Constraints = Constraints.Fixed & Constraints.FixedRX, Label = "n0" }; ;
                el1.Nodes[1] = new Node(l, 0, 0) { Label = "n1" };
                el1.Nodes[2] = new Node(2*l, 0, 0) { Constraints = Constraints.Released, Label = "n2" };

                el1.Section = sec;
                el1.Material = mat;

                m1.Nodes.Add(el1.Nodes);
                m1.Elements.Add(el1);
                m1.Nodes[1].Loads.Add(new NodalLoad(new Force(0, 0, p, 0, 0, 0)));

                var ep = 1e-10;

                m1.Solve_MPC();

                var frc = el1.GetInternalForceAt(1-ep);

                var fnc = new Func<double, double>(i => el1.GetInternalDisplacementAt(i).DZ);

                Controls.FunctionVisualizer.VisualizeInNewWindow(fnc, -1+ep, 1-ep);

                var s2 = m1.Nodes["n2"].GetSupportReaction();
                var s0 = m1.Nodes["n0"].GetSupportReaction();

                var k = (el1 as BarElement).GetLocalStifnessMatrix();
            }



            //m1.Solve_MPC();
           // m2.Solve_MPC();

           // var d1 = m1.Nodes.Last().GetNodalDisplacement();
           // var d2 = m2.Nodes.Last().GetNodalDisplacement();

        }

        public void TestConcentratedLoad()
        {
            
        }

        public static void Test3NodeBeam()
        {
            var m1 = new Model();
            var m2 = new Model();

            var l = 2.0;

            var I = 1;//(0.1 * 0.1 * 0.1 * 0.1) / 12;
            var A = 1;//(0.1 * 0.1 * 0.1);
            var E = 1;//210e9;

            var sec = new Sections.UniformParametric1DSection(A, I, I, I);
            var mat = new Materials.UniformIsotropicMaterial(E, 0.3);
            var p = 1e3;


            //var p0 = new Point(0, 0, 0);
            //var p1 = new Point(3, 4, 5);


            {//model 1 
                var el1 = new BarElement(3);

                //el1.Behavior= BarElementBehaviour.BeamZEulerBernoulli;
                el1.Nodes[0] = new Node(0, 0, 0) { Constraints = Constraints.Fixed }; ;
                el1.Nodes[1] = new Node(l/2, 0, 0) { Constraints = Constraints.Released};
                el1.Nodes[2] = new Node(l, 0, 0) { Constraints = Constraints.Released };

                el1.Section = sec;
                el1.Material = mat;

                m1.Nodes.Add(el1.Nodes);
                m1.Elements.Add(el1);
                m1.Nodes.Last().Loads.Add(new NodalLoad(new Force(0, 0, p, 0, 0, 0)));

                var k = (el1 as BarElement).GetLocalStifnessMatrix();
            }


            {//model 2 
                var el1 = new BarElement(3);

                //el1.Behavior= BarElementBehaviour.BeamZEulerBernoulli;
                el1.Nodes[0] = new Node(0, 0, 0) { Constraints = Constraints.Fixed }; ;
                el1.Nodes[1] = new Node(l / 2, 0, 0) { Constraints = Constraints.Released };
                el1.Nodes[2] = new Node(l, 0, 0) { Constraints = Constraints.Released };

                el1.Section = sec;
                el1.Material = mat;

                m1.Nodes.Add(el1.Nodes);
                m1.Elements.Add(el1);
                m1.Nodes.Last().Loads.Add(new NodalLoad(new Force(0, 0, p, 0, 0, 0)));

                var k = (el1 as BarElement).GetLocalStifnessMatrix();
            }


            m1.Solve_MPC();

            var d1 = m1.Nodes.First().GetSupportReaction();
            //var d2 = m2.Nodes.Last().GetNodalDisplacement();

        }

        public static void TestEndrelease()
        {
            var m1 = new Model();
            var m2 = new Model();

            var l = 4.0;

            var I = (0.1 * 0.1 * 0.1 * 0.1) / 12;
            var A = (0.1 * 0.1 * 0.1);
            var E = 210e9;

            var sec = new Sections.UniformParametric1DSection(A, I, I, I);
            var mat = new Materials.UniformIsotropicMaterial(E, 0.3);
            var p = 1e3;


            //var p0 = new Point(0, 0, 0);
            //var p1 = new Point(3, 4, 5);


            {//model 1 
                var el1 = new BarElement(2);
                var el2 = new BarElement(2);

                el1.Nodes[0] = new Node(0, 0, 0) { Constraints = Constraints.Fixed, Label = "n0" }; ;
                el2.Nodes[0] = el1.Nodes[1] = new Node(l/2, 0, 0) { Label = "n1" };
                el2.Nodes[1] = new Node(l, 0, 0) {  Label = "n2" };

                el1.Section = el2.Section = sec;
                el1.Material = el2.Material = mat;

                m1.Nodes.Add(el1.Nodes);
                m1.Nodes.Add(el2.Nodes[1]);

                m1.Elements.Add(el1,el2);

                m1.Nodes["n1"].Loads.Add(new NodalLoad(new Force(0, 0, p, 0, 0, 0)));

                var k = (el1 as BarElement).GetLocalStifnessMatrix();
            }

            {//model 2 
                var el2 = new BarElement(3);

                el2.Nodes[0] = new Node(0, 0, 0) {Constraints = Constraints.Fixed, Label = "n0"};
                el2.Nodes[1] = new Node(l/2, 0, 0) { Label = "n1" };
                el2.Nodes[2] = new Node(l, 0, 0) {Constraints = Constraints.Fixed, Label = "n2"};

                el2._nodalReleaseConditions[2] = Constraints.Released;

                el2.Section = sec;
                el2.Material = mat;

                m2.Nodes.Add(el2.Nodes);
                m2.Elements.Add(el2);

                m2.Nodes["n1"].Loads.Add(new NodalLoad(new Force(0, 0, p, 0, 0, 0)));

                var k = (el2 as BarElement).GetLocalStifnessMatrix();
            }

            m1.Solve_MPC();
            m2.Solve_MPC();

            var d1 = m1.Nodes["n1"].GetNodalDisplacement();
            var d2 = m2.Nodes["n1"].GetNodalDisplacement();

        }

        public static void ValidateLoadInternalForce_B_y()
        {
            var load = new Loads.PartialNonUniformLoad();
            load.Direction = Vector.K;

            var w = 1;

            load.StartLocation = new IsoPoint(-1);
            load.EndLocation = new IsoPoint(-1);

            //load.StartMagnitude = new double[] { w };
            //load.EndMagnitude = new double[] { w };

            

            var elm = new BarElement(new Node(0, 0, 0), new Node(2, 0, 0));

            var hlpr = new BriefFiniteElementNet.ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y,elm);

            var l = (elm.Nodes[0].Location - elm.Nodes[1].Location).Length;

            var v = new Func<double, double>(x => w * (l / 2 - x));
            var m = new Func<double, double>(x => (w / 12.0) * (6 * l * x - l * l - 6 * x * x));

            var rdn = new Random(0);

            for (var i = 0; i < 10; i++)
            {
                var x_t = 
                    rdn.NextDouble()
                    //0.125
                    * l;

                var xi = hlpr.Local2Iso(elm, x_t);

                var frc = hlpr.GetLoadInternalForceAt(elm, load, xi);

                var rv = v(x_t);
                var rm = m(x_t);

                //var dv = v(x_t) - frc.MembraneTensor.S12;
                //var dm = m(x_t) - frc.BendingTensor.M13;
            }

        }

        public static void ValidateEndRelease()
        {
            var model = new Model();

            var ndes = new Node[] {
                new Node(0, 0, 0),
                new Node(3, 0, 0),
                    new Node(6, 0, 0),
            };

            /**/
            var h = 0.1;
            var w = 0.05;

            var a = h * w;
            var iy = h * h * h * w / 12;
            var iz = w * w * w * h / 12;
            var j = iy + iz;
            var e = 210e9;
            var nu = 0.3;

            var g = e / (2 * 1 + nu);
            /**/

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = UniformIsotropicMaterial.CreateFromYoungShear(e, g);

            {
                var belm = new BarElement(ndes[0], ndes[1]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };

                belm.StartReleaseCondition =
                    Constraints.FixedDX & Constraints.FixedDY & Constraints.FixedDZ &
                    Constraints.FixedRX;

            model.Elements.Add(belm);
            }
            {
                var belm = new BarElement(ndes[1], ndes[2]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };

                belm.EndReleaseCondition =
                    Constraints.FixedDX & Constraints.FixedDY & Constraints.FixedDZ &
                    Constraints.FixedRX;

                model.Elements.Add(belm);
            }


            model.Nodes.Add(ndes);

            ndes[0].Constraints = ndes[2].Constraints = Constraints.Fixed;

            //ndes[1].Constraints =
            //    Constraints.FixedDX & Constraints.FixedRX
            //& Constraints.FixedDY & Constraints.FixedRZ//find beam.z dofs

            ;
            //(model.Elements[0] as BarElement).Loads.Add(new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.K, 1000, CoordinationSystem.Local));

            ndes[1].Loads.Add(new NodalLoad(new Force(Vector.K, Vector.Zero)));
            //ndes[1].Loads.Add(new NodalLoad(new Force(Vector.J * 2, Vector.Zero)));

            model.Solve_MPC();


            var d = model.Nodes[1].GetNodalDisplacement();


            var t = (model.Elements[0] as BarElement).GetInternalForceAt(1);


            var res = OpenseesValidator.OpenseesValidate(model, LoadCase.DefaultLoadCase, false);

            var disp = res[0];

            var idx = disp.Columns["Absolute Error"].Ordinal;

            var max = disp.Rows.Cast<DataRow>().Select(i => (double)i[idx]).Max();


        }

    public static void testInternalForce_Console()
        {
            var model = new Model();
            var ndes = new Node[] { new Node(0, 0, 0), new Node(3, 0, 0) };

            var h = UnitConverter.In2M(4);
            var w = UnitConverter.In2M(4);

            var e = UnitConverter.Psi2Pas(20e4);

            var a = h * w;
            var iy = h * h * h * w / 12;
            var iz = w * w * w * h / 12;
            var j = iy + iz;

            var sec = new Sections.UniformParametric1DSection(a=1, iy=1, iz=1, j=1);
            var mat = UniformIsotropicMaterial.CreateFromYoungPoisson(e=1, 0.25);

            var elm = new BarElement(ndes[0], ndes[1]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };
            //var elm2 = new BarElement(ndes[1], ndes[2]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };

            model.Elements.Add(elm);
            model.Nodes.Add(ndes);

            ndes[0].Constraints = Constraints.Fixed;

            ndes[1].Loads.Add(new NodalLoad(new Force(1, 0, 1, 0, 0, 0)));

            model.Solve_MPC();

            var tr = elm.GetTransformationManager();

            var d1 = tr.TransformLocalToGlobal(elm.GetInternalDisplacementAt(1 - 1e-10, LoadCase.DefaultLoadCase));
            var d2 = ndes[1].GetNodalDisplacement(LoadCase.DefaultLoadCase);
           
            
            var frc = elm.GetInternalForceAt(-1, LoadCase.DefaultLoadCase);

            var gfrc = elm.GetTransformationManager().TransformLocalToGlobal(frc);

            var f0 = ndes[0].GetSupportReaction();

        }

        public static void ValidateLoadInternalForce_B_z()
        {
            var load = new Loads.PartialNonUniformLoad();
            load.Direction = Vector.J;

            var w = 1;

            //load.StartLocation = new double[] { -1 };
            //load.EndLocation = new double[] { 1 };

            //load.StartMagnitude = new double[] { w };
            //load.EndMagnitude = new double[] { w };

            

            var elm = new BarElement(new Node(0, 0, 0), new Node(2, 0, 0));

            var hlpr = new BriefFiniteElementNet.ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z, elm);

            var l = (elm.Nodes[0].Location - elm.Nodes[1].Location).Length;

            var v = new Func<double, double>(x => w * (l / 2 - x));
            var m = new Func<double, double>(x => -(w / 12.0) * (6 * l * x - l * l - 6 * x * x));

            var rdn = new Random(0);

            for (var i = 0; i < 10; i++)
            {
                var x_t =
                    rdn.NextDouble()
                    //0.125
                    * l;

                var xi = hlpr.Local2Iso(elm, x_t);

                var frc = hlpr.GetLoadInternalForceAt(elm, load, xi);

                var rv = v(x_t);
                var rm = m(x_t);

                //var dv = v(x_t) - frc.MembraneTensor.S13;
                //var dm = m(x_t) - frc.BendingTensor.M12;
            }

        }
        public ValidationResult[]  DoPopularValidation()
        {
            var buf = new List<ValidationResult>();

            buf.Add(Validation_1());
            buf.Add(Validation_2());

            return buf.ToArray();
        }

        public static void TestBarStiffness()
        {
            var iy = 0.02;
            var iz = 0.02;
            var a = 0.01;

            var j = iy + iz;

            var e = 210e9;
            var g = 70e9;
            //var rho = 13;

            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0));
            model.Nodes.Add(new Node(3, 5, 7));

            var barElement = new BarElement(model.Nodes[0], model.Nodes[1]);

            barElement.Behavior = BarElementBehaviours.FullFrame;
            barElement.Material = UniformIsotropicMaterial.CreateFromYoungShear(e, g);

            var frameElement = new FrameElement2Node(model.Nodes[0], model.Nodes[1])
            {
                Iy = iy,
                Iz = iz,
                A = a,
                J = j,
                E = e,
                G = g,
                //MassDensity = rho
            };

            frameElement.ConsiderShearDeformation = false;

            //barElement.Material = new UniformBarMaterial(e, g, rho);
            barElement.Section = new UniformParametric1DSection() { Iy = iy, Iz = iz, A = a };

            var frK = frameElement.GetGlobalStifnessMatrix();
            var barK = barElement.GetGlobalStifnessMatrix();

            var d = (frK - barK).Values.Max(i => Math.Abs(i));


        }

        public static ValidationResult Validation_1()
        {
            var nx = 5;
            var ny = 5;
            var nz = 5;

            #region model definition
            var grd = StructureGenerator.Generate3DBarElementGrid(nx, ny, nz);

            //StructureGenerator.SetRandomiseConstraints(grd);
            StructureGenerator.SetRandomiseSections(grd);

            StructureGenerator.AddRandomiseNodalLoads(grd, LoadCase.DefaultLoadCase);//random nodal loads
            StructureGenerator.AddRandomiseBeamUniformLoads(grd, LoadCase.DefaultLoadCase);//random elemental loads
            StructureGenerator.AddRandomDisplacements(grd, 0.1);

            //StructureGenerator.AddRandomSettlements(grd, 0.01);
            #endregion

            grd.Solve_MPC();

            #region solve & compare
            var res = OpenseesValidator.OpenseesValidate(grd, LoadCase.DefaultLoadCase, false);


            var disp = res[0];
            var reac = res[1];

            var dispAbsErrIdx = disp.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower(CultureInfo.CurrentCulture).Contains("absolute"));
            var dispRelErrIdx = disp.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower(CultureInfo.CurrentCulture).Contains("relative"));

            var reacAbsErrIdx = reac.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower(CultureInfo.CurrentCulture).Contains("absolute"));
            var reacRelErrIdx = reac.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower(CultureInfo.CurrentCulture).Contains("relative"));


            var maxDispAbsError = disp.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[dispAbsErrIdx]);
            var maxDispRelError = disp.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[dispRelErrIdx]);


            var maxReacAbsError = reac.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[reacAbsErrIdx]);
            var maxReacRelError = reac.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[reacRelErrIdx]);


            var maxInternalDisplacementAbsErr = 0.0;
            var maxInternalForceResidual = 0.0;

            foreach (var elm in grd.Elements)
            {
                var bar = elm as BarElement;

                if (bar == null)
                    continue;

                var tr = bar.GetTransformationManager();

                var L = (bar.StartNode.Location - bar.EndNode.Location).Length;

                var d1 = bar.GetInternalDisplacementAt(-1, LoadCase.DefaultLoadCase);
                var d2 = bar.GetInternalDisplacementAt(+1, LoadCase.DefaultLoadCase);

                var dn1 = tr.TransformGlobalToLocal(bar.StartNode.GetNodalDisplacement(LoadCase.DefaultLoadCase));
                var dn2 = tr.TransformGlobalToLocal(bar.EndNode.GetNodalDisplacement(LoadCase.DefaultLoadCase));

                var diff1 = dn1 - d1;
                var diff2 = dn2 - d2;

                var dd = Math.Max(diff1.Displacements.Length, diff2.Displacements.Length);
                var dr = Math.Max(diff1.Rotations.Length, diff2.Rotations.Length);

                maxInternalDisplacementAbsErr = Math.Max(maxInternalDisplacementAbsErr, dd);
                maxInternalDisplacementAbsErr = Math.Max(maxInternalDisplacementAbsErr, dr);
                //yet internal force

                var n = 10;

                var stf = bar.GetLocalStifnessMatrix();//.GetGlobalStifnessMatrix();

                var u1 = Displacement.ToVector(dn1);
                var u2 = Displacement.ToVector(dn2);

                var u = new Matrix(12, 1);

                u1.CopyTo(u.Values, 0);
                u2.CopyTo(u.Values, 6);

                var endFrc = stf * u;

                var stFc = Force.FromVector(endFrc.Values, 0);
                var endFc = Force.FromVector(endFrc.Values, 6);

                for (var i = 0; i < n; i++)
                {
                    var xi = i / (n - 1.0) * 2.0 + -1;
                    var x = bar.IsoCoordsToLocalCoords(xi)[0];

                    var fc = bar.GetInternalForceAt(xi);

                    var toBegin = fc.Move(new Vector(-x, 0, 0));
                    var toEnd = fc.Move(new Vector(L - x, 0, 0));

                    var stResid = stFc + toBegin;
                    var endResid = endFc - toEnd;

                    var errs = new double[]
                    {
                        stResid.Forces.Length,
                        stResid.Moments.Length,
                        endResid.Forces.Length,
                        endResid.Moments.Length
                    }.Max();

                    maxInternalForceResidual = Math.Max(maxInternalForceResidual, errs);
                }

            }
            #endregion

            
            var span = new HtmlTag("span");
            span.Add("p").Text("Validate 3D frame nodal displacement and reactions");
            span.Add("h3").Text("Validate with");
            span.Add("paragraph").Text("OpenSEES (the Open System for Earthquake Engineering Simulation) software (available via http://opensees.berkeley.edu/)");
            span.Add("h3").Text("Validate objective");


            span.Add("paragraph").Text("compare nodal displacement from BFE.net library and OpenSEES for a model consist of random 3d bars");

            span.Add("h3").Text("Model Definition");

            span.Add("paragraph").Text(string.Format(CultureInfo.CurrentCulture, "A {0}x{1}x{2} grid, with {3} nodes and {4} bar elements.",nx,ny,nz,grd.Nodes.Count,grd.Elements.Count) ).AddClosedTag("br");

            span.Add("paragraph").Text("Every node in the model have a random load on it, random displacement in original location.").AddClosedTag("br");
            span.Add("paragraph").Text("Every element in the model have a random uniform distributed load on it.").AddClosedTag("br");

            span.Add("h3").Text("Validation Result");

            #region nodal disp

            {//nodal displacements

                span.Add("h4").Text("Nodal Displacements");
                span.Add("paragraph")
               .Text(string.Format("Validation output for nodal displacements:"));


                span.Add("p").AddClass("bg-info").AppendHtml(string.Format(CultureInfo.CurrentCulture, "-Max ABSOLUTE Error: {0:e3}<br/>-Max RELATIVE Error: {1:e3}", maxDispAbsError, maxDispRelError));

                var id = "tbl_" + Guid.NewGuid().ToString("N", CultureInfo.CurrentCulture).Substring(0, 5);

                span.Add("button").Attr("type", "button").Text("Toggle Details").AddClasses("btn btn-primary")
                    .Attr("onclick", string.Format(CultureInfo.CurrentCulture, "$('#{0}').collapse('toggle');",id));

                var div = span.Add("div").AddClasses("panel-collapse", "collapse", "out").Id(id);

                var tbl = div.Add("table").AddClass("table table-striped table-inverse table-bordered table-hover");
                tbl.Id(id);

                var trH = tbl.Add("Thead").Add("tr");


                foreach (DataColumn column in disp.Columns)
                {
                    trH.Add("th").Attr("scope", "col").Text(column.ColumnName);
                }

                var tbody = tbl.Add("tbody");

                for (var i = 0; i < disp.Rows.Count; i++)
                {
                    var tr = tbody.Add("tr");

                    for (var j = 0; j < disp.Columns.Count; j++)
                    {
                        tr.Add("td").Text(disp.Rows[i][j].ToString());
                    }
                }
            }

            #endregion

            #region nodal reaction

            

           

            {//nodal reactions
                span.Add("h4").Text("Nodal Support Reactions");
                span.Add("paragraph")
               .Text(string.Format(CultureInfo.CurrentCulture, "Validation output for nodal support reactions:"));


                span.Add("p").AddClass("bg-info").AppendHtml(string.Format(CultureInfo.CurrentCulture, "-Max ABSOLUTE Error: {0:e3}<br/>-Max RELATIVE Error: {1:e3}", maxReacAbsError, maxReacRelError));

                var id = "tbl_" + Guid.NewGuid().ToString("N", CultureInfo.CurrentCulture).Substring(0, 5);

                span.Add("button").Attr("type", "button").Text("Toggle Details").AddClasses("btn btn-primary")
                    .Attr("onclick", string.Format(CultureInfo.CurrentCulture, "$('#{0}').collapse('toggle');",id));

                var div = span.Add("div").AddClasses("panel-collapse", "collapse", "out").Id(id);

                var tbl = div.Add("table").AddClass("table table-striped table-inverse table-bordered table-hover");
                tbl.Id(id);

                var trH = tbl.Add("Thead").Add("tr");


                foreach (DataColumn column in reac.Columns)
                {
                    trH.Add("th").Attr("scope", "col").Text(column.ColumnName);
                }

                var tbody = tbl.Add("tbody");

                for (var i = 0; i < reac.Rows.Count; i++)
                {
                    var tr = tbody.Add("tr");

                    for (var j = 0; j < reac.Columns.Count; j++)
                    {
                        tr.Add("td").Text(reac.Rows[i][j].ToString());
                    }
                }
            }

            #endregion

            #region internal displacement
            {//internal displacements

                span.Add("h4").Text("Internal Displacements");
                span.Add("paragraph")
                    .Text(string.Format(CultureInfo.CurrentCulture, "Validation output for internal displacements (Displacement at each end node of bar elements should be equal to bar element's internal displacement of bar element at that node)"));

                span.Add("p").AddClass("bg-info").AppendHtml(string.Format(CultureInfo.CurrentCulture, "-Max ABSOLUTE Error: {0:e3}", maxInternalDisplacementAbsErr));
            }
            #endregion

            #region internal force
            {//internal force

                span.Add("h4").Text("Internal Force");
                span.Add("paragraph")
                    .Text(string.Format(CultureInfo.CurrentCulture, "Validation output for internal force (forces retrived by K.Δ formula should be in equiblirium with internal force of bar elements at any location within element):"));

                span.Add("p").AddClass("bg-info").AppendHtml(string.Format(CultureInfo.CurrentCulture, "-Max ABSOLUTE Error: {0:e3}", maxInternalForceResidual));
            }
            #endregion

            var buf = new ValidationResult();
            buf.Span = span;
            buf.Title = "3D Grid Validation";

            return buf;
        }

        public static ValidationResult Validation_2()
        {
            var nx = 5;
            var ny = 5;
            var nz = 5;

            var grd = StructureGenerator.Generate3DBarElementGrid(nx, ny, nz);

            //StructureGenerator.SetRandomiseConstraints(grd);
            StructureGenerator.SetRandomiseSections(grd);

            StructureGenerator.AddRandomiseNodalLoads(grd, LoadCase.DefaultLoadCase);//random nodal loads
            StructureGenerator.AddRandomiseBeamUniformLoads(grd, LoadCase.DefaultLoadCase);//random elemental loads
            StructureGenerator.AddRandomDisplacements(grd, 0.1);

            var rnd = new Random(0);

            foreach (var elm in grd.Elements)
            {
                var bar = elm as BarElement;

                if (bar == null)
                    continue;

                /*
                if (rnd.Next() % 10 == 0)
                {
                    var cnd = RandomStuff.GetRandomConstraint();

                    cnd.DY = cnd.DZ = cnd.RY = cnd.RZ = DofConstraint.Fixed;

                    bar.StartReleaseCondition = cnd;

                }


                if (rnd.Next() % 10 == 1)
                {
                    var cnd = RandomStuff.GetRandomConstraint();

                    cnd.DY = cnd.DZ = cnd.RY = cnd.RZ = DofConstraint.Fixed;

                    bar.EndReleaseCondition = cnd;

                }
                */
            }

            grd.Solve_MPC();

            var maxEpsilon = 0.0;


            foreach (var elm in grd.Elements)
            {
                var bar = elm as BarElement;

                if (bar == null)
                    continue;

                var start = bar.StartReleaseCondition;
                var end = bar.EndReleaseCondition;


                var stFrc = bar.GetInternalForceAt(-1, LoadCase.DefaultLoadCase);
                var enFrc = bar.GetInternalForceAt(1, LoadCase.DefaultLoadCase);


                if (start != Constraints.Fixed)
                {
                    if (start.DX == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(stFrc.Fx), maxEpsilon);

                    if (start.DY == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(stFrc.Fy), maxEpsilon);

                    if (start.DZ == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(stFrc.Fz), maxEpsilon);

                    if (start.RX == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(stFrc.Mx), maxEpsilon);

                    if (start.RY == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(stFrc.My), maxEpsilon);

                    if (start.RZ == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(stFrc.Mz), maxEpsilon);
                }

                if (end != Constraints.Fixed)
                {
                    if (end.DX == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(enFrc.Fx), maxEpsilon);

                    if (end.DY == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(enFrc.Fy), maxEpsilon);

                    if (end.DZ == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(enFrc.Fz), maxEpsilon);

                    if (end.RX == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(enFrc.Mx), maxEpsilon);

                    if (end.RY == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(enFrc.My), maxEpsilon);

                    if (end.RZ == DofConstraint.Released)
                        maxEpsilon = Math.Max(Math.Abs(enFrc.Mz), maxEpsilon);
                }

            }

          


            var span = new HtmlTag("span");
            span.Add("p").Text("Validate a 3D frame internal force and internal displacement");
            span.Add("h3").Text("Validate objective");

            span.Add("paragraph").Text("validate internal force and internal displacement of bar elements. ")
                .AddClosedTag("br");

            span.Add("paragraph").Text("Internal displacement in each element at each end node should be equal to that node's displacement.").AddClosedTag("br");
            span.Add("paragraph").Text("End forces and mid force should be in equiblirium with each other.").AddClosedTag("br");

            span.Add("h3").Text("Model Definition");

            span.Add("paragraph").Text(string.Format(CultureInfo.CurrentCulture, "A {0}x{1}x{2} grid, with {3} nodes and {4} bar elements.",nx,ny,nz,grd.Nodes.Count,grd.Elements.Count)).AddClosedTag("br");

            span.Add("paragraph").Text("Every node in the model have a random load on it, random displacement in original location.").AddClosedTag("br");

            span.Add("h3").Text("Validation Result");


            /*
            {//internal displacements

                span.Add("h4").Text("Internal Displacements");
                span.Add("paragraph")
                    .Text(string.Format("Validation output for internal displacements:"));

                span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}", maxInternalDisplacementAbsErr));
            }


            {//internal force

                span.Add("h4").Text("Internal Force");
                span.Add("paragraph")
                    .Text(string.Format("Validation output for internal force (force residuals):"));

                span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}", maxInternalForceResidual));
            }
            */


            var buf = new ValidationResult();
            buf.Span = span;
            buf.Title = "Internal force & displacement validation";

            return buf;
        }
        public static void ValidateSingleInclinedFrame()
        {
            var model = new Model();
            var ndes = new Node[] { new Node(0, 0, 0), new Node(2, 3, 5) };

            var h = 0.1;
            var w = 0.05;

            var a = h * w;
            var iy = h * h * h * w / 12;
            var iz = w * w * w * h / 12;
            var j = iy + iz;

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = UniformIsotropicMaterial.CreateFromYoungPoisson(1, 0.25);

            var elm = new BarElement(ndes[0], ndes[1]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };
            //var elm2 = new BarElement(ndes[1], ndes[2]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };

            model.Elements.Add(elm);
            model.Nodes.Add(ndes);

            ndes[0].Constraints =  Constraints.Fixed;

            ndes[1].Loads.Add(new NodalLoad(new Force(0, 1, 0, 0, 0, 0)));

            model.Solve_MPC();

            var res = OpenseesValidator.OpenseesValidate(model, LoadCase.DefaultLoadCase, false);



        }


        public static void ValidateOneSpanUniformLoad()
        {
            var model = new Model();

            var ndes = new Node[] {
                new Node(0, 0, 0),
                new Node(1, 0, 0),
                //new Node(2, 0, 0)
            };

            var h = 0.1;
            var w = 0.05;

            var a = h * w;
            var iy = h * h * h * w / 12;
            var iz = w * w * w * h / 12;
            var j = iy + iz;
            var e = 210e9;

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = UniformIsotropicMaterial.CreateFromYoungPoisson(e, 0.25);

            BarElement e1;

            model.Elements.Add(e1 = new BarElement(ndes[0], ndes[1]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame });
            //model.Elements.Add(new BarElement(ndes[1], ndes[2]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame });

            e1.StartReleaseCondition = 
            //e1.EndReleaseCondition =
                Constraints.MovementFixed;

            var ld = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.K, 1000, CoordinationSystem.Global);

            var eqload = e1.GetGlobalEquivalentNodalLoads(ld);


            model.Nodes.Add(ndes);

            ndes[0].Constraints = ndes[2].Constraints = Constraints.Fixed;
            //ndes[1].Constraints = ndes[2].Constraints = Constraints.Fixed;

            //for (var i = 0; i < model.Elements.Count; i++)
            //    (model.Elements[i] as BarElement).Loads.Add();

            //ndes[1].Loads.Add(new NodalLoad(new Force(0, 1, 0, 0, 0, 0)));



            model.Solve_MPC();

            var res = OpenseesValidator.OpenseesValidate(model, LoadCase.DefaultLoadCase, false);
            var disp = res[0];

            var idx = disp.Columns["Absolute Error"].Ordinal;

            var max = disp.Rows.Cast<DataRow>().Select(i => (double)i[idx]).Max();


        }

        public static void ValidateConsoleUniformLoad()
        {
            var model = new Model();

            var ndes = new Node[] {
                new Node(0, 0, 0),
                new Node(3, 0, 0)};

            /**/
            var h = 0.1;
            var w = 0.05;

            var a =  h * w;
            var iy =  h * h * h * w / 12;
            var iz = w * w * w * h / 12;
            var j =  iy + iz;
            var e = 210e9;
            var nu = 0.3;

            var g = e / (2 * 1 + nu);
            /**/

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = UniformIsotropicMaterial.CreateFromYoungShear(e, g);

            BarElement belm;
            FrameElement2Node frmelm;

            belm = new BarElement(ndes[0], ndes[1]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };
            frmelm = new FrameElement2Node(ndes[0], ndes[1]) { Iz = sec.Iz, Iy = sec.Iy, A = sec.A, J = sec.J, E = e, G = g };

            var bk = belm.GetGlobalStifnessMatrix();
            var fk = frmelm.GetGlobalStifnessMatrix();
            var diff = bk-fk;

            model.Elements.Add(belm);

            model.Nodes.Add(ndes);

            ndes[0].Constraints = Constraints.Fixed;

            //ndes[1].Constraints = 
            //    Constraints.FixedDX & Constraints.FixedRX 
                //& Constraints.FixedDY & Constraints.FixedRZ//find beam.z dofs

            ;
            var ul = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.K, 1000, CoordinationSystem.Global);

            (model.Elements[0] as BarElement).Loads.Add(ul);


            var eqv = (model.Elements[0] as BarElement).GetGlobalEquivalentNodalLoads(ul);
            //ndes[1].Loads.Add(new NodalLoad(new Force(Vector.K, Vector.Zero)));
            //ndes[1].Loads.Add(new NodalLoad(new Force(Vector.J*2, Vector.Zero)));

            model.Solve_MPC();


            var d = model.Nodes[1].GetNodalDisplacement();

            var t = (model.Elements[0] as BarElement).GetInternalForceAt(-1);


            var res = OpenseesValidator.OpenseesValidate(model, LoadCase.DefaultLoadCase, false);

            var disp = res[0];

            var idx = disp.Columns["Absolute Error"].Ordinal;

            var max = disp.Rows.Cast<DataRow>().Select(i => (double)i[idx]).Max();


        }

        public static double epsilon = 1e-9;

        public static ValidationResult TestFixedEndMoment_uniformLoad()
        {
            var buff = new ValidationResult();

            buff.Title = "Test #1 for UniformLoad on BarElement";
            buff.Span.Add("p").Text("endforce from uniformload should be statically in equiblirium with uniform load");


            var elm = new BarElement(new Node(0,0,0), new Node(8.66, 0, 5));
        
            elm.Behavior = BarElementBehaviours.FullFrame;

            var ld = new Loads.UniformLoad();
            ld.Magnitude = 1;//*Math.Sqrt(2);
            ld.Direction = Vector.K;
            ld.CoordinationSystem = CoordinationSystem.Global;
            elm.Loads.Add(ld);

            var loads = elm.GetGlobalEquivalentNodalLoads(ld);

            {//test 1 : static balance

                var l = (elm.Nodes[1].Location - elm.Nodes[0].Location);

                var totEndForces = new Force();

                for (int i = 0; i < loads.Length; i++)
                {
                    totEndForces += loads[i].Move(elm.Nodes[i].Location, elm.Nodes[0].Location);
                }

                var d = l / 2;

                var gDir = ld.Direction;

                if (ld.CoordinationSystem == CoordinationSystem.Local)
                    gDir = elm.GetTransformationManager().TransformLocalToGlobal(ld.Direction);

                var cos = (1 / (d.Length * gDir.Length)) * Vector.Dot(d, gDir);

                var f_mid =  gDir * ld.Magnitude* (l.Length);//uniform load as concentrated load at middle
                var m = Vector.Cross(d, f_mid);

                var expectedForce = new Force(f_mid, m);
                var zero = totEndForces - expectedForce;

                buff.ValidationFailed = !zero.Forces.Length.FEquals(0, epsilon) || !zero.Moments.Length.FEquals(0, epsilon);
            }


            return buff;
        }

        public static ValidationResult Test_Trapezoid_1()
        {
            var buff = new ValidationResult();

            buff.Title = "Test #2 for Trapezoid Load on BarElement";

            buff.Span = new HtmlTag("span");

            buff.Span.Add("p").Text("endforces from Trapezoidal load with 0 offset and same start and end should be same as uniform load");

            var elm = new BarElement(new Node(0, 0, 0), new Node(1, 0, 0));

            elm.Behavior = BarElementBehaviours.BeamZ;

            var direction = Vector.K + Vector.I + Vector.J;
            var ld_u = new Loads.UniformLoad();
            ld_u.Magnitude = 1;//*Math.Sqrt(2);
            ld_u.Direction = direction;
            ld_u.CoordinationSystem = CoordinationSystem.Global;

            var ld_t = new Loads.PartialNonUniformLoad();
            //ld_t.EndLocation = ld_t.StartLocation = new double[] { 0 };
            //ld_t.StartMagnitude = ld_t.EndMagnitude = new double[] { 1 };
            ld_t.Direction = direction;
            ld_t.CoordinationSystem = CoordinationSystem.Global;

            var loads = elm.GetGlobalEquivalentNodalLoads(ld_u);
            var loads2 = elm.GetGlobalEquivalentNodalLoads(ld_t);

            var epsilon = 1e-9;

            {//test 1 : equality betweeb above

                var resid = new Force[loads.Length];

                for(var i = 0;i<loads.Length;i++)
                {
                    var f = resid[i] = loads[i] - loads2[i];

                    buff.ValidationFailed = Math.Abs(f.Forces.Length) > epsilon || Math.Abs(f.Moments.Length) > epsilon;
                }
            }


            return buff;
        }
    }
}
