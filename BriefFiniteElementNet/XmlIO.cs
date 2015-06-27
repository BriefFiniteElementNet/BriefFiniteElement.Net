using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet
{
    public static class XmlIO
    {
        public static Model ReadFromXml(Stream stream)
        {
            var doc = new XmlDocument();
            doc.Load(stream);

            var buf = new Model();

            var nodesList = new List<Node>();
            var nodesDictionary = new Dictionary<string, Node>();

            #region nodes

            var nodes = doc.SelectNodes("Model/Nodes/Node");

            if (nodes == null)
                throw new Exception("Model/Nodes/Node element not found");

            foreach (XmlElement node in nodes)
            {
                var x = node.GetAttributeValue<double>("X");
                var y = node.GetAttributeValue<double>("Y");
                var z = node.GetAttributeValue<double>("Z");

                #region fixity

                var dxFix = node.GetAttributeValue<bool>("DxFix");
                var dyFix = node.GetAttributeValue<bool>("DyFix");
                var dzFix = node.GetAttributeValue<bool>("DzFix");
                var rxFix = node.GetAttributeValue<bool>("RxFix");
                var ryFix = node.GetAttributeValue<bool>("RyFix");
                var rzFix = node.GetAttributeValue<bool>("RzFix");

                var lbl = node.GetAttributeValue("Label");

                var nde = new Node(x.Value, y.Value, z.Value);

                var cns = Constraint.Released;

                if (dxFix.HasValue)
                    cns.DX = dxFix.Value ? DofConstraint.Fixed : DofConstraint.Released;

                if (dyFix.HasValue)
                    cns.DY = dyFix.Value ? DofConstraint.Fixed : DofConstraint.Released;

                if (dzFix.HasValue)
                    cns.DZ = dzFix.Value ? DofConstraint.Fixed : DofConstraint.Released;

                if (rxFix.HasValue)
                    cns.RX = rxFix.Value ? DofConstraint.Fixed : DofConstraint.Released;

                if (ryFix.HasValue)
                    cns.RY = ryFix.Value ? DofConstraint.Fixed : DofConstraint.Released;

                if (rzFix.HasValue)
                    cns.RZ = rzFix.Value ? DofConstraint.Fixed : DofConstraint.Released;

                nde.Constraints = cns;

                #endregion

                #region settlement

                var dxSl = node.GetAttributeValue<double>("DxStl");
                var dySl = node.GetAttributeValue<double>("DxStl");
                var dzSl = node.GetAttributeValue<double>("DxStl");
                var rxSl = node.GetAttributeValue<double>("DxStl");
                var rySl = node.GetAttributeValue<double>("DxStl");
                var rzSl = node.GetAttributeValue<double>("DxStl");

                var settl = new Displacement();

                if (dxSl.HasValue)
                    settl.DX = dxSl.Value;

                if (dySl.HasValue)
                    settl.DY = dySl.Value;

                if (dzSl.HasValue)
                    settl.DZ = dzSl.Value;

                if (rxSl.HasValue)
                    settl.RX = rxSl.Value;

                if (rySl.HasValue)
                    settl.RY = rySl.Value;

                if (rzSl.HasValue)
                    settl.RZ = rzSl.Value;


                nde.Settlements = settl;

                #endregion

                var tag = node.GetAttributeValue("Tag");

                if (tag != null)
                    nde.Tag = tag;


                if (lbl != null)
                {
                    nde.Label = lbl;
                    if (nodesDictionary.ContainsKey(lbl))
                        throw new Exception("duplicated label for nodes: " + lbl);
                    nodesDictionary[lbl] = nde;
                }

                nodesList.Add(nde);
            }

            #endregion

            var elementList = new List<Element>();
            var elementDictionary = new Dictionary<string, Element>();

            var frmElms = doc.SelectNodes("Model/Elements/FrameElement2Node");

            #region frame elements

            if (frmElms != null)
                foreach (XmlElement node in frmElms)
                {
                    var elm = new FrameElement2Node();

                    var n1Index = node.GetAttributeValue<int>("Node1Index");
                    var n2Index = node.GetAttributeValue<int>("Node2Index");

                    var n1Label = node.GetAttributeValue("Node1Label");
                    var n2Label = node.GetAttributeValue("Node2Label");

                    var iy = node.GetAttributeValue<double>("Iy");
                    var iz = node.GetAttributeValue<double>("Iz");
                    var j = node.GetAttributeValue<double>("J");
                    var a = node.GetAttributeValue<double>("A");
                    var ay = node.GetAttributeValue<double>("Ay");
                    var az = node.GetAttributeValue<double>("Az");
                    var density = node.GetAttributeValue<double>("Density");
                    var hing1 = node.GetAttributeValue<bool>("HingedAtStart");
                    var hing2 = node.GetAttributeValue<bool>("HingedAtEnd");
                    var webRot = node.GetAttributeValue<double>("WebRoation");

                    var label = node.GetAttributeValue("Label");
                    var tag = node.GetAttributeValue("Tag");


                    var shearEffect = node.GetAttributeValue<bool>("ConsiderShearEffect");

                    if (n1Index == null && n1Label == null)
                        throw new Exception("Neither Node1Index and Node1Label are defined");

                    if (n2Index == null && n2Label == null)
                        throw new Exception("Neither Node2Index and Node2Label are defined");

                    elm.Iy = iy.Value;
                    elm.Iz = iz.Value;
                    elm.J = j.Value;
                    elm.A = a.Value;

                    if (label != null)
                    {
                        elm.Label = label;
                        elementDictionary[label] = elm;
                    }

                    if (tag != null)
                        elm.Tag = tag;

                    if (ay.HasValue)
                        elm.Ay = ay.Value;

                    if (az.HasValue)
                        elm.Az = az.Value;

                    if (hing1.HasValue)
                        elm.HingedAtStart = hing1.Value;

                    if (hing2.HasValue)
                        elm.HingedAtEnd = hing2.Value;

                    elm.MassDensity = density.Value;

                    elm.UseOverridedProperties = true;

                    if (shearEffect.HasValue)
                        elm.ConsiderShearDeformation = shearEffect.Value;

                    if (webRot.HasValue)
                        elm.WebRotation = webRot.Value;


                    if (n1Index.HasValue)
                    {
                        elm.StartNode = nodesList[n1Index.Value];
                    }
                    else
                    {
                        if (n1Label == null)
                            throw new Exception();

                        if (!nodesDictionary.ContainsKey(n1Label))
                            throw new Exception("node not exists, label: " + n1Label);

                        elm.StartNode = nodesDictionary[n1Label];
                    }


                    if (n2Index.HasValue)
                    {
                        elm.EndNode = nodesList[n2Index.Value];
                    }
                    else
                    {
                        if (n2Label == null)
                            throw new Exception();

                        if (!nodesDictionary.ContainsKey(n2Label))
                            throw new Exception("node not exists, label: " + n1Label);

                        elm.EndNode = nodesDictionary[n2Label];
                    }

                    elementList.Add(elm);


                }

            #endregion

            var rgdElms = doc.SelectNodes("Model/Elements/RigidElement");

            #region rigid elements

            foreach (XmlElement node in rgdElms)
            {
                var rElm = new RigidElement();

                var innerNodes = node.SelectNodes("/Node");

                var label = node.GetAttributeValue("Label");

                if (label != null)
                    rElm.Label = label;

                var tag = node.GetAttributeValue("Tag");

                if (tag != null)
                    rElm.Tag = tag;


                foreach (XmlElement innerNode in innerNodes)
                {
                    var num = innerNode.GetAttributeValue<int>("Index");
                    var lbl = innerNode.GetAttributeValue("Label");
                    var isCenter = innerNode.GetAttributeValue<bool>("IsCenter");

                    if (lbl == null && !num.HasValue)
                        continue;

                    Node targetNode;

                    if (num.HasValue)
                    {
                        rElm.Nodes.Add(targetNode = nodesList[num.Value]);
                    }
                    else
                    {
                        if (!nodesDictionary.ContainsKey(lbl))
                            throw new Exception("Invalid node label: " + lbl);

                        rElm.Nodes.Add(targetNode = nodesDictionary[lbl]);
                    }


                    if (isCenter.HasValue)
                    {
                        if (isCenter.Value)
                        {
                            rElm.CentralNode = targetNode;
                        }
                    }


                }

                var useForAllLoads = node.GetAttributeValue<bool>("UseForAllLoads");

                if (useForAllLoads.HasValue)
                    rElm.UseForAllLoads = useForAllLoads.Value;


                var appliedLoadCases = node.SelectNodes("RigidElement/AppliedLoadCases/LoadCase");

                #region cases

                foreach (XmlElement item in appliedLoadCases)
                {
                    var name = item.GetAttributeValue("Name");
                    var type = item.GetAttributeValue("Type");

                    if (name == null || type == null)
                        throw new Exception("RigidElement/AppliedLoadCases/LoadCase should have both Type and Title");

                    LoadType loadTp;

                    if (type.IsNumber())
                    {
                        loadTp = (LoadType) int.Parse(type);
                    }
                    else
                    {
                        loadTp = (LoadType) Enum.Parse(typeof (LoadType), type);
                    }


                    var lcase = new LoadCase(name, loadTp);
                    rElm.AppliedLoadCases.Add(lcase);
                }

                #endregion


                var appliedLoadTypes = node.SelectNodes("RigidElement/AppliedLoadTypes/LoadType");

                #region types

                foreach (XmlElement item in appliedLoadTypes)
                {
                    var type = item.GetAttributeValue("Type");

                    if (type == null)
                        throw new Exception("RigidElement/AppliedLoadCases/LoadCase should have both Type and Title");

                    LoadType loadTp;

                    if (type.IsNumber())
                    {
                        loadTp = (LoadType) int.Parse(type);
                    }
                    else
                    {
                        loadTp = (LoadType) Enum.Parse(typeof (LoadType), type);
                    }


                    rElm.AppliedLoadTypes.Add(loadTp);
                }

                #endregion

            }



            #endregion

            #region loads

            {
//reading loads

                var loadSets = doc.SelectNodes("Model/LoadSet");

                foreach (XmlElement xelm in loadSets)
                {
                    var loadCaseType = xelm.GetAttributeValue("LoadCaseType");
                    var loadCaseName = xelm.GetAttributeValue("LoadCaseName");


                    var loads = xelm.SelectNodes("/");

                    foreach (XmlElement loadElm in loads)
                    {
                        if (loadElm.Name == "NodalLoad")
                        {
                            #region nodal load

                            var nodeIndex = xelm.GetAttributeValue<int>("NodeIndex");
                            var nodeLabel = xelm.GetAttributeValue("NodeLabel");

                            var fx = xelm.GetAttributeValue<double>("Fx");
                            var fy = xelm.GetAttributeValue<double>("Fy");
                            var fz = xelm.GetAttributeValue<double>("Fz");
                            var mx = xelm.GetAttributeValue<double>("Mx");
                            var my = xelm.GetAttributeValue<double>("My");
                            var mz = xelm.GetAttributeValue<double>("Mz");

                            var load = new NodalLoad();

                            var force = new Force();

                            force.Fx = fx ?? 0;
                            force.Fy = fy ?? 0;
                            force.Fz = fz ?? 0;
                            force.Mx = mx ?? 0;
                            force.My = my ?? 0;
                            force.Mz = mz ?? 0;

                            load.Case = new LoadCase(loadCaseName, GetLoadType(loadCaseType));
                            load.Force = force;

                            if (nodeIndex.HasValue)
                            {
                                nodesList[nodeIndex.Value].Loads.Add(load);
                            }
                            else if (nodeLabel != null)
                            {
                                nodesDictionary[nodeLabel].Loads.Add(load);
                            }
                            else
                                throw new Exception("neither node index and label are defined");

                            #endregion
                        }

                        else if (loadElm.Name == "UniformLoad")
                        {
                            #region uniform load

                            var elmIndex = xelm.GetAttributeValue<int>("NodeIndex");
                            var elmLabel = xelm.GetAttributeValue("NodeLabel");

                            var magnitude = xelm.GetAttributeValue<double>("Magnitude");
                            var dir = xelm.GetAttributeValue("Direction");
                            var sys = xelm.GetAttributeValue("CoordinationSystem");

                            var load = new UniformLoad1D();

                            load.Magnitude = magnitude ?? magnitude.Value;

                            load.Case = new LoadCase(loadCaseName, GetLoadType(loadCaseType));
                            load.Direction = (LoadDirection) Enum.Parse(typeof (LoadDirection), dir);
                            load.CoordinationSystem = (CoordinationSystem) Enum.Parse(typeof (CoordinationSystem), sys);

                            if (elmIndex.HasValue)
                            {
                                elementList[elmIndex.Value].Loads.Add(load);
                            }
                            else if (elmLabel != null)
                            {
                                elementDictionary[elmLabel].Loads.Add(load);
                            }
                            else
                                throw new Exception("neither node index and label are defined");

                            #endregion
                        }

                        else
                        {
                            throw new Exception(string.Format("Element Type {0} not supported!", loadElm.Name));
                        }
                    }

                }
            }

            #endregion


            throw new NotImplementedException();
        }









        private static LoadType GetLoadType(string loadType)
        {
            LoadType loadTp;

            if (loadType.IsNumber())
            {
                loadTp = (LoadType) int.Parse(loadType);
            }
            else
            {
                loadTp = (LoadType) Enum.Parse(typeof (LoadType), loadType);
            }

            return loadTp;
        }

        private static T? GetAttributeValue<T>(this XmlElement elm, string attribute) where T : struct
        {
            if (!elm.HasAttribute(attribute))
                return new T?();

            if (typeof (T) == typeof (double))
            {
                var val = (T) (ValueType) double.Parse(elm.Attributes[attribute].Value);
                return new T?(val);
            }

            if (typeof (T) == typeof (int))
            {
                var val = (T) (ValueType) int.Parse(elm.Attributes[attribute].Value);
                return new T?(val);
            }

            if (typeof (T) == typeof (bool))
            {
                var val = (T) (ValueType) bool.Parse(elm.Attributes[attribute].Value);
                return new T?(val);
            }

            throw new NotImplementedException();
        }

        private static string GetAttributeValue(this XmlElement elm, string attribute)
        {
            if (!elm.HasAttribute(attribute))
                return null;

            return elm.Attributes[attribute].Value;
        }

        private static bool IsNumber(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            int t;

            return int.TryParse(str, out t);
        }


        //write to xml

        public static string WriteToXml(Model model)
        {
            var sb = new StringBuilder();

            var w = new XmlTextWriter(new StringWriter(sb));

            for (var i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;


            w.WriteStartElement("Model");

            #region nodes

            w.WriteStartElement("Nodes");

            foreach (var nde in model.Nodes)
            {
                w.WriteStartElement("Node");
                w.WriteAttributeString("X", nde.Location.X.ToString());
                w.WriteAttributeString("Y", nde.Location.Y.ToString());
                w.WriteAttributeString("Z", nde.Location.Z.ToString());

                if (nde.Constraints.DX == DofConstraint.Fixed) w.WriteAttributeString("DxFix", "true");
                if (nde.Constraints.DY == DofConstraint.Fixed) w.WriteAttributeString("DyFix", "true");
                if (nde.Constraints.DZ == DofConstraint.Fixed) w.WriteAttributeString("DzFix", "true");

                if (nde.Constraints.RX == DofConstraint.Fixed) w.WriteAttributeString("RxFix", "true");
                if (nde.Constraints.RY == DofConstraint.Fixed) w.WriteAttributeString("RyFix", "true");
                if (nde.Constraints.RZ == DofConstraint.Fixed) w.WriteAttributeString("RzFix", "true");


                if (!nde.Settlements.DX.Equals(0)) w.WriteAttributeString("DxStl", nde.Settlements.DX.ToString());
                if (!nde.Settlements.DY.Equals(0)) w.WriteAttributeString("DyStl", nde.Settlements.DY.ToString());
                if (!nde.Settlements.DZ.Equals(0)) w.WriteAttributeString("DzStl", nde.Settlements.DZ.ToString());
                if (!nde.Settlements.RX.Equals(0)) w.WriteAttributeString("RxStl", nde.Settlements.RX.ToString());
                if (!nde.Settlements.RY.Equals(0)) w.WriteAttributeString("RyStl", nde.Settlements.RY.ToString());
                if (!nde.Settlements.RZ.Equals(0)) w.WriteAttributeString("RzStl", nde.Settlements.RZ.ToString());

                if (nde.Label != null) w.WriteAttributeString("Label", nde.Label);
                if (nde.Tag != null) w.WriteAttributeString("Tag", nde.Tag);

                w.WriteEndElement();
            }

            w.WriteEndElement();

            #endregion

            #region elements

            w.WriteStartElement("Elements");

            foreach (var elm in model.Elements)
            {
                switch (elm.ElementType)
                {
                    case ElementType.Undefined:
                        break;
                    case ElementType.FrameElement2Node:
                        WriteFrameElement(w, elm as FrameElement2Node);
                        break;
                    case ElementType.TrussElement2Noded:
                        WriteTrussElement(w, elm as TrussElement2Node);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            foreach (var rigidElement in model.RigidElements)
            {
                WriteRigidElement(w, rigidElement);
            }

            w.WriteEndElement();

            #endregion


            //w.WriteEndElement();


            var buf = sb.ToString();

            return buf;
        }

        private static void WriteFrameElement(XmlWriter w, FrameElement2Node elm)
        {
            w.WriteStartElement("FrameElement2Node");

            w.WriteAttributeString("Node1Index", elm.StartNode.Index.ToString());
            w.WriteAttributeString("Node2Index", elm.EndNode.Index.ToString());


            w.WriteAttributeString("Iy", elm.Iy.ToString());
            w.WriteAttributeString("Iz", elm.Iz.ToString());
            w.WriteAttributeString("J", elm.J.ToString());
            w.WriteAttributeString("A", elm.A.ToString());
            w.WriteAttributeString("Ay", elm.Ay.ToString());
            w.WriteAttributeString("Az", elm.Az.ToString());
            w.WriteAttributeString("Density", elm.MassDensity.ToString());

            if (elm.HingedAtStart) w.WriteAttributeString("HingedAtStart", "true");
            if (elm.HingedAtEnd) w.WriteAttributeString("HingedAtEnd", "true");
            if (elm.ConsiderShearDeformation) w.WriteAttributeString("ConsiderShearEffect", "true");
            if (!elm.WebRotation.Equals(0)) w.WriteAttributeString("WebRoation", elm.WebRotation.ToString());

            w.WriteEndElement();

        }

        private static void WriteTrussElement(XmlWriter w, TrussElement2Node elm)
        {
            w.WriteStartElement("TrussElement2Node");

            w.WriteAttributeString("Node1Index", elm.StartNode.Index.ToString());
            w.WriteAttributeString("Node2Index", elm.EndNode.Index.ToString());

            w.WriteAttributeString("A", elm.A.ToString());
            w.WriteAttributeString("Density", elm.MassDensity.ToString());

            w.WriteEndElement();

        }

        private static void WriteRigidElement(XmlWriter w, RigidElement elm)
        {
            w.WriteStartElement("RigidElement");

            if (elm.Label != null)
                w.WriteAttributeString("Label", elm.Label);

            if (elm.UseForAllLoads)
                w.WriteAttributeString("UseForAllLoads", "true");

            foreach (var nde in elm.Nodes)
            {
                w.WriteStartElement("Node");
                w.WriteAttributeString("Index", nde.Index.ToString());

                if (ReferenceEquals(nde, elm.CentralNode))
                    w.WriteAttributeString("IsCenter", "true");

                w.WriteEndElement();
            }

            w.WriteEndElement();

            if (elm.AppliedLoadCases.Any())
            {
                w.WriteStartElement("AppliedLoadCases");

                foreach (var cse in elm.AppliedLoadCases)
                {
                    w.WriteStartElement("LoadCase");

                    w.WriteAttributeString("Type", cse.LoadType.ToString());
                    w.WriteAttributeString("Name", cse.CaseName);
                    w.WriteEndElement();
                }

                w.WriteEndElement();
            }

            if (elm.AppliedLoadTypes.Any())
            {
                w.WriteStartElement("AppliedLoadTypes");

                foreach (var tp in elm.AppliedLoadTypes)
                {
                    w.WriteStartElement("LoadType");

                    w.WriteAttributeString("Type", tp.ToString());
                    w.WriteEndElement();
                }

                w.WriteEndElement();
            }

            w.WriteEndElement();
        }
    }

}