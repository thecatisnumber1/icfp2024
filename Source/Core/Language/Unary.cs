using System.Text;

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

    internal override void AppendICFP(StringBuilder builder)
    {
        builder.Append($"U{Operator} ");
        Operand.AppendICFP(builder);
    }

    internal override Value Eval(Dictionary<long, Value> environment)
    {
        int hash = HashEval(environment);
        _evalCache.TryGetValue(hash, out var value);
        if (value != null)
        {
            return value;
        }

        var operand = Operand.Eval(environment);

        Value result = Operator switch
        {
            '-' => new Integer(-operand.AsInt()),
            '!' => Bool.Make(!operand.AsBool()),
            '#' => new Integer(operand.AsMachineString()),
            '$' => new Str(operand.AsMachineInt()),
            _ => throw new EvaluationException($"Invalid unary operator {Operator}"),
        };

        _evalCache[hash] = result;
        return result;
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
