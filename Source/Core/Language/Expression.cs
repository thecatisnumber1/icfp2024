using System.Text;

namespace Core;

public abstract class Expression
{
    public abstract Value Eval(Dictionary<long, Value> environment);

    public string ToICFP()
    {
        StringBuilder builder = new();
        AppendICFP(builder);
        return builder.ToString();
    }

    internal abstract void AppendICFP(StringBuilder builder);

    public static Expression Parse(string icfp)
    {
        var tokens = new Queue<string>(icfp.Split(' '));
        
        return ParseHelper(tokens);
    }

    private static Expression ParseHelper(Queue<string> tokens)
    {
        var token = tokens.Dequeue();

        char type = token[0];

        switch (type)
        {
            case 'T':
                ValidateTokenLength(token, 1);
                return Bool.True;
            case 'F':
                ValidateTokenLength(token, 1);
                return Bool.False;
            case 'I':
                return new Integer(token[1..]);
            case 'S':
                return new Str(token[1..]);
            case 'U':
                ValidateTokenLength(token, 2);
                return new Unary(token[1], ParseHelper(tokens));
            case 'B':
                ValidateTokenLength(token, 2);
                return new Binary(token[1], ParseHelper(tokens), ParseHelper(tokens));
            case '?':
                return new If(ParseHelper(tokens), ParseHelper(tokens), ParseHelper(tokens));
            case 'v':
                return new Variable(token[1..]);
            case 'L':
                return new Lambda(token[1..], ParseHelper(tokens));
            default:
                throw new EvaluationException($"Invalid token {token}");
        }
    }

    private static void ValidateTokenLength(string token, int length)
    {
        if (token.Length != length)
            throw new EvaluationException($"Invalid token {token}");
    }
}
