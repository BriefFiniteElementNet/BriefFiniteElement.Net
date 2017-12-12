using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Common
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


        /// <summary>
        /// Writes the specified message to all listeners.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="formats">The formats.</param>
        public void Write(TraceLevel level,string message,params object[] formats)
        {
            foreach (var listener in _listeners)
            {
                if (listener != null)
                    listener.Write(TraceRecord.Create(level, string.Format(message, formats)));
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

        /// <summary>
        /// Gets the records catched by this listener.
        /// </summary>
        /// <value>
        /// The records catched by this yet.
        /// </value>
        ReadOnlyCollection<TraceRecord> Records { get; }
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
        private DateTime _timeStamp = DateTime.Now;
        private string _solution;
        private string _issueId;
        private TraceLevel _level;
        private string _helpLink = "https://brieffiniteelementnet.codeplex.com/wikipage?title=Error%20message%20list&referringTitle=Documentation";

        #endregion

        #region Properties

        [Obsolete]
        public int Number
        {
            get { return _number; }
        }

        [Obsolete]
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
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

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp of record.
        /// </value>
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

        /// <summary>
        /// Gets or sets the issue identifier.
        /// </summary>
        /// <value>
        /// The issue id which exists here with detailed info:
        ///  https://brieffiniteelementnet.codeplex.com/wikipage?title=Error%20message%20list
        /// </value>
        public string IssueId
        {
            get { return _issueId; }
            set { _issueId = value; }
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

        /// <summary>
        /// Gets or sets the target identifier.
        /// </summary>
        /// <value>
        /// The identifier of either node or element or anything else who causes this.
        /// </value>
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
}