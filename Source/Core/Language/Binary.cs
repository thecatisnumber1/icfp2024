using System.Text;

namespace Core;

public class Binary : Expression
{
    public Expression Left { get; }
    public Expression Right { get; }
    public char Operator { get; }

    public Binary(char op, Expression left, Expression right)
    {
        Left = left;
        Right = right;
        Operator = op;
    }

    internal override Value Eval(Dictionary<long, Value> environment)
    {
        int hash = HashEval(environment);
        _evalCache.TryGetValue(hash, out var value);
        if (value != null)
        {
            return value;
        }


        Value left = Left.Eval(environment);
        Value right = Right.Eval(environment);

        Value result = Operator switch
        {
            '+' => new Integer(left.AsInt() + right.AsInt()),
            '-' => new Integer(left.AsInt() - right.AsInt()),
            '*' => new Integer(left.AsInt() * right.AsInt()),
            '/' => new Integer(left.AsInt() / right.AsInt()),
            '%' => new Integer(left.AsInt() % right.AsInt()),
            '<' => Bool.Make(left.AsInt() < right.AsInt()),
            '>' => Bool.Make(left.AsInt() > right.AsInt()),
            '=' => Bool.Make(left.EqualsValue(right)),
            '|' => Bool.Make(left.AsBool() || right.AsBool()),
            '&' => Bool.Make(left.AsBool() && right.AsBool()),
            '.' => new Str(left.AsMachineString() + right.AsMachineString()),
            'T' => new Str(right.AsMachineString()[..(int)left.AsInt()]),
            'D' => new Str(right.AsMachineString()[(int)left.AsInt()..]),
            '$' => left.AsClosure().Apply(right),
            _ => throw new EvaluationException($"Invalid binary operator {Operator}"),
        };

        _evalCache[hash] = result;
        return result;
    }

    internal override void AppendICFP(StringBuilder builder)
    {
        builder.Append($"B{Operator} ");
        Left.AppendICFP(builder);
        builder.Append(' ');
        Right.AppendICFP(builder);
    }

    public override string ToString()
    {
        return $"({Left} {Operator} {Right})";
    }

    public override bool Equals(object obj)
    {
        if (obj is Binary other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right) && Operator.Equals(other.Operator);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Right, Operator);
    }
}
