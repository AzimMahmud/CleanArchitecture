namespace Core.Common;

public class TypedIdValueBase : IEquatable<TypedIdValueBase>
{
    public Guid Value { get; }

    public TypedIdValueBase(Guid value)
    {
        Value = value;
    }
    public bool Equals(TypedIdValueBase? other)
    {
        return this.Value == other?.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is TypedIdValueBase other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(TypedIdValueBase obj1, TypedIdValueBase obj2)
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

    public static bool operator !=(TypedIdValueBase obj1, TypedIdValueBase obj2)
    {
        return !(obj1 == obj2);
    }
}