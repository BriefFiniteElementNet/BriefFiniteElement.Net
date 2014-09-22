using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a distinct trace for each object!
    /// </summary>
    public class Trace
    {
        /// <summary>
        /// Writes the specified record to all listeners.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(TraceRecord record)
        {
            foreach (var listener in _listeners)
            {
                if (listener != null)
                    listener.Write(record.Clone());
            }
        }

        private List<ITraceListener> _listeners = new List<ITraceListener>();

        /// <summary>
        /// Gets the listeners of this trace object.
        /// </summary>
        /// <value>
        /// The listeners.
        /// </value>
        public List<ITraceListener> Listeners
        {
            get { return _listeners; }
        }
    }

    /// <summary>
    /// Represents a base class for trace listeners!
    /// </summary>
    public interface ITraceListener
    {
        /// <summary>
        /// Writes the specified record into output.
        /// </summary>
        /// <param name="record">The record.</param>
        void Write(TraceRecord record);
    }

    /// <summary>
    /// Represents an enum for type of each rtace record
    /// </summary>
    public enum TraceLevel
    {
        /// <summary>
        /// The undefined
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The information
        /// </summary>
        Info = 1,

        /// <summary>
        /// The warning
        /// </summary>
        Warning = 2,

        /// <summary>
        /// The errorNumber
        /// </summary>
        Error = 3
    }

    /// <summary>
    /// Represents a type for a trace record!
    /// </summary>
    public class TraceRecord
    {
        #region Fields

        private int _number;
        private string _category;
        private string _message;
        private string _detailedMessage;
        private DateTime _timeStamp;
        private string _solution;
        private string _errorId;
        private TraceLevel _level;
        private string _helpLink;

        #endregion

        #region Properties

        [Obsolete]
        public int Number
        {
            get { return _number; }
        }

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        [Obsolete]
        public string DetailedMessage
        {
            get { return _detailedMessage; }
            set { _detailedMessage = value; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        [Obsolete]
        public string Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

        public string ErrorId
        {
            get { return _errorId; }
            set { _errorId = value; }
        }

        public TraceLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public string HelpLink
        {
            get { return _helpLink; }
            set { _helpLink = value; }
        }

        public string TargetIdentifier
        {
            get { return _targetIdentifier; }
            set { _targetIdentifier = value; }
        }


        private string _targetIdentifier;

        #endregion

        /// <summary>
        /// Creates a shallow copy of the current <see cref="TraceRecord"/>
        /// </summary>
        /// <returns></returns>
        public TraceRecord Clone()
        {
            var buf = this.MemberwiseClone();
            return buf as TraceRecord;
        }

        public static TraceRecord Create(TraceLevel level, string message)
        {
            var buf = new TraceRecord();
            
            buf.TimeStamp = DateTime.Now;
            buf.Message = message;
            buf.Level = level;

            return buf;
        }
    }

    /// <summary>
    /// Represents ready records for well known situations
    /// </summary>
    public static class TraceRecords
    {
        /// <summary>
        /// Gets the ready filled record.
        /// </summary>
        /// <param name="errorNumber">The error Number.</param>
        /// <param name="elementIdentifier">The element identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static TraceRecord GetRecord(int errorNumber, string elementIdentifier)
        {
            var rec = new TraceRecord();
            rec.TimeStamp = DateTime.Now;

            rec.HelpLink = @"https://brieffiniteelementnet.codeplex.com/wikipage?title=Error%20message%20list"
                           + "#MA" + errorNumber;

            rec.TargetIdentifier = elementIdentifier;

            switch (errorNumber)
            {
                case 10000:
                    rec.ErrorId = "MA10000";
                    rec.Category = "Elements";
                    rec.Level = TraceLevel.Warning;
                    rec.Message = "FrameElement2Node.Geometry & FrameElement2Node.UseOverridedProperties values are inconsistent";
                    break;
                case 10010:
                    rec.ErrorId = "MA10010";
                    rec.Category = "Elements";
                    rec.Level = TraceLevel.Warning;
                    rec.Message = "Neither FrameElement2Node.Geometry and one of {FrameElement2Node.A,FrameElement2Node.Iy,FrameElement2Node.Iz} values are setted";
                    break;

                case 10020:
                    rec.ErrorId = "MA10020";
                    rec.Category = "Elements";
                    rec.Level = TraceLevel.Warning;
                    rec.Message = "FrameElement2Node.ConsiderShearDeformation & FrameElement2Node.Ay and FrameElement2Node.Az values are inconsistent";
                    
                    break;

                case 10100:
                    rec.ErrorId = "MA10100";
                    rec.Category = "Elements";
                    rec.Level = TraceLevel.Warning;
                    rec.Message = "Either FrameElement2Node.E or FrameElement2Node.G is not setted";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("errorNumber");
            }

            return rec;
        }
    }

}