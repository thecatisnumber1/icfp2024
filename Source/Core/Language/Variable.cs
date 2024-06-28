
namespace Core;

public class Variable : Expression
{
    public long Key { get; }

    public Variable(string key)
    {
        Key = Encodings.DecodeMachineInt(key);
    }

    public Value Eval(Dictionary<long, Value> environment)
    {
        if (!environment.ContainsKey(Key)) {
            throw new EvaluationException($"Variable {Key} not found in environment");
        }

        return environment[Key];
    }

    public override string ToString()
    {
        return Encodings.EncodeMachineInt(Key);
    }

    public override bool Equals(object obj)
    {
        if (obj is Variable other)
        {
            return Key == other.Key;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}
