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

    public Value Eval(Dictionary<int, Value> environment)
    {
        Value left = Left.Eval(environment);
        Value right = Right.Eval(environment);

        return Operator switch
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
            '$' => throw new EvaluationException("Implement binary operator $"),
            _ => throw new EvaluationException($"Invalid binary operator {Operator}"),
        };
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
