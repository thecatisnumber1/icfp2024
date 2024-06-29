using System.Text;

namespace Core;

public class Bool : Value
{
    const int UPPER_HASH_MAGICK = -952961297;
    const int LOWER_HASH_MAGICK = -1453934138;

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
        CreateKey(UPPER_HASH_MAGICK, LOWER_HASH_MAGICK, Value);
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

    public override bool Equals(object? obj)
    {
        if (obj is Bool other && Key == other.Key)
        {
            return Value == other.Value;
        }
        return false;
    }
}
