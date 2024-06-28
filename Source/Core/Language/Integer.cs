namespace Core;

public class Integer : Value
{
    public long Value { get; }
    public string MachineValue { get { return Encodings.EncodeMachineInt(Value); } }

    public Integer(long value)
    {
        Value = value;
    }

    public Integer(string machineValue)
    {
        Value = Encodings.DecodeMachineInt(machineValue);
    }

    public override long AsInt() => Value;
    public override string AsMachineInt() => MachineValue;

    public override bool EqualsValue(Value other)
    {
        return Value == other.AsInt();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj is Integer other)
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
