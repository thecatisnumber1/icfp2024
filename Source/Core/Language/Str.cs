namespace Core;

public class Str : Value
{
    public string Value { get { return Encodings.DecodeMachineString(MachineValue); } }
    public string MachineValue { get; }

    public Str(string machineValue)
    {
        MachineValue = machineValue;
    }

    public static Str Make(string value)
    {
        return new Str(Encodings.EncodeMachineString(value));
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

    public override bool Equals(object obj)
    {
        if (obj is Str other)
        {
            return MachineValue == other.MachineValue;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return MachineValue.GetHashCode();
    }

}
