using System;

namespace BriefFiniteElementNet.Common
{
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