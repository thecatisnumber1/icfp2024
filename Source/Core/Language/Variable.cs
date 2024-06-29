
using System.Text;

namespace Core;

public class Variable : Expression
{
    const int UPPER_HASH_MAGICK = -728727012;
    const int LOWER_HASH_MAGICK = 653149098;

    public long VarKey { get; }

    public Variable(string key)
    {
        VarKey = Encodings.DecodeMachineInt(key);
        CreateKey(UPPER_HASH_MAGICK, LOWER_HASH_MAGICK, VarKey);
    }

    internal override void AppendICFP(StringBuilder builder)
    {
        builder.Append($"v{Encodings.EncodeMachineInt(VarKey)}");
    }

    internal override Value Eval(Dictionary<long, Value> environment)
    {
        if (!environment.ContainsKey(VarKey)) {
            throw new EvaluationException($"Variable {VarKey} not found in environment");
        }

        return environment[VarKey];
    }

    public override string ToString()
    {
        return Encodings.EncodeMachineInt(VarKey);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Variable other && Key == other.Key)
        {
            return VarKey == other.VarKey;
        }
        return false;
    }
}
