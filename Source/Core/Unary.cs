﻿namespace Core;

public class Unary : Expression
{
    public char Operator { get; }
    public Expression Operand { get; }

    public Unary(char op, Expression operand)
    {
        Operator = op;
        Operand = operand;
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