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

    public Value Eval(Dictionary<int, Value> environment)
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
