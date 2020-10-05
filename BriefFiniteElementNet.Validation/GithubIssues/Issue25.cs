using BriefFiniteElementNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue25
    {
        //FrameElement2Node is obsolete
        //public static void Run()
        //{
        //    //runs (but results seem strange)
        //    PI(13);
        //    //Throws exception
        //    PI(15);
        //}

        //private static void PI(double canteleverDist)
        //{
        //    var model = new Model();

        //    Node Leg1Base = null;
        //    Node Leg1Tip = null;
        //    Node Leg2Base = null;
        //    Node Leg2Tip = null;

        //    for (int l = 1; l < 3; l++)
        //    {
        //        int x = -100 + (l - 1) * 200;
        //        Node LegBase = new Node(x, 0, 0) { Label = "Leg" + l.ToString() + "Base" };
        //        LegBase.Constraints = Constraints.Fixed;
        //        model.Nodes.Add(LegBase);
        //        var LegTip = new Node(x, 0, 400) { Label = "Leg" + l.ToString() + "Tip" };
        //        model.Nodes.Add(LegTip);
        //        var beam = MakeBeam(LegBase, LegTip, "Leg" + l.ToString());
        //        model.Elements.Add(beam);

        //        var n = new Node(x, canteleverDist, 400) { Label = "T" + l.ToString() };
        //        model.Nodes.Add(n);
        //        var offsetArm = MakeBeam(LegTip, n, "Offset" + l.ToString());
        //        model.Elements.Add(offsetArm);

        //        offsetArm.HingedAtEnd = true;

        //        LegTip = n;

        //        if (l < 2)
        //        {
        //            Leg1Base = LegBase;
        //            Leg1Tip = LegTip;
        //        }
        //        else
        //        {
        //            Leg2Base = LegBase;
        //            Leg2Tip = LegTip;
        //        }
        //    }


        //    var TopOverhangL = new Node(-150, 5, 350) { Label = "TopOverhang1" };
        //    var TopOverhangR = new Node(150, 5, 350) { Label = "TopOverhang2" };
        //    model.Nodes.Add(TopOverhangL);
        //    model.Nodes.Add(TopOverhangR);

        //    var TopCant1 = MakeBeam(TopOverhangL, Leg1Tip, "TopCant1");
        //    var TopBetweenLegs = MakeBeam(Leg1Tip, Leg2Tip, "TopBetweenLegs");
        //    var TopCant2 = MakeBeam(Leg2Tip, TopOverhangR, "TopCant2");
        //    model.Elements.Add(TopCant1);
        //    model.Elements.Add(TopBetweenLegs);
        //    model.Elements.Add(TopCant2);

        //    var force = new Force(100, 10, 10, 0, 0, 0);
        //    TopOverhangL.Loads.Add(new NodalLoad(force));

        //    force = new Force(-100, -10, -10, 0, 0, 0);
        //    TopOverhangR.Loads.Add(new NodalLoad(force));

        //    model.Trace.Listeners.Add(new ConsoleTraceListener());

        //    PosdefChecker.CheckModel(model, LoadCase.DefaultLoadCase);

        //    model.Solve();

        //    //var wnd = new Window();
        //    //var ctrl = new ModelVisualizerControl();
        //    //ctrl.ModelToVisualize = model;
        //    //wnd.Content = ctrl;
        //    //wnd.ShowDialog();





        //    var d = Leg1Tip.GetNodalDisplacement();
        //    Console.WriteLine("displacement on leg1 X: " + d.DX.ToString() + " Y: " + d.DY.ToString() + " Z: " + d.DZ.ToString());
        //    d = Leg2Tip.GetNodalDisplacement();
        //    Console.WriteLine("displacement on leg2 X: " + d.DX.ToString() + " Y: " + d.DY.ToString() + " Z: " + d.DZ.ToString());

        //}

        //FrameElement2Node is obsolete
        //private static FrameElement2Node MakeBeam(Node pN1, Node pN2, string pName)
        //{
        //    var frameBeam = new FrameElement2Node(pN1, pN2);
        //    frameBeam.Label = pName;
        //    frameBeam.A = 116;
        //    frameBeam.Ay = 105;
        //    frameBeam.Az = 105;
        //    frameBeam.J = 408;
        //    frameBeam.Iy = 1080;
        //    frameBeam.Iz = 1080;
        //    frameBeam.E = 1660;
        //    frameBeam.G = 0.620;
        //    frameBeam.MassDensity = 0.3;
        //    frameBeam.MassFormulationType = MassFormulation.Consistent;
        //    return frameBeam;
        //}

    }
}
