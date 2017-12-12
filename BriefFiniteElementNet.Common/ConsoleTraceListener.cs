using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Common
{
    public class ConsoleTraceListener:ITraceListener
    {
        public ReadOnlyCollection<TraceRecord> Records
        {
            get { return new ReadOnlyCollection<TraceRecord>(records); }
        }

        private List<TraceRecord> records=new List<TraceRecord>();


        public void Write(TraceRecord record)
        {
            records.Add(record);

            var bck = Console.ForegroundColor;

            switch (record.Level)
            {
                case TraceLevel.Undefined:
                    break;
                case TraceLevel.Info:
                    Console.ForegroundColor=ConsoleColor.Cyan;
                    break;
                case TraceLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case TraceLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine("{0},{1}", record.IssueId, record.Message);

            Console.ForegroundColor = bck;
        }
    }
}
