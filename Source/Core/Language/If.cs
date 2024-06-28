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

    public override Value Eval(Dictionary<long, Value> environment)
    {
        var condition = Condition.Eval(environment);

        if (condition.AsBool())
        {
            return Then.Eval(environment);
        }
        else
        {
            return Else.Eval(environment);
        }
    }

    public override string ToString()
    {
        return $"(if {Condition} {Then} {Else})";
    }
}
