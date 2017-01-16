/*
using System; 
using System.Text;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics.Contracts; 

namespace System { 
 
    /// 
    /// Helper so we can call some tuple methods recursively without knowing the underlying types. 
    /// 
    internal interface ITuple {
        string ToString(StringBuilder sb);
        int GetHashCode(IEqualityComparer comparer); 
        int Size { get; }
 
    } 

    public class Tuple<T1,T2,T3> : ITuple, IEquatable<Tuple<T1, T2, T3>>
    {
 
        private readonly T1 m_Item1; 
        private readonly T2 m_Item2;
        private readonly T3 m_Item3; 

        public T1 Item1 { get { return m_Item1; } }
        public T2 Item2 { get { return m_Item2; } }
        public T3 Item3 { get { return m_Item3; } } 

        public Tuple(T1 item1, T2 item2, T3 item3) { 
            m_Item1 = item1; 
            m_Item2 = item2;
            m_Item3 = item3; 
        }


        public bool Equals(Tuple<T1, T2, T3> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T1>.Default.Equals(m_Item1, other.m_Item1) && EqualityComparer<T2>.Default.Equals(m_Item2, other.m_Item2) && EqualityComparer<T3>.Default.Equals(m_Item3, other.m_Item3);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tuple<T1, T2, T3>) obj);
        }

        public static bool operator ==(Tuple<T1, T2, T3> left, Tuple<T1, T2, T3> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tuple<T1, T2, T3> left, Tuple<T1, T2, T3> right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode() {
            unchecked
            {
                int hashCode = EqualityComparer<T1>.Default.GetHashCode(m_Item1);
                hashCode = (hashCode*397) ^ EqualityComparer<T2>.Default.GetHashCode(m_Item2);
                hashCode = (hashCode*397) ^ EqualityComparer<T3>.Default.GetHashCode(m_Item3);
                return hashCode;
            }
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCode();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("("); 
            return ((ITuple)this).ToString(sb);
        } 
 
        string ITuple.ToString(StringBuilder sb) {
            sb.Append(m_Item1); 
            sb.Append(", ");
            sb.Append(m_Item2);
            sb.Append(", ");
            sb.Append(m_Item3); 
            sb.Append(")");
            return sb.ToString(); 
        } 

        int ITuple.Size { 
            get {
                return 3;
            }
        }


    }
 
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// Copyright (c) Microsoft Corporation. All rights reserved.

*/