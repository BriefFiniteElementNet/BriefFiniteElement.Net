using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue24
    {
        public static void Run()
        {
            var delta = 2;

            var n1 = new Node(0 * delta, 0, 0) { Constraints = Constraints.Fixed  };
            var n2 = new Node(1 * delta, 0, 0);
            var n3 = new Node(2 * delta, 0, 0);// { Constraints = Constraints.FixedDZ  };
            var n4 = new Node(3 * delta, 0, 0);// { Constraints = Constraints.FixedDZ  };
            var n5 = new Node(4 * delta, 0, 0) { Constraints = Constraints.Fixed };

            var e1 = new BarElement(n1, n2);
            var e2 = new BarElement(n2, n3);
            var e3 = new BarElement(n3, n4);// { StartReleaseCondition = Constraint.MovementFixed };
            var e4 = new BarElement(n4, n5);

            var sec = new Sections.UniformGeometric1DSection(SectionGenerator.GetRectangularSection(0.05, 0.05));
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            e1.Section = e2.Section = e3.Section = e4.Section = sec;
            e1.Material = e2.Material = e3.Material = e4.Material = mat;

            var model = new Model();

            var load = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.J + Vector.K, -100, CoordinationSystem.Global);
            
            e1.Loads.Add(load);
            e2.Loads.Add(load);
            e3.Loads.Add(load);
            e4.Loads.Add(load);

            var elms = new[] { e1, e2, e3, e4 };

            //n1.Loads.Add(new NodalLoad(new Force(0, 0, 100, 0, 0, 0)));
            //n2.Loads.Add(new NodalLoad(new Force(0, 0, 100, 0, 0, 0)));
            //n3.Loads.Add(new NodalLoad(new Force(0, 0, 100, 0, 0, 0)));
            //n4.Loads.Add(new NodalLoad(new Force(0, 0, 100, 0, 0, 0)));
            //n5.Loads.Add(new NodalLoad(new Force(0, 0, 100, 0, 0, 0)));

            model.Elements.Add(e1, e2, e3, e4);
            model.Nodes.Add(n1, n2, n3, n4,n5);

            model.Solve_MPC();

            var rnd = new Random();

            var fnc = new Func<double, double>(x =>
            {
                try
                {
                    var i = x / (delta );

                    var ii = (int)i;

                    var elm = elms[ii];

                    
                    var x_i = (i - ii) * delta;

                    //x_i = delta - x_i;

                    x_i += rnd.NextDouble() * 1e-3;

                    if (x_i < 0)
                        x_i = 0;

                    if (x_i > delta)
                        x_i = delta;

                    var xi = elm.LocalCoordsToIsoCoords(x_i)[0];

                    var f = elm.GetExactInternalForceAt(xi);

                    return f.Fz;
                }
                catch
                {
                    return 0;
                }
            });


            //Controls.FunctionVisualizer.VisualizeInNewWindow(fnc, 1E-6, 4 * delta - 1E-6, 537);
        }
    }
}
