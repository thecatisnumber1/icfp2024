using System.Text;

namespace Core;

public class Lambda : Expression
{
    const int UPPER_HASH_MAGICK = -1215836966;
    const int LOWER_HASH_MAGICK = 1287286039;

    public long VariableKey { get; }
    public Expression Content { get; }

    public Lambda(string variableKey, Expression content)
        : this(Encodings.DecodeMachineInt(variableKey), content)
    { }

    public Lambda(long variableKey, Expression content)
    {
        VariableKey = variableKey;
        Content = content;
        CreateKey(UPPER_HASH_MAGICK, LOWER_HASH_MAGICK, VariableKey, Content);
    }

    internal override void AppendICFP(StringBuilder builder)
    {
        builder.Append($"L{Encodings.EncodeMachineInt(VariableKey)} ");
        Content.AppendICFP(builder);
    }

    internal override Value Eval(Dictionary<long, Value> environment)
    {
        return new Closure(this, environment);
    }

    public override string ToString()
    {
        return $"(lambda({VariableKey}) {{ {Content} }})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Lambda other && Key == other.Key)
        {
            return VariableKey.Equals(other.VariableKey) && Content.Equals(other.Content);
        }
        return false;
    }
}
