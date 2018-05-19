using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet.SerializationObsolete
{
    [DebuggerDisplay("{FullTypeName}")]
    public class ObjectWrapper
    {
        public bool IsNull;

        public bool IsPremitive;

        public bool IsISerializable;

        public bool IsArray;


        public object PrimitiveValue;

        public ObjectWrapper[] ArrayValue;

        public PropertyWrapper[] ISerializableValue;



        public string FullTypeName;



        public static bool IsPrimitive(Type t)
        {
            return t.IsPrimitive || t == typeof(Decimal) || t == typeof(String);
        }

        public static ObjectWrapper Wrap(object obj)
        {
            var buf = new ObjectWrapper();


            if (ReferenceEquals(null, obj))
            {
                buf.IsNull = true;
                goto done;
            }


            if (IsPrimitive(obj.GetType()))
            {
                buf.IsPremitive = true;
                buf.PrimitiveValue = obj;
                buf.FullTypeName = obj.GetType().FullName;
                goto done;
            }

            if ( obj is IEnumerable)
            {
                buf.IsArray = true;

                var lst = new List<ObjectWrapper>();

                foreach (var item in obj as IEnumerable)
                {
                    lst.Add(ObjectWrapper.Wrap(item));
                }

                buf.ArrayValue = lst.ToArray();
                buf.FullTypeName = obj.GetType().FullName;
                goto done;
            }

            if (obj is ISerializable)
            {
                buf.IsISerializable = true;
                buf.ISerializableValue = GetProperties(obj as ISerializable);
                buf.FullTypeName = obj.GetType().FullName;
                goto done;
            }


            done:

            return buf;

            throw new NotImplementedException();
        }


        public static PropertyWrapper[] GetProperties(ISerializable obj)
        {
            var buf = new List<PropertyWrapper>();

            SerializationInfo info = new SerializationInfo(obj.GetType(), new FormatterConverter());

            obj.GetObjectData(info, new StreamingContext());

            var mData = (object[])info.GetMemberValue("m_data");
            var mTypes = (Type[])info.GetMemberValue("m_types");
            var mMembers = (string[])info.GetMemberValue("m_members");
            var indexes = (Dictionary<string, int>) info.GetMemberValue("m_nameToIndex");

            foreach (var pair in indexes)
            {
                var member = pair.Key;

                if (indexes.ContainsKey(member))
                {
                    var prp = new PropertyWrapper()
                    {
                        PropertyName = member,
                        PropertyValue = Wrap(mData[indexes[member]])
                    };

                    buf.Add(prp);
                }
            }


            return buf.ToArray();
            throw new NotImplementedException();
        }

        public static object UnWrap(ObjectWrapper wrapper)
        {
            throw new NotImplementedException();
        }
    }

    [DebuggerDisplay("{PropertyName} {PropertyValue.FullTypeName}")]
    public class PropertyWrapper
    {
        public string PropertyName;

        public ObjectWrapper PropertyValue;


    }
}
