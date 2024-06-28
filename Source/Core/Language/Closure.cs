using System.Text;

namespace Core;

public class Closure : Value
{
    public Lambda Lambda { get; }
    private Dictionary<long, Value> Environment { get; }

    public Closure(Lambda lambda, Dictionary<long, Value> environment)
    {
        Lambda = lambda;
        Environment = environment;
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
        return $"(closure {Lambda} {Environment})";
    }
}
