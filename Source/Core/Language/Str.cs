using System.Text;

namespace Core;

public class Str : Value
{
    const int UPPER_HASH_MAGICK = 1560627002;
    const int LOWER_HASH_MAGICK = -1142775805;

    public string Value { get { return Encodings.DecodeMachineString(MachineValue); } }
    public string MachineValue { get; }

    public Str(string machineValue)
    {
        MachineValue = machineValue;
        CreateKey(UPPER_HASH_MAGICK, LOWER_HASH_MAGICK, MachineValue);
    }

    public static Str Make(string value)
    {
        return new Str(Encodings.EncodeMachineString(value));
    }

    internal override void AppendICFP(StringBuilder builder)
    {
        builder.Append($"S{MachineValue}");
    }

    public override string AsString()
    {
        return Value;
    }

    public override string AsMachineString()
    {
        return MachineValue;
    }

    public override bool EqualsValue(Value other)
    {
        return MachineValue == other.AsMachineString();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Str other && Key == other.Key)
        {
            return MachineValue == other.MachineValue;
        }
        return false;
    }
}
