using System.Text;

namespace Core;

public class Binary : Expression
{
    const int UPPER_HASH_MAGICK = 710809744;
    const int LOWER_HASH_MAGICK = -1193599821;

    public Expression Left { get; }
    public Expression Right { get; }
    public char Operator { get; }

    public Binary(char op, Expression left, Expression right)
    {
        if (left.Equals(right))
        {
            // Drop it in case it is a duplicate object with matching expression
            right = left;
        }

        Left = left;
        Right = right;
        Operator = op;
        CreateKey(UPPER_HASH_MAGICK, LOWER_HASH_MAGICK, Operator, Left, Right);
    }

    internal override Value Eval(Dictionary<long, Value> environment)
    {
        Value left = Left.Eval(environment);
        // Don't eval again if it is the same expression
        Value right = left.Equals(Right) ? left : Right.Eval(environment);

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

    public override bool Equals(object? obj)
    {
        if (obj is Binary other && Key == other.Key)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right) && Operator.Equals(other.Operator);
        }
        return false;
    }
}
