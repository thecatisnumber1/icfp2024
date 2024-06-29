using System.Text;

namespace Core;

public class Closure : Value
{
    const int UPPER_HASH_MAGICK = -1081232256;
    const int LOWER_HASH_MAGICK = -1879060346;

    public Lambda Lambda { get; }
    private Dictionary<long, Value> Environment { get; }

    public Closure(Lambda lambda, Dictionary<long, Value> environment)
    {
        Lambda = lambda;
        Environment = environment;
        CreateKey(UPPER_HASH_MAGICK, LOWER_HASH_MAGICK, Lambda, Environment);
    }

    internal override void AppendICFP(StringBuilder builder)
    {
        throw new NotImplementedException();
    }

    public override Closure AsClosure()
    {
        return this;
    }

    public Value Apply(Value argument)
    {
        var newEnvironment = new Dictionary<long, Value>(Environment);
        newEnvironment[Lambda.VariableKey] = argument;

        return Lambda.Content.Eval(newEnvironment);
    }

    public override bool EqualsValue(Value other)
    {
        throw new EvaluationException("Not Implemented");
    }

    public override string ToString()
    {
        return $"(closure {Lambda} {string.Join(", ", Environment.Select(kvp => $"{kvp.Key}: {kvp.Value}"))})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Closure other && Key == other.Key)
        {
            return Lambda.Equals(other.Lambda) && Environment.SequenceEqual(other.Environment);
        }
        return false;
    }
}
