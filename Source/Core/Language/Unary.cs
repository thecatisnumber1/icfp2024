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

        return Operator switch
        {
            '-' => new Integer(-operand.AsInt()),
            '!' => Bool.Make(!operand.AsBool()),
            '#' => new Integer(operand.AsMachineString()),
            '$' => new Str(operand.AsMachineInt()),
            _ => throw new EvaluationException($"Invalid unary operator {Operator}"),
        };
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
