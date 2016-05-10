using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Validation
{
    public class DkqElementChecker
    {
        public static void Test1()
        {
            #region creating model

            var model = new Model();

            var l = 144 * 0.0254; //in [m]
            var t = 6 * 0.0254;//

            var e = 24821126255.44; //3600 ksi
            var no = 0.3; //Poisson ratio

            var n = 9;

            var span = l / (n - 1);

            var nodes = new Node[n][];

            for (var i = 0; i < n; i++)
            {
                nodes[i] = new Node[n];

                for (var j = 0; j < n; j++)
                {
                    model.Nodes.Add(nodes[i][j] = new Node(i * span, j * span, 0));

                    if (i == 0 || i == n - 1 || j == 0 || j == n - 1)
                        nodes[i][j].Constraints = Constraint.Fixed;

                    var newCns = new Constraint(DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Released,
                        DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed);

                    nodes[i][j].Constraints = nodes[i][j].Constraints & newCns;
                }
            }

            for (var i = 0; i < n - 1; i++)
            {
                for (var j = 0; j < n - 1; j++)
                {
                    var elm1 = new DkqElement() { ElasticModulus = e, PoissonRatio = no, Thickness = t };

                    elm1.Nodes[3] = nodes[i][j];
                    elm1.Nodes[2] = nodes[i][j + 1];
                    elm1.Nodes[1] = nodes[i + 1][j + 1];
                    elm1.Nodes[0] = nodes[i + 1][j];


                    model.Elements.Add(elm1);
                }
            }

            //loading, 0.01 kips/sq. in. on all elements
            foreach (var elm in model.Elements)
            {
                elm.Loads.Add(new UniformLoadForPlanarElements()
                {
                    CoordinationSystem = CoordinationSystem.Global,
                    Uz = 68947.5728 //0.01 kips/sq. in.
                });
            }

            //test loading, not one who is defined at pdf
            /*
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    nodes[i][j].Loads.Add(new NodalLoad(new Force(0, 0, 100, 0, 0, 0)));
                }
            }
            */


            #endregion

            model.Trace.Listeners.Add(new ConsoleTraceListener());
            new ModelWarningChecker().CheckModel(model);

            model.Solve();

            var d = nodes[n / 2][n / 2].GetNodalDisplacement();

            //Controls.ModelVisualizerControl.VisualizeInNewWindow(model);
        }

    }
}
