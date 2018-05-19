/// <summary>
/// Extensions methos for using reflection to get / set member values
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets the public or private member using reflection.
    /// </summary>
    /// <param name="obj">The source target.</param>
    /// <param name="memberName">Name of the field or property.</param>
    /// <returns>the value of member</returns>
    public static object GetMemberValue(this object obj, string memberName)
    {
        var memInf = GetMemberInfo(obj, memberName);

        if (memInf == null)
            throw new System.Exception("memberName");

        if (memInf is System.Reflection.PropertyInfo)
            return memInf.As<System.Reflection.PropertyInfo>().GetValue(obj, null);

        if (memInf is System.Reflection.FieldInfo)
            return memInf.As<System.Reflection.FieldInfo>().GetValue(obj);

        throw new System.Exception();
    }

    /// <summary>
    /// Gets the public or private member using reflection.
    /// </summary>
    /// <param name="obj">The target object.</param>
    /// <param name="memberName">Name of the field or property.</param>
    /// <returns>Old Value</returns>
    public static object SetMemberValue(this object obj, string memberName, object newValue)
    {
        var memInf = GetMemberInfo(obj, memberName);


        if (memInf == null)
            throw new System.Exception("memberName");

        var oldValue = obj.GetMemberValue(memberName);

        if (memInf is System.Reflection.PropertyInfo)
            memInf.As<System.Reflection.PropertyInfo>().SetValue(obj, newValue, null);
        else if (memInf is System.Reflection.FieldInfo)
            memInf.As<System.Reflection.FieldInfo>().SetValue(obj, newValue);
        else
            throw new System.Exception();

        return oldValue;
    }

    /// <summary>
    /// Gets the member info
    /// </summary>
    /// <param name="obj">source object</param>
    /// <param name="memberName">name of member</param>
    /// <returns>instanse of MemberInfo corresponsing to member</returns>
    private static System.Reflection.MemberInfo GetMemberInfo(object obj, string memberName)
    {
        var prps = new System.Collections.Generic.List<System.Reflection.PropertyInfo>();

        prps.Add(obj.GetType().GetProperty(memberName,
                                           System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                                           System.Reflection.BindingFlags.FlattenHierarchy));
        prps = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(prps, i => !ReferenceEquals(i, null)));
        if (prps.Count != 0)
            return prps[0];

        var flds = new System.Collections.Generic.List<System.Reflection.FieldInfo>();

        flds.Add(obj.GetType().GetField(memberName,
                                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance |
                                        System.Reflection.BindingFlags.FlattenHierarchy));

        //to add more types of properties

        flds = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(flds, i => !ReferenceEquals(i, null)));

        if (flds.Count != 0)
            return flds[0];

        return null;
    }

    [System.Diagnostics.DebuggerHidden]
    private static T As<T>(this object obj)
    {
        return (T)obj;
    }
}