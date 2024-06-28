namespace Core;

public class Unary : Expression
{
    public char Operator { get; }
    public Expression Operand { get; }

    public Unary(char op, Expression operand)
    {
        Operator = op;
        Operand = operand;
    }

    public Value Eval(Dictionary<int, Value> environment)
    {
        var operand = Operand.Eval(environment);

        switch (Operator)
        {
            case '-':
                return new Integer(-operand.AsInt());
            case '!':
                return Bool.Make(!operand.AsBool());
            case '#':
                return new Integer(operand.AsMachineString());
            case '$':
                return new Str(operand.AsMachineInt());
            default:
                throw new EvaluationException($"Invalid unary operator {Operator}");
        }
    }

    public override string ToString()
    {
        return $"{Operator}{Operand}";
    }

    public override bool Equals(object obj)
    {
        if (obj is Unary other)
        {
            return Operand.Equals(other.Operand) && Operator.Equals(other.Operator);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Operand, Operator);
    }
}
