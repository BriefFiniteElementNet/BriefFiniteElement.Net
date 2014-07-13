using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Example1();
        }

        private static void Example1()
        {
            Console.WriteLine("Example 1: Simple 3D truss with four members");


            // Initiating Model, Nodes and Members
            var model = new Model();

            var n1 = new Node(1, 1, 0);
            n1.Label = "n1";//Set a unique label for node
            var n2 = new Node(-1, 1, 0) {Label = "n2"};//using object initializer for assigning Label
            var n3 = new Node(1, -1, 0) {Label = "n3"};
            var n4 = new Node(-1, -1, 0) {Label = "n4"};
            var n5 = new Node(0, 0, 1) {Label = "n5"};

            var e1 = new TrussElement(n1, n5) {Label = "e1"};
            var e2 = new TrussElement(n2, n5) {Label = "e2"};
            var e3 = new TrussElement(n3, n5) {Label = "e3"};
            var e4 = new TrussElement(n4, n5) {Label = "e4"};
            //Note: labels for all members should be unique, else you will receive InvalidLabelException when adding it to model

            e1.A = e2.A = e3.A = e4.A = 9e-4;
            e1.E = e2.E = e3.E = e4.E = 210e9;

            model.Nodes.AddRange(n1, n2, n3, n4, n5);
            model.Elements.AddRange(e1, e2, e3, e4);

            //Aplying restrains


            n1.Constraints = n2.Constraints = n3.Constraints = n4.Constraints = Constraint.Fixed;
            n5.Constraints = Constraint.RotationFixed;


            //Applying load
            var force = new Force(0, 0, -1000, 0, 0, 0);
            n5.Loads.Add(new NodalLoad(force));//adds a load whith LoadCase of DefaultLoadCase to node loads
            
            //Adds a NodalLoad with Default LoadCase

            model.Solve();

            var r1 = n1.GetSupportReaction();
            var r2 = n2.GetSupportReaction();
            var r3 = n3.GetSupportReaction();
            var r4 = n4.GetSupportReaction();

            var rt = r1 + r2 + r3 + r4;//shows the Fz=1000 and Fx=Fy=Mx=My=Mz=0.0

            
        }
    }
}
