using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BriefFiniteElementNet.Validation
{
    public class AbaqusOutputFileReader
    {
        public static List<Tuple<string, Displacement>> ReadFullDisplacements(Stream str)
        {
            var buf = new List<Tuple<string, Displacement>>();

            var rdr = new StreamReader(str);

            string ln;

            while ((ln = rdr.ReadLine()) != null)
            {
                var pat = "^\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)$";
                var mtch = Regex.Match(ln, pat);

                if (mtch.Success)
                {
                    int ttt;

                    var lbl = mtch.Groups[1].Value;
                    var intpt = mtch.Groups[2].Value;

                    var t = mtch.Groups.Cast<Group>().Skip(2).Select(i => i.Value).ToList();

                    double tmp;

                    if (t.Any(i => !double.TryParse(i, out tmp)))
                        continue;//check them all double

                    var dbls = t.Select(i => double.Parse(i)).ToArray();

                    var d = Vector.FromArray(dbls, 0);
                    var r = Vector.FromArray(dbls, 3);

                    var dd = new Displacement(d, r);

                    var tpl = new Tuple<string, Displacement>(lbl, dd);

                    buf.Add(tpl);
                }
            }

            return buf;

            throw new NotImplementedException();
        }

        public static List<Tuple<string,Displacement>> ReadDisplacements(Stream str)
        {
            var buf = new List<Tuple<string, Displacement>>();

            var rdr = new StreamReader(str);

            string ln;

            while ((ln = rdr.ReadLine()) != null)
            {
                var pat = "^\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)$";
                var mtch = Regex.Match(ln, pat);

                if (mtch.Success)
                {
                    int ttt;

                    var lbl = mtch.Groups[1].Value;
                    var intpt = mtch.Groups[2].Value;

                    var t = mtch.Groups.Cast<Group>().Skip(2).Select(i => i.Value).ToList();

                    double tmp;

                    if (t.Any(i => !double.TryParse(i, out tmp)))
                        continue;//check them all double

                    var dbls = t.Select(i => double.Parse(i)).ToArray();

                    var d = Vector.FromArray(dbls);

                    var dd = new Displacement(d, Vector.Zero);

                    var tpl = new Tuple<string, Displacement>(lbl, dd);

                    buf.Add(tpl);
                }
            }

            return buf;

            throw new NotImplementedException();
        }

        public static List<Tuple<string,int,double[]>> ReadElementStresses(Stream str)
        {
            var buf = new List<Tuple<string, int, double[]>>();

            var rdr = new StreamReader(str);

            string ln;

            while ((ln = rdr.ReadLine()) != null)
            {
                var pat = "^\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)\\s+(\\S+)$";
                var mtch = Regex.Match(ln, pat);

                if (mtch.Success)
                {
                    int ttt;
                    
                    var lbl = mtch.Groups[1].Value;
                    var intpt = mtch.Groups[2].Value;

                    var t = mtch.Groups.Cast<Group>().Skip(3).Select(i => i.Value).ToList();

                    double tmp;

                    if (t.Any(i => !double.TryParse(i, out tmp)))
                        continue;//check them all double

                    var dbls = t.Select(i => double.Parse(i)).ToArray();

                    var tpl = new Tuple<string, int, double[]>(mtch.Groups[1].Value, int.Parse(mtch.Groups[2].Value), dbls);

                    buf.Add(tpl);
                }
            }

            return buf;
        }

        private static bool TryParse(string str,ColType type,out object val )
        {
            switch (type)
            {
                case ColType.String:
                    val = str;
                    return true;
                    break;

                case ColType.Int:
                    int tmp;
                    if (int.TryParse(str, out tmp))
                    {
                        val = tmp;
                        return true;
                    }
                    break;

                case ColType.Real:
                    double tmp2;
                    if (double.TryParse(str, out tmp2))
                    {
                        val = tmp2;
                        return true;
                    }
                    break;
            }

            val = null;
            return false;
        }


        public static List<object[]> ReadTable(Stream file,params ColType[] colTypes)
        {
            var pattern = "^" + string.Join("", Enumerable.Repeat("\\s+(\\S+)", colTypes.Length)) + "$";

            var buf = new List<object[]>();

            var rdr = new StreamReader(file);

            string ln;

            while ((ln = rdr.ReadLine()) != null)
            {
                var mtch = Regex.Match(ln, pattern);

                if (mtch.Success)
                {
                    var lst = new List<object>();

                    var flag = false;

                    for (var i = 0; i < colTypes.Length; i++)
                    {
                        var val = mtch.Groups[i + 1].Value;

                        object v;

                        if (!TryParse(val, colTypes[i], out v))
                        {
                            flag = true;
                            break;
                        }
                            

                        lst.Add(v);
                    }

                    if (!flag)
                        buf.Add(lst.ToArray());
                }
            }

            return buf;
        }

        public enum ColType
        {
            String,
            Int,
            Real

        }
    }

    public class AbaqusInputFileReader
    {
        /// <summary>
        /// Method that reads an Abaqus input file and returns a BFE model with the same nodes and elements 
        /// </summary>
        /// <param name="path">Path to an input file</param>
        /// <returns>A BFE model</returns>
        public static Model AbaqusInputToBFE(string pathToInputFile)
        {
            var buf = new Model();

            //splitting char
            char delimiter = ',';
            Node node;
            TetrahedronElement element;

            //list for the node- and elementsets
            List<NodeSet> nodeSets = new List<NodeSet>();
            List<ElementSet> elementSets = new List<ElementSet>();

            using (StreamReader sr = File.OpenText(pathToInputFile))
            {
                string[] split;
                selectedInputVariable selectedVar = selectedInputVariable.Nodes;
                string selectedVarInfo = "";//selectedVar + info

                string input = sr.ReadLine();
                while (input != null)
                {
                    split = input.Split(delimiter);
                    if (split[0].Contains('*'))
                    {
                        switch (split[0])
                        {
                            case "*Node":
                                selectedVar = selectedInputVariable.Nodes;
                                break;
                            case "*Element":
                                selectedVar = selectedInputVariable.Elements;
                                selectedVarInfo = input;
                                break;
                            case "*Nset":
                                selectedVar = selectedInputVariable.NodeSet;
                                nodeSets.Add(new NodeSet() { Name = split[1].Replace("nset=", "") });
                                break;
                            case "*Elset":
                                selectedVar = selectedInputVariable.ElementSet;
                                elementSets.Add(new ElementSet() { Name = split[1].Replace("elset=", "") });
                                break;
                            case "*Cload":
                                selectedVar = selectedInputVariable.CLoad;
                                break;
                            case "*Boundary":
                                selectedVar = selectedInputVariable.BC;
                                break;
                            default:
                                selectedVar = selectedInputVariable.Other;
                                break;
                        }
                    }
                    else
                    {
                        switch (selectedVar)
                        {
                            case selectedInputVariable.Nodes:
                                {
                                    node = ReadNode(input, delimiter);
                                    if (node != null)
                                    {
                                        //tetrahedron element - > fix rotation
                                        node.Constraints = Constraints.RotationFixed;
                                        buf.Nodes.Add(node);
                                    }
                                    break;
                                }
                            case selectedInputVariable.Elements:
                                {
                                    var elm = ReadElement(input, delimiter, buf.Nodes, selectedVarInfo);

                                    //if (elm is TetrahedronElement tet)
                                    //    tet.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.25);

                                    //element = ReadTetraElement(input, delimiter, buf.Nodes);
                                        if (elm != null)
                                    {
                                        //elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.25);
                                        //element.FixNodeOrder();
                                        buf.Elements.Add(elm);
                                    }
                                    break;
                                }
                            case selectedInputVariable.NodeSet:
                                {
                                    for (int i = 0; i < split.Count(); i++)
                                    {
                                        int tmp;

                                        if (int.TryParse(split[i], out tmp))
                                            nodeSets[nodeSets.Count - 1].Nodes.Add(Convert.ToInt32(split[i]));
                                    }
                                    break;
                                }
                            case selectedInputVariable.ElementSet:
                                {
                                    for (int i = 0; i < split.Count(); i++)
                                    {
                                        elementSets[elementSets.Count - 1].Elements.Add(Convert.ToInt32(split[i]));
                                    }
                                    break;
                                }
                            case selectedInputVariable.CLoad:
                                {
                                    NodeSet set = nodeSets.Where(x => x.Name.Replace(" ", "") == split[0].Replace(" ", "")).FirstOrDefault();
                                    if (set != null)
                                    {
                                        //determine the magnitude of the load
                                        var load = new BriefFiniteElementNet.NodalLoad();
                                        var frc = new Force();
                                        if (Convert.ToInt32(split[1]) == 1)
                                        {
                                            frc.Fx = Convert.ToDouble(split[2]);
                                            load.Force = frc;
                                        }
                                        else if (Convert.ToInt32(split[1]) == 2)
                                        {
                                            frc.Fy = Convert.ToDouble(split[2]);
                                            load.Force = frc;
                                        }
                                        else if (Convert.ToInt32(split[1]) == 3)
                                        {
                                            frc.Fz = Convert.ToDouble(split[2]);
                                            load.Force = frc;
                                        }
                                        //add the load to the nodes
                                        foreach (var nodeLabel in set.Nodes)
                                        {
                                            buf.Nodes[nodeLabel - 1].Loads.Add(load.Clone());
                                        }
                                    }
                                    break;
                                }
                            case selectedInputVariable.BC:
                                {
                                    NodeSet set = nodeSets.Where(x => x.Name.Replace(" ", "") == split[0].Replace(" ", "")).FirstOrDefault();
                                    if (set != null)
                                    {                                        
                                        //add the load to the nodes
                                        foreach (var nodeLabel in set.Nodes)
                                        {
                                            buf.Nodes[nodeLabel - 1].Constraints = Constraints.Fixed;
                                        }
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    input = sr.ReadLine();
                }
            }
            return buf;
        }
        /// <summary>
        /// Reading node
        /// </summary>
        /// <param name="nodeline">A line from an Abaqus file</param>
        /// <param name="delimiter">The separator char</param>
        /// <returns>A node for the BFE model</returns>
        private static Node ReadNode(string nodeline, char delimiter)
        {
            //variables needed for later
            string[] split;
            Node node = null;
            double X, Y, Z;
            int nodeNr;

            try
            {
                split = nodeline.Split(delimiter);

                nodeNr = Convert.ToInt32(split[0]);
                X = Convert.ToDouble(split[1]);
                Y = Convert.ToDouble(split[2]);
                Z = Convert.ToDouble(split[3]);
                node = new Node(X, Y, Z) { Label = "n" + nodeNr.ToString() };
            }
            catch (Exception)
            {
                throw new Exception("Something went wrong with reading the nodes! Error with line: " + nodeline);
            }
            return node;
        }


        /// <summary>
        /// Reads an element from an Abaqus input file
        /// </summary>
        /// <param name="elementline">A line with the element props</param>
        /// <param name="delimiter">The separator char</param>
        /// <param name="nodes">A list of nodes - starts at 0 -> = node-1</param>
        /// <returns>An element</returns>
        private static HexahedralElement ReadHexaElement(string elementline, char delimiter, NodeCollection nodes)
        {
            var elm = new HexahedralElement();
            string[] split;
            int elementNr;//, nodeNr1, nodeNr2, nodeNr3, nodeNr4;
            try
            {
                var spl = elementline.Split(delimiter).Select(int.Parse).ToArray();

                var elmNum = spl[0];//element number

                var nodeIdxs = spl.Skip(1).ToArray();//index of connected nodes


                //subtract 1. The nodes are numbered from 1-> end and are stored as 0-> end-1

                for (var i = 0; i < 8; i++)
                {
                    elm.Nodes[i] = nodes[nodeIdxs[i ] - 1];
                }
                
                elm.label = elmNum.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong with reading the nodes! Error with line: " + elementline, ex);
            }

            return elm;
        }


        private static Element ReadElement(string elementline, char delimiter, NodeCollection nodes,string elementInfo)
        {
            var patt = @"^\*Element, type=(\S+)$";

            var mtch = Regex.Match(elementInfo, patt);

            if (!mtch.Success)
                throw new Exception();

            var elmType = mtch.Groups[1].Value;

            switch(elmType)
            {
                case "C3D8":
                    return ReadHexaElement(elementline, delimiter, nodes);
                case "C3D4":
                    return ReadTetraElement(elementline, delimiter, nodes);
                case "STRI3":
                    return ReadTriangleElement(elementline, delimiter, nodes);

                default:
                    throw new NotImplementedException("undefined ABAQUS element: " + elmType);
            }
        }

        /// <summary>
        /// Reads an element from an Abaqus input file
        /// </summary>
        /// <param name="elementline">A line with the element props</param>
        /// <param name="delimiter">The separator char</param>
        /// <param name="nodes">A list of nodes - starts at 0 -> = node-1</param>
        /// <returns>An element</returns>
        private static TriangleElement ReadTriangleElement(string elementline, char delimiter, NodeCollection nodes)
        {
            var elm = new TriangleElement();
            string[] split;
            int elementNr;//, nodeNr1, nodeNr2, nodeNr3, nodeNr4;
            try
            {
                split = elementline.Split(delimiter);

                elementNr = Convert.ToInt32(split[0]);
                //subtract 1. The nodes are numbered from 1-> end and are stored as 0-> end-1
                elm.Nodes[0] = nodes[Convert.ToInt32(split[1]) - 1];
                elm.Nodes[1] = nodes[Convert.ToInt32(split[2]) - 1];
                elm.Nodes[2] = nodes[Convert.ToInt32(split[3]) - 1];
                elm.label = elementNr.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong with reading the nodes! Error with line: " + elementline);
            }

            return elm;
        }

        /// <summary>
        /// Reads an element from an Abaqus input file
        /// </summary>
        /// <param name="elementline">A line with the element props</param>
        /// <param name="delimiter">The separator char</param>
        /// <param name="nodes">A list of nodes - starts at 0 -> = node-1</param>
        /// <returns>An element</returns>
        private static TetrahedronElement ReadTetraElement(string elementline, char delimiter, NodeCollection nodes)
        {
            var elm = new TetrahedronElement();
            string[] split;
            int elementNr;//, nodeNr1, nodeNr2, nodeNr3, nodeNr4;
            try
            {
                split = elementline.Split(delimiter);

                elementNr = Convert.ToInt32(split[0]);
                //subtract 1. The nodes are numbered from 1-> end and are stored as 0-> end-1
                elm.Nodes[0] = nodes[Convert.ToInt32(split[1]) - 1];
                elm.Nodes[1] = nodes[Convert.ToInt32(split[2]) - 1];
                elm.Nodes[2] = nodes[Convert.ToInt32(split[3]) - 1];
                elm.Nodes[3] = nodes[Convert.ToInt32(split[4]) - 1];
                elm.label = elementNr.ToString();
            }
            catch (Exception)
            {
                throw new Exception("Something went wrong with reading the nodes! Error with line: " + elementline);
            }

            return elm;
        }
    }


    /// <summary>
    /// A nodeset
    /// </summary>
    public class NodeSet
    {
        /// <summary>
        /// Name of the set
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of node labels
        /// </summary>
        public List<int> Nodes { get; set; } = new List<int>();
    }
    /// <summary>
    /// Element set
    /// </summary>
    public class ElementSet
    {
        /// <summary>
        /// Name of the set
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of element labels
        /// </summary>
        public List<int> Elements { get; set; } = new List<int>();
    }

    /// <summary>
    /// Enum for reading data
    /// </summary>
    public enum selectedInputVariable
    {
        Nodes = 1,
        Elements = 2,
        NodeSet = 3,
        ElementSet = 4,
        CLoad = 5,
        BC = 6,
        Other = 7,
    }
}
