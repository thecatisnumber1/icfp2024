using System.Text;

namespace Core;

public abstract class Expression
{
    public long Key { get; private set; }

    protected void CreateKey(
        int magicUpper,
        int magicLower,
        object term1,
        object? term2 = null,
        object? term3 = null
        )
    {
        int upper = HashCode.Combine(magicUpper, term1, term2, term3);
        int lower = HashCode.Combine(term1, term2, term3, magicLower);
        Key = ((long)upper << 32) | (uint)lower;
    }

    public Value Eval()
    {
        return Eval(new());
    }

    internal abstract Value Eval(Dictionary<long, Value> environment);

    public string ToICFP()
    {
        StringBuilder builder = new();
        AppendICFP(builder);
        return builder.ToString();
    }

    internal abstract void AppendICFP(StringBuilder builder);

    public static Expression Parse(string icfp)
    {
        var tokens = icfp.Split(' ').Reverse();
        Stack<Expression> stack = new();

        foreach (var token in tokens)
        {
            char type = token[0];

            switch (type)
            {
                case 'T':
                    ValidateTokenLength(token, 1);
                    stack.Push(Bool.True);
                    break;
                case 'F':
                    ValidateTokenLength(token, 1);
                    stack.Push(Bool.False);
                    break;
                case 'I':
                    stack.Push(new Integer(token[1..]));
                    break;
                case 'S':
                    stack.Push(new Str(token[1..]));
                    break;
                case 'U':
                    ValidateTokenLength(token, 2);
                    stack.Push(new Unary(token[1], stack.Pop()));
                    break;
                case 'B':
                    ValidateTokenLength(token, 2);
                    stack.Push(new Binary(token[1], stack.Pop(), stack.Pop()));
                    break;
                case '?':
                    ValidateTokenLength(token, 1);
                    stack.Push(new If(stack.Pop(), stack.Pop(), stack.Pop()));
                    break;
                case 'v':
                    stack.Push(new Variable(token[1..]));
                    break;
                case 'L':
                    stack.Push(new Lambda(token[1..], stack.Pop()));
                    break;
                default:
                    throw new EvaluationException($"Invalid token {token}");
            }
        }

        if (stack.Count != 1)
        {
            throw new EvaluationException("Invalid ICFP: Multiple Expressions");
        }

        return stack.Pop();
    }

    private static void ValidateTokenLength(string token, int length)
    {
        if (token.Length != length)
            throw new EvaluationException($"Invalid token {token}");
    }

    public static Unary operator -(Expression e)
        => new('-', e);

    public static Unary operator !(Expression e)
        => new('!', e);

    public static Binary operator +(Expression e1, Expression e2)
        => new('+', e1, e2);

    public static Binary operator -(Expression e1, Expression e2)
        => new('-', e1, e2);

    public static Binary operator *(Expression e1, Expression e2)
        => new('*', e1, e2);

    public static Binary operator /(Expression e1, Expression e2)
        => new('/', e1, e2);

    public static Binary operator %(Expression e1, Expression e2)
        => new('%', e1, e2);

    public static Binary operator <(Expression e1, Expression e2)
        => new('<', e1, e2);

    public static Binary operator >(Expression e1, Expression e2)
        => new('>', e1, e2);
    public static Expression operator ==(Expression e1, Expression e2)
        => new Binary('=', e1, e2);
    public static Expression operator !=(Expression e1, Expression e2)
        => !(e1 == e2);

    public static Binary operator |(Expression e1, Expression e2)
        => new('|', e1, e2);

    public static Binary operator &(Expression e1, Expression e2)
        => new('&', e1, e2);
}
