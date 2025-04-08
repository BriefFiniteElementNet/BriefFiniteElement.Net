using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.MpcElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlTags;
using System.Globalization;


namespace BriefFiniteElementNet.Validation.Case_04
{
    [ValidationCase("RigidElement (MPC)", false, typeof(MpcElements.RigidElement_MPC))]
    public class Validator : IValidationCase
    {
        public ValidationResult Validate()
        {
            var model = new Model();

            var ndes = new Node[] {
                new Node(0, 0, 0),
                new Node(1, 0, 0),
                new Node(2, 0, 0),
                new Node(3, 0, 0),
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

            //bar-element -> rigid linkg -> bar element
            {
                var barElm = new BarElement(ndes[0], ndes[1]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };
                model.Elements.Add(barElm);
            }
            {
                var rigidElm = new RigidElement_MPC();
                rigidElm.Nodes = new NodeList() { ndes[1], ndes[2] };
                rigidElm.UseForAllLoads = true;
                model.MpcElements.Add(rigidElm);
            }
            {
                var barElm = new BarElement(ndes[2], ndes[3]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };
                model.Elements.Add(barElm);
            }
            model.Nodes.Add(ndes);

            ndes[0].Constraints = Constraints.Fixed;

            ndes[3].Loads.Add(new NodalLoad(new Force(1000, 2000, 5000, 1000, 2000, 2000)));
            //ndes[1].Loads.Add(new NodalLoad(new Force(Vector.J * 2, Vector.Zero)));

            model.Solve_MPC();

            var span = new HtmlTag("span");
            span.Add("p").Text("Validation of rigid element");
            span.Add("paragraph").Text("The rigid element connects different nodes through non-deformable elements. ");
            span.Add("h3").Text("Model Definition");

            span.Add("paragraph").Text(string.Format(CultureInfo.CurrentCulture, "A linear beam of 3 meters long, with 4 nodes, 2 bar elements and 1 rigid element in-between.")).AddClosedTag("br");

            span.Add("paragraph").Text("The first node is fixed in the 3D space.").AddClosedTag("br");
            span.Add("paragraph").Text("The last node is loaded with a random load; both forces and moments.").AddClosedTag("br");
            span.Add("paragraph").Text("The rigid element should transfer all rotational DOF from the second node to the thrid. The displacements  along the beam will be trasnfered ass well, although the the displacements in the perpendicular direct will scale.").AddClosedTag("br");

            span.Add("h3").Text("Validation Result");
            span.Add("h4").Text("Nodal Displacements");
            span.Add("paragraph").Text(string.Format("Validation output for nodal displacements:"));


            var n1 = model.Nodes[1].GetNodalDisplacement();
            var n2 = model.Nodes[2].GetNodalDisplacement();
            var diff = n1 - n2;
            //linear beam with loads -> rigid elements introduces rigid body -> Y and Z (translations perpendicular to the beam) are non zero and scaled. Set to zero to avoid confusion with result.
            diff.DY = diff.DZ = 0.0;

            //span.Add("p").AddClass("bg-info").Text(string.Format("-Max ABSOLUTE Error: {0:e3}", diff));//htmltags cannot encode the delta and teta chars so will use vector length
            span.Add("p").AddClass("bg-info")
                .Text(string.Format("-Max ABSOLUTE Error: Displacement:{0:e3} , Rotation:{1:e3}",
                    diff.Displacements.Length, diff.Rotations.Length));

            var buf = new ValidationResult();
            buf.Span = span;
            buf.Title = "Rigid element Validation";

            return buf;
        }
    }
}
