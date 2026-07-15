#nullable disable
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Select(method => method.Name);
    }

    public IEnumerable<string> GetMethodParams(string methodname)
    {
        MethodInfo method = _type.GetMethod(methodname);
        if (method == null)
        {
            return Enumerable.Empty<string>();
        }
        return method.GetParameters()
                     .Select(param => param.Name);
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Select(field => field.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Select(prop => prop.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.GetCustomAttribute<T>() != null;
    }
}
