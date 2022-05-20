using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace BriefFiniteElementNet.BenchmarkApplication
{
    public class TextLogger
    {
        public StringBuilder Sb = new StringBuilder();

        public bool WriteToConsole = true;

        public string GetAllLog()
        {
            return Sb.ToString();
        }

        public void Log(string format, params object[] pars)
        {
            Sb.AppendLine(string.Format(CultureInfo.CurrentCulture, format, pars));

            if(WriteToConsole)
                Console.WriteLine(format, pars);
        }
    }
}
