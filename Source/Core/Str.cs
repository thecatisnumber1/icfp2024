namespace Core;

public class Str : Value
{
    public string Value { get { return Encodings.DecodeMachineString(MachineValue); } }
    public string MachineValue { get; }

    public Str(string machineValue)
    {
        MachineValue = machineValue;
    }

    public override string AsString()
    {
        return Value;
    }

    public override string AsMachineString()
    {
        return MachineValue;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj is Str other)
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
