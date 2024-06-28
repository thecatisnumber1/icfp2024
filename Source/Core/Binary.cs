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

        switch (Operator)
        {
            case '+':
                return new Integer(left.AsInt() + right.AsInt());
            case '-':
                return new Integer(left.AsInt() - right.AsInt());
            case '*':
                return new Integer(left.AsInt() * right.AsInt());
            case '/':
                return new Integer(left.AsInt() / right.AsInt());
            case '%':
                return new Integer(left.AsInt() % right.AsInt());
            case '<':
                return Bool.Make(left.AsInt() < right.AsInt());
            case '>':
                return Bool.Make(left.AsInt() > right.AsInt());
            case '=':
                return Bool.Make(left.EqualsValue(right));
            case '|':
                return Bool.Make(left.AsBool() || right.AsBool());
            case '&':
                return Bool.Make(left.AsBool() && right.AsBool());
            case '.':
                return new Str(left.AsMachineString() + right.AsMachineString());
            case 'T':
                return new Str(right.AsMachineString().Substring(0, (int)left.AsInt()));
            case 'D':
                return new Str(right.AsMachineString().Substring((int)left.AsInt()));
            case '$':
                throw new EvaluationException("Implement binary operator $");
            default:
                throw new EvaluationException($"Invalid binary operator {Operator}");
        }
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
