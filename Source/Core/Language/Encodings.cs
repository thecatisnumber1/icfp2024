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

    public static string EncodeMachineInt(long value)
    {
        StringBuilder builder = new();

        while (value > 0)
        {
            builder.Append((char)(MIN_CHAR + (char)(value % BASE)));
            value /= BASE;
        }

        return new string(builder.ToString().Reverse().ToArray());
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

    public static string EncodeMachineString(string value)
    {
        StringBuilder builder = new();

        foreach (char c in value)
        {
            int index = CHAR_MAP.IndexOf(c);
            if (index == -1)
            {
                throw new EvaluationException($"Invalid string literal character {c}");
            }

            builder.Append((char)(MIN_CHAR + index));
        }

        return builder.ToString();
    }
}
