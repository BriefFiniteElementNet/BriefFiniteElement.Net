using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet
{
    public class FemNetSerializationBinder : SerializationBinder
    {
        /// <summary>
        /// When overridden in a derived class, controls the binding of a serialized object to a type.
        /// </summary>
        /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly" /> name of the serialized object.</param>
        /// <param name="typeName">Specifies the <see cref="T:System.Type" /> name of the serialized object.</param>
        /// <returns>
        /// The type of the object the formatter creates a new instance of.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Type BindToType(string assemblyName, string typeName)
        {
            return Type.GetType(typeName);
            throw new NotImplementedException();
        }
    }
}


    public class FemNetSurrogateSelector : ISurrogateSelector
    {
        /// <summary>
        /// Specifies the next <see cref="T:System.Runtime.Serialization.ISurrogateSelector" /> for surrogates to examine if the current instance does not have a surrogate for the specified type and assembly in the specified context.
        /// </summary>
        /// <param name="selector">The next surrogate selector to examine.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void ChainSelector(ISurrogateSelector selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the surrogate that represents the specified object's type, starting with the specified surrogate selector for the specified serialization context.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type" /> of object (class) that needs a surrogate.</param>
        /// <param name="context">The source or destination context for the current serialization.</param>
        /// <param name="selector">When this method returns, contains a <see cref="T:System.Runtime.Serialization.ISurrogateSelector" /> that holds a reference to the surrogate selector where the appropriate surrogate was found. This parameter is passed uninitialized.</param>
        /// <returns>
        /// The appropriate surrogate for the given type in the given context.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the next surrogate selector in the chain.
        /// </summary>
        /// <returns>
        /// The next surrogate selector in the chain or null.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ISurrogateSelector GetNextSelector()
        {
            throw new NotImplementedException();
        }
    }

    public class FemNetSurrogate : ISerializationSurrogate
    {
        /// <summary>
        /// Populates the provided <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is ISerializable)
            {
                ((ISerializable) obj).GetObjectData(info, context);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Populates the object using the information in the <see cref="T:System.Runtime.Serialization.SerializationInfo" />.
        /// </summary>
        /// <param name="obj">The object to populate.</param>
        /// <param name="info">The information to populate the object.</param>
        /// <param name="context">The source from which the object is deserialized.</param>
        /// <param name="selector">The surrogate selector where the search for a compatible surrogate begins.</param>
        /// <returns>
        /// The populated deserialized object.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            throw new NotImplementedException();
        }
    }


