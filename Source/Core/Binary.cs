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
