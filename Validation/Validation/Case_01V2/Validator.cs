using System.Linq;
using BriefFiniteElementNet.Elements;
using HtmlTags;
using BriefFiniteElementNet.Materials;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BriefFiniteElementNet.Validation.Case_01V2
{
    //[ValidationCase("I Beam Torsion with TriangleElement", typeof(TriangleElement))]
    public class Validator : IValidationCase
    {
        public ValidationResult Validate()
        {
            var val = new ValidationResult();

            val.Title = "I Beam torsion with triangle element";

            var span = val.Span = new HtmlTag("span");

            {//report
                
                span.Add("p").Text("Validate an I Beam nodal displacement and reactions and internal forces");
                span.Add("h3").Text("Validate with");
                span.Add("paragraph").Text("Abaqus");
                span.Add("h3").Text("Validate objective");


                span.Add("paragraph").Text("compare nodal displacement for a model consist of Triangle Elements");

                span.Add("h3").Text("Model Definition");

                span.Add("paragraph")
                    .Text("An I shaped beam, totally fixed on one side and under a couple force on other side")
                    .AddClosedTag("br");

                span.Add("h3").Text("Validation Result");
            }
            //var magic = 0;

            //example #13 p175


            #region creating model

            //var l = UnitConverter.In2M(40);
            //var w = UnitConverter.In2M(10);
            //var h = UnitConverter.In2M(5);
            var t = 0.25;// UnitConverter.In2M(0.25);

            var e = 10000.0;// UnitConverter.Ksi2Pas(10000); //10'000 ksi
            var no = 0.3;

            var mat = new Materials.UniformIsotropicMaterial(e, no);
            var sec = new Sections.UniformParametric2DSection(t);

            var model = AbaqusInputFileReader.AbaqusInputToBFE("Case_01V2\\data1\\job-1.inp");

            {//inp file is imerial unit, bfe is metric

                foreach (var nde in model.Nodes)//location
                {
                    var lc = nde.Location;

                    lc.X = UnitConverter.In2M(lc.X);
                    lc.Y = UnitConverter.In2M(lc.Y);
                    lc.Z = UnitConverter.In2M(lc.Z);

                    //nde.Location = lc;


                    if (nde.Constraints == Constraints.RotationFixed)
                        nde.Constraints = Constraints.Released;
                }

                foreach (var nde in model.Nodes)//loads
                {
                   foreach(var ld in nde.Loads)
                    {
                        var f = ld.Force;

                        f.Fx = UnitConverter.Kip2N(f.Fx);
                        f.Fy = UnitConverter.Kip2N(f.Fy);
                        f.Fz = UnitConverter.Kip2N(f.Fz);

                        //ld.Force = f;
                    }
                }
            }


            foreach (var elm in model.Elements)
            {
                if (elm is TriangleElement tri)
                {
                    tri.Material = mat;
                    tri.Section = sec;

                    tri.MembraneFormulation = MembraneFormulation.PlaneStress;
                    tri.Behavior = PlaneElementBehaviours.FullThinShell;
                }
            }

            #endregion

            model.Trace.Listeners.Add(new Common.ConsoleTraceListener());
            new ModelWarningChecker().CheckModel(model);

            model.Solve_MPC();

            var bfeNodalDisp = model.Nodes.ToDictionary(i => i.Index, i => i.GetNodalDisplacement());

            //Dictionary<int, Displacement> abqNodalDisp;

            List<object[]> disp;
            List<object[]> stresses;

            using (var str = System.IO.File.OpenRead("Case_01V2\\data1\\node-disp.rpt"))
                disp =
                    AbaqusOutputFileReader.ReadTable(str,
                    AbaqusOutputFileReader.ColType.String,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real);

            using (var str = System.IO.File.OpenRead("Case_01V2\\data1\\element-stress.rpt"))
                stresses = 
                    AbaqusOutputFileReader.ReadTable(str,
                    AbaqusOutputFileReader.ColType.String,
                    AbaqusOutputFileReader.ColType.Int,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real,
                    AbaqusOutputFileReader.ColType.Real);

            var strs = stresses.Select(i => new
            {
                Label = (string)i[0],
                IntPt = (int)i[1],
                S11Loc1 = (double)i[2],
                S11Loc2 = (double)i[3],
                S22Loc1 = (double)i[4],
                S22Loc2 = (double)i[5],
                S33Loc1 = (double)i[6],
                S33Loc2 = (double)i[7],
                S12Loc1 = (double)i[8],
                S12Loc2 = (double)i[9],
            }).ToArray();

            var disps = disp.Select(i => new
            {
                Label = (string)i[0],
                Displacement = new Displacement((double)i[1], (double)i[2], (double)i[3], (double)i[4], (double)i[5], (double)i[6])
            }).ToArray();

            var strss = strs.Select(i => new
            {
                Label = i.Label,
                ptr = i.IntPt,
                st = new CauchyStressTensor(i.S11Loc1, i.S22Loc1, i.S33Loc1) { S12 = -i.S12Loc1, S21 = -i.S12Loc1 },//top stress
                sb = new CauchyStressTensor(i.S11Loc2, i.S22Loc2, i.S33Loc2) { S12 = -i.S12Loc2, S21 = -i.S12Loc2 },//bot stress
                sm = new CauchyStressTensor(0.5 * i.S11Loc2 + 0.5 * i.S11Loc1, 0.5 * i.S22Loc2 + 0.5 * i.S22Loc1, 0.5 * i.S33Loc2 + 0.5 * i.S33Loc1) { S12 = -0.5 * i.S12Loc2 + -0.5 * i.S12Loc1, S21 = -0.5 * i.S12Loc2 + -0.5 * i.S12Loc1 },//mid stress
            }).ToArray();

            
            var maxAbsErr = Vector.Zero;

            var nodalErrs = new List<double>();
            var elmErrs = new List<double>();

            var mids = new int[] { 42, 57, 58, 59, 60, 61, 62, 63, 34 };
            var sup = new int[] { 6, 11, 3, 34, 7, 2, 10 };
            var rest = Enumerable.Range(1, 63).Where(i => !mids.Contains(i) && !sup.Contains(i)).ToArray();

            var abqNodalDisp = disps.ToDictionary(i => i.Label, i => i.Displacement);


            foreach (var key in bfeNodalDisp.Keys)
            {
                var test_bfe = bfeNodalDisp[key];
                var ref_abq = abqNodalDisp[(key+1).ToString()];

                if (mids.Contains(key + 1) && !sup.Contains(key + 1))
                {
                    test_bfe.DY = ref_abq.DY;
                    Guid.NewGuid();
                }
                    

                if (key == 60)
                    Guid.NewGuid();

                var d = ref_abq - test_bfe;

                var du = d.Displacements;
                var dr = d.Rotations;

                var ru = du.Length / ref_abq.Displacements.Length;
                var rr = dr.Length / ref_abq.Rotations.Length;

                if (d.Displacements.Length > maxAbsErr.Length)
                    maxAbsErr = d.Displacements;

                nodalErrs.Add(ru);

                var r1 = test_bfe.DX / ref_abq.DX;
                var r2 = test_bfe.DY / ref_abq.DY;
                var r3 = test_bfe.DZ / ref_abq.DZ;
            }


            foreach (var key in bfeNodalDisp.Keys)
            {
                var test_bfe = bfeNodalDisp[key];
                var ref_abq = abqNodalDisp[(key + 1).ToString()];

                var nde = model.Nodes[key];

                //nde.SetNodalDisplacement(LoadCase.DefaultLoadCase, ref_abq);
            }

            //errs.Sort();


            var midErrs = mids.Select(i => nodalErrs[i - 1]).ToArray();
            var supErrs = sup.Select(i => nodalErrs[i - 1]).ToArray();
            var restErr = rest.Select(i => nodalErrs[i - 1]).ToArray();

            var restMax = restErr.Max();



            //var elm2 = model.Elements[64];


            var p1t = new double[] { 1 / 6.0, 1 / 6.0, 1 };
            var p1m = new double[] { 1 / 6.0, 1 / 6.0, 0 };
            var p1b = new double[] { 1 / 6.0, 1 / 6.0, -1 };

            var p2t = new double[] { 4 / 6.0, 1 / 6.0, 1 };
            var p2m = new double[] { 4 / 6.0, 1 / 6.0, 0 };
            var p2b = new double[] { 4 / 6.0, 1 / 6.0, -1 };

            var p3t = new double[] { 1 / 6.0, 4 / 6.0, 1 };
            var p3m = new double[] { 1 / 6.0, 4 / 6.0, 0 };
            var p3b = new double[] { 1 / 6.0, 4 / 6.0, -1 };


            var problems = new int[] { 82, 84, 86, 88, 90, 92, 94, 96 };
            var trs = new Matrix(3, 3);

            trs[0, 1] = 1;
            trs[1, 0] = -1;
            trs[2, 2] = 1;



            foreach (var elm in model.Elements)
            {
                if (elm is TriangleElement tri)
                {
                    var trns = tri.GetTransformationManager();


                    var s1t = tri.GetLocalInternalStress(p1t);
                    var s1m = tri.GetLocalInternalStress(p1m);
                    var s1b = tri.GetLocalInternalStress(p1b);

                    var s2t = tri.GetLocalInternalStress(p2t);
                    var s2m = tri.GetLocalInternalStress(p2m);
                    var s2b = tri.GetLocalInternalStress(p2b);

                    var s3t = tri.GetLocalInternalStress(p3t);
                    var s3m = tri.GetLocalInternalStress(p3m);
                    var s3b = tri.GetLocalInternalStress(p3b);

                    if (tri.Label == "94")
                        Guid.NewGuid();

                    if (problems.Contains(tri.Label.ToInt()))
                    {
                        s1m = CauchyStressTensor.Transform(s1m, trs);
                        s2m = CauchyStressTensor.Transform(s2m, trs);
                        s3m = CauchyStressTensor.Transform(s3m, trs);

                        s1t = CauchyStressTensor.Transform(s1t, trs);
                        s2t = CauchyStressTensor.Transform(s2t, trs);
                        s3t = CauchyStressTensor.Transform(s3t, trs);

                        s1b = CauchyStressTensor.Transform(s1b, trs);
                        s2b = CauchyStressTensor.Transform(s2b, trs);
                        s3b = CauchyStressTensor.Transform(s3b, trs);
                    }

                    var tag = tri.Label;

                    var as1 = strss.FirstOrDefault(i => i.ptr == 1 && i.Label == tag);
                    var as2 = strss.FirstOrDefault(i => i.ptr == 2 && i.Label == tag);
                    var as3 = strss.FirstOrDefault(i => i.ptr == 3 && i.Label == tag);

                    var fnc = new Func<CauchyStressTensor, double[]>(i => new double[] { i.S11, i.S22, i.S33, i.S12 });

                    var e1t = ErrorUtil.GetRelativeError(as1.sb, -s1t, fnc);
                    var e1m = ErrorUtil.GetRelativeError(as1.sm, -s1m, fnc);
                    var e1b = ErrorUtil.GetRelativeError(as1.st, -s1b, fnc);

                    var e2t = ErrorUtil.GetRelativeError(as2.sb, -s2t, fnc);
                    var e2m = ErrorUtil.GetRelativeError(as2.sm, -s2m, fnc);
                    var e2b = ErrorUtil.GetRelativeError(as2.st, -s2b, fnc);

                    var e3t = ErrorUtil.GetRelativeError(as3.sb, -s3t, fnc);
                    var e3m = ErrorUtil.GetRelativeError(as3.sm, -s3m, fnc);
                    var e3b = ErrorUtil.GetRelativeError(as3.st, -s3b, fnc);

                    var eMax =
                        CalcUtil.NormInf(e1t, e1b, e2t, e2b, e3t, e3b);
                        //CalcUtil.NormInf(e1m, e2m, e3m);//max abs value

                    if (tri.Label == "51")
                        Guid.NewGuid();

                    elmErrs.Add(eMax);
                }
            }

            

            var maxStress = elmErrs.Max();
            var avg = elmErrs.Average();

            var tmp = Enumerable.Range(0, elmErrs.Count).Select(i => Tuple.Create(i + 1, elmErrs[i])).OrderBy(i => -i.Item2).ToList();

            elmErrs.Sort();

            Guid.NewGuid();

            /*
            {//nodal displacements

                span.Add("h4").Text("Nodal Displacements");
                span.Add("paragraph").Text(string.Format("Validation output for nodal displacements:"));

                var da = 1 / 0.0254 * A.GetNodalDisplacement().Displacements; // [inch]
                var db = 1 / 0.0254 * B.GetNodalDisplacement().Displacements; // [inch]

                var d50 = 1 / 0.0254 * n50.GetNodalDisplacement().Displacements; // [inch]
                var d56 = 1 / 0.0254 * n56.GetNodalDisplacement().Displacements; // [inch]
                var d57 = 1 / 0.0254 * n57.GetNodalDisplacement().Displacements; // [inch]

                var sap2000Da = new Vector(-0.014921, 0.085471, 0.146070); //tbl 7.14
                var sap2000Db = new Vector(-0.014834, -0.085475, -0.144533); //tbl 7.14

                var abaqusDa = new Vector(-15.4207E-03, 88.2587E-03, 150.910E-03); //node 9
                var abaqusDb = new Vector(-15.3246E-03, -88.2629E-03, -148.940E-03); //node 5

                var abaqus8 = new Vector(-120.875E-06, 88.3894E-03, -1.01662E-03); //node 8
                var abaqus12 = new Vector(15.3931E-03, 89.1206E-03, -149.515E-03); //node 12
                var abaqus41 = new Vector(-189.084E-06, 72.3778E-03, -734.918E-06); //node 41


                span.Add("paragraph").Text(string.Format("Validation output for nodal displacements:"));

                span.Add("paragraph").Text(string.Format("Err at node A (displacement): {0:0.00}%", Util.GetErrorPercent(da, abaqusDa))).AddClosedTag("br"); ;
                span.Add("paragraph").Text(string.Format("Err at node B (displacement): {0:0.00}%", Util.GetErrorPercent(db, abaqusDb))).AddClosedTag("br"); ;

                span.Add("paragraph").Text(string.Format("Err at node 41 (57) (displacement): {0:0.00}%", Util.GetErrorPercent(d50, abaqus41))).AddClosedTag("br"); ;
                span.Add("paragraph").Text(string.Format("Err at node 12 (56) (displacement): {0:0.00}%", Util.GetErrorPercent(d56, abaqus12))).AddClosedTag("br"); ;
                span.Add("paragraph").Text(string.Format("Err at node 08 (50) (displacement): {0:0.00}%", Util.GetErrorPercent(d57, abaqus8))).AddClosedTag("br"); ;
            }

            {//element stress
                {
                    var e81 = model.Elements[85] as TriangleElement;

                    var tr = e81.GetTransformationManager();

                    //t = 1/t;

                    var am = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 1 / 6.0, 0) * (1 / t));
                    var at = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 1 / 6.0, +1) * (1 / t));
                    var ab = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 1 / 6.0, -1) * (1 / t));

                    var bm = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 4 / 6.0, 1 / 6.0, 0) * (1 / t));
                    var bt = tr.TransformGlobalToLocal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 4 / 6.0, 1 / 6.0, +1) * (1 / t));
                    var bb = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 4 / 6.0, 1 / 6.0, -1) * (1 / t));

                    var cm = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 4 / 6.0, 0) * (1 / t));
                    var ct = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 4 / 6.0, +1) * (1 / t));
                    var cb = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 4 / 6.0, -1) * (1 / t));

                    var abacus_at = new CauchyStressTensor() { S11 = 103.814E-03, S22 = 249.185E-03, S12 = 1.03438, S21 = 1.03438 } * -1e9;
                    var abacus_bt = new CauchyStressTensor() { S11 = -34.7168E-03, S22 = -538.942E-03, S12 = 1.03438, S21 = 1.08243 } * -1e9;
                    var abacus_ct = new CauchyStressTensor() { S11 = -201.062E-03, S22 = -1.18348, S12 = 747.243E-03, S21 = 747.243E-03 } * -1e9;

                    var e1 = Util.GetErrorPercent(at, abacus_ct);
                    var e2 = Util.GetErrorPercent(bt, abacus_at);
                    var e3 = Util.GetErrorPercent(ct, abacus_bt);

                    span.Add("paragraph").Text(string.Format("Validation output for element stress:"));

                    span.Add("paragraph").Text(string.Format("Err at p1 element 81 (stress): {0:0.00}%", e1)).AddClosedTag("br"); 
                    span.Add("paragraph").Text(string.Format("Err at p2 element 81 (stress): {0:0.00}%", e2)).AddClosedTag("br"); 
                    span.Add("paragraph").Text(string.Format("Err at p3 element 81 (stress): {0:0.00}%", e3)).AddClosedTag("br"); 

                    //in abaqus e81 connected to 8-12-41
                    //in bfe e85 connected to 57-56-50
                }

            }
            */
            return val;
        }

        /// <summary>
        /// convert abaqus iso coords to bfe coords
        /// </summary>
        /// <param name="elm">target triangle element</param>
        /// <param name="elementTransformation">target triangle element</param>
        /// <param name="v">coordinations in <see cref="elm"/>'s local coordination system (abaqus)</param>
        /// <returns></returns>
        public static double[] AbaqusCoord2BfeCoord(TriangleElement elm, Vector v)
        {
            ///abaqus local coordination def: comment from https://engineering.stackexchange.com/questions/48285/abaqus-stri3-element-local-coordination-system?noredirect=1#comment87622_48285
            ///bfe local coordination def: comment from

            {
                //abaqus
                var v12 = elm.Nodes[1].Location - elm.Nodes[0].Location;//from node 2 to node 1
                var v13 = elm.Nodes[2].Location - elm.Nodes[0].Location;//from node 3 to node 1

                var n = Vector.Cross(v12, v13).GetUnit();//unit of positive normal

                var cosOneTenth = Math.Cos(0.1*Math.PI / 180);//cos(0.1 deg)

                var takeI = false;//take projection of I vector as local x dir
                var cos_normal_i = Vector.Dot(n, Vector.I) ;//cos angle between I and positive normal unit
                var angle_n_i = Math.Acos(cos_normal_i) * 180.0 / Math.PI;

                //due to Math.Acos documentation, 0° ≤ angle_n_i ≤ 180°
                if (angle_n_i < 179.9 && angle_n_i > 0.1)
                    takeI = true;
                else
                    takeI = false;

                var _1Axis = takeI ? Vector.I : Vector.J;//needs to be projected on the plane with normal of `n`

                //vector to plane project https://www.maplesoft.com/support/help/maple/view.aspx?path=MathApps%2FProjectionOfVectorOntoPlane

                var alpha = Vector.Dot(n, _1Axis) / n.Length;

                var proj1 = _1Axis - alpha * n;//it is local x axis

                var local1 = proj1;//local x axis in global 3d 
                var local3 = n;//local y axis in global 3d 
                var local2 = Vector.Cross(local3, local1);//local z axis in global 3d 

                var lamX = local1.GetUnit();//Lambda_X
                var lamY = local2.GetUnit();//Lambda_Y
                var lamZ = local3.GetUnit();//Lambda_Z

                var lambda = Matrix.OfJaggedArray(new[]//eq. 5.13
                {
                    new[] {lamX.X, lamY.X, lamZ.X},
                    new[] {lamX.Y, lamY.Y, lamZ.Y},
                    new[] {lamX.Z, lamY.Z, lamZ.Z}
                });

                var tr = TransformManagerL2G.MakeFromLambdaMatrix(lambda.AsMatrix());

                //lambda for transform between abaqus local coordination system <> global system

                //first need to convert v from element coord system to 
                //TODO: these are right only if both abaqus and bfe local system origins are same 

                Vector local;

                {//transform from abaus local to bfe local
                    var abaqusLocal = v;
                    var global = tr.TransformLocalToGlobal(abaqusLocal);
                    var bfeLocal = elm.GetTransformationManager().TransformGlobalToLocal(global);

                    local = bfeLocal;
                }
                

                return new double[] { local.X, local.Y, local.Z };
            }
        }


        public static CauchyStressTensor Avg(CauchyStressTensor s1, CauchyStressTensor s2)
        {
            var sum = s1 + s2;
            return sum * 0.5;
        }
    }
}
