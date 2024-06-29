using System.Text;

namespace Core;

public class Unary : Expression
{
    const int UPPER_HASH_MAGICK = -1685890912;
    const int LOWER_HASH_MAGICK = 1656027401;

    public char Operator { get; }
    public Expression Operand { get; }

    public Unary(char op, Expression operand)
    {
        Operator = op;
        Operand = operand;
        CreateKey(UPPER_HASH_MAGICK, LOWER_HASH_MAGICK, Operator, Operand);
    }

    internal override void AppendICFP(StringBuilder builder)
    {
        builder.Append($"U{Operator} ");
        Operand.AppendICFP(builder);
    }

    internal override Value Eval(Dictionary<long, Value> environment)
    {
        var operand = Operand.Eval(environment);

        Value result = Operator switch
        {
            '-' => new Integer(-operand.AsInt()),
            '!' => Bool.Make(!operand.AsBool()),
            '#' => new Integer(operand.AsMachineString()),
            '$' => new Str(operand.AsMachineInt()),
            _ => throw new EvaluationException($"Invalid unary operator {Operator}"),
        };

        return result;
    }

    public override string ToString()
    {
        return $"{Operator}{Operand}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Unary other && Key == other.Key)
        {
            return Operand.Equals(other.Operand) && Operator.Equals(other.Operator);
        }
        return false;
    }
}
