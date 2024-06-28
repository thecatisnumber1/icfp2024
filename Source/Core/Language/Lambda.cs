namespace Core;

public class Lambda : Expression
{
    public long VariableKey { get; }
    public Expression Content { get; }

    public Lambda(string variableKey, Expression content)
    {
        VariableKey = Encodings.DecodeMachineInt(variableKey);
        Content = content;
    }

    public Value Eval(Dictionary<long, Value> environment)
    {
        return new Closure(this, environment);
    }

    public override string ToString()
    {
        return $"(lambda({VariableKey}) {{ {Content} }})";
    }

    public override bool Equals(object obj)
    {
        if (obj is Lambda other)
        {
            return VariableKey.Equals(other.VariableKey) && Content.Equals(other.Content);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(VariableKey, Content);
    }
}
