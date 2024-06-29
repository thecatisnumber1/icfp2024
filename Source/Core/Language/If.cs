using System.Text;

namespace Core;

public class If : Expression
{
    public Expression Condition { get; }
    public Expression Then { get; }
    public Expression Else { get; }

    public If(Expression condition, Expression then, Expression @else)
    {
        Condition = condition;
        Then = then;
        Else = @else;
    }

    internal override void AppendICFP(StringBuilder builder)
    {
        builder.Append("? ");
        Condition.AppendICFP(builder);
        builder.Append(' ');
        Then.AppendICFP(builder);
        builder.Append(' ');
        Else.AppendICFP(builder);
    }

    internal override Value Eval(Dictionary<long, Value> environment)
    {
        int hash = HashEval(environment);
        _evalCache.TryGetValue(hash, out var value);
        if (value != null)
        {
            return value;
        }

        var condition = Condition.Eval(environment);

        Value result = condition.AsBool() ? Then.Eval(environment) : Else.Eval(environment);
        _evalCache[hash] = result;
        return result;
    }

    public override string ToString()
    {
        return $"(if {Condition} {Then} {Else})";
    }

    public override bool Equals(object obj)
    {
        if (obj is If other)
        {
            return Condition.Equals(other.Condition) && Then.Equals(other.Then) && Else.Equals(other.Else);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Condition, Then, Else);
    }
}
