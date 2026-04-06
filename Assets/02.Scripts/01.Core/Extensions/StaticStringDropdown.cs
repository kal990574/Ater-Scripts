using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;

public static class StaticStringDropdown<T> where T : class
{
    public static IEnumerable<string> GetValues()
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType != typeof(string))
            {
                continue;
            }

            yield return (string)field.GetValue(null);
        }
    }

    public static bool Contains(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        foreach (string key in GetValues())
        {
            if (key == value)
            {
                return true;
            }
        }

        return false;
    }

    public static IEnumerable<ValueDropdownItem<string>> GetItems()
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType != typeof(string))
            {
                continue;
            }

            string name = field.Name;
            string value = (string)field.GetValue(null);
            yield return new ValueDropdownItem<string>(name, value);
        }
    }
}
