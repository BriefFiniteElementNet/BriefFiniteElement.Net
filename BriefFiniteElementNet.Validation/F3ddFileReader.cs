using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    /// <summary>
    /// represents a utility class for reading frame3dd output file
    /// </summary>
    public class F3ddFileReader
    {
        public string FileName { get; set; }

        public F3ddFileReader(string fileName)
        {
            FileName = fileName;
        }

        public Dictionary<int,Displacement> GetNodalDisplacements()
        {
            var allLines = System.IO.File.ReadAllLines(FileName);

            var idx =
                Enumerable.Range(0, allLines.Length)
                    .First(i => allLines[i].Contains("N O D E   D I S P L A C E M E N T S"));

            idx+=2;

            var buf = new Dictionary<int, Displacement>();

            while (true)
            {
                var ln = RemoveExtraSpaces(allLines[idx++]).Trim();

                var sp = ln.Split(' ');

                if (!char.IsNumber(sp[0][0]))
                    break;

                var nodeIndex = sp[0].ToInt();

                var disps = sp.Skip(1).Select(i => i.ToDouble()).ToList();

                var disp = new Displacement();
                
                disp.DX = disps[0];
                disp.DY = disps[1];
                disp.DZ = disps[2];

                disp.RX = disps[3];
                disp.RY = disps[4];
                disp.RZ = disps[5];

                buf[nodeIndex - 1] = disp;
                //buf.Add(new KeyValuePair<int, Displacement>(nodeIndex - 1, disp));
            }

            return buf;
        }

        public static string RemoveExtraSpaces(string str)
        {
            while (str != str.Replace("  ", " "))
            {
                str = str.Replace("  ", " ");
            }

            return str;
        }
    }
}
