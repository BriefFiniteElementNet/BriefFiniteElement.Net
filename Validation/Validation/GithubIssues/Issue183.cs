using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue183
    {
        public static void Run()
        {
            
            //Section
            var uniformParametric1DSection = new Sections.UniformParametric1DSection(6400, 1706670, 1706670, 1000);
            //Material
            var uniformIsotropicMaterial = new UniformIsotropicMaterial(9350, 0.3);

            BriefFiniteElementNet.Node node1 = new BriefFiniteElementNet.Node(-300, 0, 0) { Label = "n1" };
            BriefFiniteElementNet.Node node2 = new BriefFiniteElementNet.Node(0, 0, 0) { Label = "n2" };
            BriefFiniteElementNet.Node node3 = new BriefFiniteElementNet.Node(300, 0, 0) { Label = "n3" };

            //Constraint
            node1.Constraints = new Constraint(DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Released, DofConstraint.Fixed);
            node2.Constraints = new Constraint(DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Released, DofConstraint.Fixed);
            node3.Constraints = new Constraint(DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Released, DofConstraint.Fixed);

            //Element
            double eleLength = 300 - 0.0;
            Element element1 = new BarElement(node1, node2)
            {
                //Behavior = BarElementBehaviour.BeamZEulerBernoulli,
                Section = uniformParametric1DSection,
                Material = uniformIsotropicMaterial,
            };
            Element element2 = new BarElement(node2, node3)
            {
                //Behavior = BarElementBehaviour.BeamZEulerBernoulli,
                Section = uniformParametric1DSection,
                Material = uniformIsotropicMaterial,
            };

            //element uniform load
            element1.Loads.Add(new UniformLoad(LoadCase.DefaultLoadCase, new BriefFiniteElementNet.Vector(0, 0, 1), -0.0208, CoordinationSystem.Global));
            element2.Loads.Add(new UniformLoad(LoadCase.DefaultLoadCase, new BriefFiniteElementNet.Vector(0, 0, 1), -0.0208, CoordinationSystem.Global));

            //concentrated load
            List<double> ptLoadCoords = new List<double>() { -250, -150, -50, 50, 150, 250 };

            for (int i = 0; i < ptLoadCoords.Count(); i++)
            {
                if (ptLoadCoords[i] < 0)  //left element
                {
                    double isoPointCoord = (2.0 * (ptLoadCoords[i] - element1.Nodes[0].Location.X) - eleLength) / eleLength;
                    IsoPoint isoPoint = new IsoPoint(isoPointCoord);
                    element1.Loads.Add(new ConcentratedLoad(new Force(0, 0, -287.71875, 0, 0, 0), isoPoint, CoordinationSystem.Global));
                }
                else //right element
                {
                    double isoPointCoord = (2.0 * (ptLoadCoords[i] - element2.Nodes[0].Location.X) - eleLength) / eleLength;
                    IsoPoint isoPoint = new IsoPoint(isoPointCoord);
                    element2.Loads.Add(new ConcentratedLoad(new Force(0, 0, -287.71875, 0, 0, 0), isoPoint, CoordinationSystem.Global));
                }
            }

            //Model
            BriefFiniteElementNet.Model model = new BriefFiniteElementNet.Model();
            model.Nodes.Add(node1, node2, node3);
            model.Elements.Add(element1, element2);

            //model.Solve();
            model.Solve_MPC();

            //Return Result

            //Return IsoPoint, only (-1,0,0) and (1,0,0) returned
            IsoPoint[] discreteIsoPoints = (element1 as BarElement).GetInternalForceDiscretationPoints();

            //Use -250 to calculate a IsoPoint, which is equal to -2/3.
            double currIsoPoint11 = (2.0 * (-250 - element1.Nodes[0].Location.X) - eleLength) / eleLength;
            //Exception thrown: 'BriefFiniteElementNet.InvalidInternalForceLocationException' in BriefFiniteElementNet.dll
            //The reason for the exception may be that a concentrated load is added on the same location
            Force force = (element1 as BarElement).GetExactInternalForceAt(currIsoPoint11);

        }
    }
}
