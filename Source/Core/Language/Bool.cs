using System.Text;

namespace Core;

public class Bool : Value
{
    public static readonly Bool True = new(true);
    public static readonly Bool False = new(false);
    public static Bool Make(bool value)
    {
        return value ? True : False;
    }

    public bool Value { get; }

    private Bool(bool value)
    {
        Value = value;
    }

    internal override void AppendICFP(StringBuilder builder)
    {
        builder.Append(Value ? 'T' : 'F');
    }

    public override bool AsBool()
    {
        return Value;
    }

    public override bool EqualsValue(Value other)
    {
        return Value == other.AsBool();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj is Bool other)
        {
            return Value == other.Value;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
