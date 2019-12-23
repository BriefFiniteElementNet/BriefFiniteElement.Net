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
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();


            if (typeName.Contains(".Elements.Element,"))
                typeName = typeName.Replace(".Elements.Element,", ".Element,");

            if (typeName.Contains("BriefFiniteElementNet.Load,"))
                typeName = typeName.Replace("BriefFiniteElementNet.Load,", "BriefFiniteElementNet.ElementalLoad,");

            if (typeName.Equals("BriefFiniteElementNet.Load"))
                typeName = typeName.Replace("BriefFiniteElementNet.Load", "BriefFiniteElementNet.ElementalLoad");


            if (typeName == "BriefFiniteElementNet.Elements.Element")
                typeName = typeName.Replace(".Elements.Element", ".Element");

            foreach (var asm in assemblies)
            {
                var tp = asm.GetType(typeName);

                if (tp != null)
                    return tp;
            }

            return null;
        }
    }


}