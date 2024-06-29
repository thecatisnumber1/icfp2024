using System.Text;

namespace Core;

public abstract class Expression
{
    protected static Dictionary<int, Value> _evalCache = new();

    protected static int HashEnvironment(Dictionary<long, Value> environment)
    {
        int hash = 0;
        foreach (var kvp in environment)
        {
            hash = HashCode.Combine(hash, kvp.Key, kvp.Value);
        }
        return hash;
    }

    protected int HashEval(Dictionary<long, Value> environment)
    {
        return HashCode.Combine(GetHashCode(), HashEnvironment(environment));
    }

    public Value Eval()
    {
        _evalCache = new();
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

    public static Binary operator |(Expression e1, Expression e2)
        => new('|', e1, e2);

    public static Binary operator &(Expression e1, Expression e2)
        => new('&', e1, e2);
}
