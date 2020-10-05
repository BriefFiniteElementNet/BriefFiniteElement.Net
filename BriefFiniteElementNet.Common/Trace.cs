using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Globalization;

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
                    listener.Write(TraceRecord.Create(level, string.Format(CultureInfo.CurrentCulture, message, formats)));
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
}