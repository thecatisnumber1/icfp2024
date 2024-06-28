using System.Text;

namespace Core;

public static class Encodings
{
    const int BASE = 94;
    const char MIN_CHAR = '!';
    const char MAX_CHAR = '~';
    const string CHAR_MAP = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`|~ \n";

    public static long DecodeMachineInt(string body)
    {
        if (body.Length == 0)
        {
            throw new EvaluationException("Invalid integer literal");
        }

        long result = 0;

        foreach (char c in body)
        {
            if (c < MIN_CHAR || c > MAX_CHAR)
            {
                throw new EvaluationException($"Invalid integer literal character {c}");
            }

            result = result * BASE + (c - MIN_CHAR);
        }

        return result;
    }

    public static string DecodeMachineString(string body)
    {
        StringBuilder builder = new();

        foreach (char c in body)
        {
            if (c < MIN_CHAR || c > MAX_CHAR)
            {
                throw new EvaluationException($"Invalid string literal character {c}");
            }

            builder.Append(CHAR_MAP[c - MIN_CHAR]);
        }

        return builder.ToString();
    }
}
