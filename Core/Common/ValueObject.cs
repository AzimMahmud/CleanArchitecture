using System.Reflection;

namespace Core.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
    private List<PropertyInfo> _properties;
    private List<FieldInfo> _fields;
    
    public bool Equals(ValueObject? obj)
    {
        return Equals(obj as object);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        return GetProperties().All(p => PropertiesAreEqual(obj, p))
               && GetFields().All(f => FieldsAreEqual(obj, f));
    }

    private bool FieldsAreEqual(object obj, FieldInfo field)
    {
        return object.Equals(field.GetValue(this), field.GetValue(obj));
    }

    private IEnumerable<FieldInfo> GetFields()
    {
        if (this._fields is null)
        {
            this._fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.GetCustomAttribute(typeof(IgnoreMemberAttribute)) == null)
                .ToList();
        }

        return this._fields;
    }

    private bool PropertiesAreEqual(object obj, PropertyInfo p)
    {
        return object.Equals(p.GetValue(this, null), p.GetValue(obj, null));
    }

    private IEnumerable<PropertyInfo> GetProperties()
    {
        if (this._properties is null)
        {
            this._properties = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute(typeof(IgnoreMemberAttribute)) == null)
                .ToList();
        }

        return this._properties;
    }

    public static bool operator ==(ValueObject obj1, ValueObject obj2)
    {
        if (object.Equals(obj1, null))
        {
            if (object.Equals(obj2, null))
            {
                return true;
            }

            return false;
        }

        return obj1.Equals(obj2);
    }

    public static bool operator !=(ValueObject obj1, ValueObject obj2)
    {
        return !(obj1 == obj2);
    }
    
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var property in GetProperties())
            {
                var value = property.GetValue(this, null);
                hash = HashValue(hash, value);
            }

            foreach (var field in GetFields())
            {
                var value = field.GetValue(this);
                hash = HashValue(hash, value);
            }

            return hash;
        }
    }

    private static int HashValue(int seed, object? value)
    {
        var currentHash = value?.GetHashCode() ?? 0;
        return seed * 23 + currentHash;
    }

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}