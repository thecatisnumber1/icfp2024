namespace Core;

public class Bool : Value
{
    public static Bool True = new Bool(true);
    public static Bool False = new Bool(false);
    public static Bool Make(bool value)
    {
        return value ? True : False;
    }

    public bool Value { get; }

    private Bool(bool value)
    {
        Value = value;
    }

    public override bool AsBool()
    {
        return Value;
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
