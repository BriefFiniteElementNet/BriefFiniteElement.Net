﻿using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.Data.FlatShell___triangle.IBeamTorsion
{
    public class Validator : IValidator
    {
        public ValidationResult[] DoAllValidation()
        {
            throw new NotImplementedException();
        }


        public ValidationResult Validate()
        {

            var val = new ValidationResult();

            //var magic = 0;

            //example #13 p175

            #region creating model

            var model = new Model();

            var l = UnitConverter.In2M(40);
            var w = UnitConverter.In2M(10);
            var h = UnitConverter.In2M(5);
            var t = UnitConverter.In2M(0.25);

            var e = UnitConverter.Ksi2Pas(10000); //10'000 ksi
            var no = 0.3;

            var n = 9;

            var xSpan = l / (n - 1);

            var nodes = new Node[n][];

            for (var i = 0; i < n; i++)
            {
                var x = i * xSpan;

                nodes[i] = new Node[7];

                nodes[i][0] = new Node(x, 0, 0);
                nodes[i][1] = new Node(x, w / 2, 0);
                nodes[i][2] = new Node(x, w, 0);

                nodes[i][3] = new Node(x, w / 2, h / 2);

                nodes[i][4] = new Node(x, 0, h);
                nodes[i][5] = new Node(x, w / 2, h);
                nodes[i][6] = new Node(x, w, h);

                model.Nodes.AddRange(nodes[i]);
            }

            var pairs = new int[6][];

            pairs[0] = new int[] { 0, 1 };
            pairs[1] = new int[] { 1, 2 };
            pairs[2] = new int[] { 1, 3 };
            pairs[3] = new int[] { 3, 5 };
            pairs[4] = new int[] { 4, 5 };
            pairs[5] = new int[] { 5, 6 };


            var mat = new Materials.UniformIsotropicMaterial( e,no);
            var sec = new Sections.UniformParametric2DSection(t);


            for (var i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var n11 = nodes[i][pairs[j][0]];
                    var n12 = nodes[i][pairs[j][1]];

                    var n21 = nodes[i + 1][pairs[j][0]];
                    var n22 = nodes[i + 1][pairs[j][1]];

                    {
                        var elm1 = new TriangleElement() { Material = mat, Section = sec };

                        elm1.Nodes[0] = n11;
                        elm1.Nodes[1] = n12;
                        elm1.Nodes[2] = n21;

                        model.Elements.Add(elm1);

                        var elm2 = new TriangleElement() { Material = mat, Section = sec };

                        elm2.Nodes[0] = n21;
                        elm2.Nodes[1] = n22;
                        elm2.Nodes[2] = n12;

                        model.Elements.Add(elm2);
                    }
                }
            }

            //loading
            nodes.Last()[0].Loads.Add(new NodalLoad(new Force(0, UnitConverter.Kip2N(1.6), 0, 0, 0, 0)));
            nodes.Last()[6].Loads.Add(new NodalLoad(new Force(0, -UnitConverter.Kip2N(1.6), 0, 0, 0, 0)));

            nodes[0].ToList().ForEach(i => i.Constraints = Constraints.Fixed);

            #endregion

            model.Trace.Listeners.Add(new BriefFiniteElementNet.Common.ConsoleTraceListener());
            new ModelWarningChecker().CheckModel(model);


            

            model.Solve();

            var A = nodes.Last()[2];
            var B = nodes.Last()[4];
            var C = nodes.First()[1];
            var D = nodes.First()[0];
            var E = nodes.Last()[6];

            /*
            for (int i = 0; i < nodes.Last().Length; i++)
            {
                nodes.Last()[i].Label = i.ToString();
            }
            */

            /**/
            A.Label = "A";
            B.Label = "B";
            C.Label = "C";
            D.Label = "D";
            E.Label = "E";
            /**/


            var n50 = model.Nodes[50];
            var n56 = model.Nodes[56];
            var n57 = model.Nodes[57];



            val.Title = "I Beam torsion with triangle element";

            var da = 1 / 0.0254 * A.GetNodalDisplacement().Displacements; // [inch]
            var db = 1 / 0.0254 * B.GetNodalDisplacement().Displacements; // [inch]

            var d50 = 1 / 0.0254 * n50.GetNodalDisplacement().Displacements; // [inch]
            var d56 = 1 / 0.0254 * n56.GetNodalDisplacement().Displacements; // [inch]
            var d57 = 1 / 0.0254 * n57.GetNodalDisplacement().Displacements; // [inch]

            var sap2000Da = new Vector(-0.014921, 0.085471, 0.146070); //tbl 7.14
            var sap2000Db = new Vector(-0.014834, -0.085475, -0.144533); //tbl 7.14

            var abaqusDa = new Vector(-15.4207E-03, 88.2587E-03, 150.910E-03); //node 9
            var abaqusDb = new Vector(-15.3246E-03, -88.2629E-03, -148.940E-03); //node 5

            var abaqus8 = new Vector(-120.875E-06   ,  88.3894E-03 ,- 1.01662E-03); //node 8
            var abaqus12 = new Vector(15.3931E-03   ,  89.1206E-03 ,- 149.515E-03); //node 12
            var abaqus41 = new Vector(-189.084E-06, 72.3778E-03, -734.918E-06); //node 41

            Console.WriteLine("Err at A against abaqus (displacement): {0:0.00}%", GetError(da, abaqusDa));
            Console.WriteLine("Err at B against abaqus (displacement): {0:0.00}%", GetError(db, abaqusDb));

            Console.WriteLine("Err at 41 (57) against abaqus (displacement): {0:0.00}%", GetError(d50, abaqus41));
            Console.WriteLine("Err at 12 (56) against abaqus (displacement): {0:0.00}%", GetError(d56, abaqus12));
            Console.WriteLine("Err at 08 (50) against abaqus (displacement): {0:0.00}%", GetError(d57, abaqus8));


            model.ReIndexElements();
            model.ReIndexNodes();

            //ModelVisualizerControl.VisualizeInNewWindow(model);

            {
                var e81 = model.Elements[85] as TriangleElement;


                var tr = e81.GetTransformationManager();

                //t = 1/t;

                var am = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 1 / 6.0, 0) * (1 / t));
                var at = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 1 / 6.0, +1) * (1 / t));
                var ab = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 1 / 6.0, -1) * (1 / t));

                var bm = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 4 / 6.0, 1 / 6.0, 0) * (1 / t));
                var bt = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 4 / 6.0, 1 / 6.0, +1) * (1 / t));
                var bb = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 4 / 6.0, 1 / 6.0, -1) * (1 / t));

                var cm = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 4 / 6.0, 0) * (1 / t));
                var ct = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 4 / 6.0, +1) * (1 / t));
                var cb = tr.TransformLocalToGlobal(e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 4 / 6.0, -1) * (1 / t));

                var abacus_at = new CauchyStressTensor() { S11 = 103.814E-03, S22 = 249.185E-03, S12 = 1.03438, S21 = 1.03438 } * -1e9;
                var abacus_bt = new CauchyStressTensor() { S11 = -34.7168E-03, S22 = -538.942E-03, S12 = 1.03438, S21 = 1.08243 } * -1e9;
                var abacus_ct = new CauchyStressTensor() { S11 = -201.062E-03, S22 = -1.18348, S12 = 747.243E-03, S21 = 747.243E-03 } * -1e9;

                var e1 = GetError(at, abacus_ct);
                var e2 = GetError(bt, abacus_at);
                var e3 = GetError(ct, abacus_bt);

                Console.WriteLine("Err at p1 element 81 (stress): {0:0.00}%", e1);
                Console.WriteLine("Err at p2 element 81 (stress): {0:0.00}%", e2);
                Console.WriteLine("Err at p3 element 81 (stress): {0:0.00}%", e3);

                //in abaqus e81 connected to 8-12-41
                //in bfe e85 connected to 57-56-50
            }

            for (int i = 0; i < model.Elements.Count; i++)
                model.Elements[i].Label = i.ToString();

            throw new NotImplementedException();
        }

        private static double GetError(double test, double accurate)
        {
            var buf = Math.Abs(test - accurate) / Math.Abs(accurate);

            return 100 * buf;
        }

        private static double GetError(Vector test, Vector accurate)
        {
            if (test == accurate)
                return 0;

            return 100 * Math.Abs((test - accurate).Length) / Math.Max(test.Length, accurate.Length);
        }

        private static double GetError(CauchyStressTensor test, CauchyStressTensor accurate)
        {
            var f1t = new Vector(test.S11, test.S12, test.S13);
            var f1a = new Vector(accurate.S11, accurate.S12, accurate.S13);

            var f2t = new Vector(test.S21, test.S22, test.S23);
            var f2a = new Vector(accurate.S21, accurate.S22, accurate.S23);

            var f3t = new Vector(test.S31, test.S32, test.S33);
            var f3a = new Vector(accurate.S31, accurate.S32, accurate.S33);

            var e1 = GetError(f1t, f1a);
            var e2 = GetError(f2t, f2a);
            var e3 = GetError(f3t, f3a);


            return e1 + e2 + e3;
        }



        public ValidationResult[] DoPopularValidation()
        {
            Console.Clear();
            return new ValidationResult[] { Validate() };
        }
    }
}
