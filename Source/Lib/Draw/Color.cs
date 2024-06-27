namespace Lib;

/// <summary>
/// Represents a color with byte-precision RGBA values.
/// </summary>
public readonly record struct Color(byte R = 0, byte G = 0, byte B = 0, byte A = 0)
{
    public static readonly Color WHITE = new(255, 255, 255, 255);
    public static readonly Color BLACK = new(0, 0, 0, 255);
    public static readonly Color EMPTY = new(0, 0, 0, 0);

    /// <summary>
    /// Creates a new color from HSV values.
    /// </summary>
    public static Color FromHSV(double h, double s, double v)
    {
        int hi = (int)(h * 6);
        double f = h * 6 - hi;
        double p = v * (1 - s);
        double q = v * (1 - f * s);
        double t = v * (1 - (1 - f) * s);

        var (r, g, b) = hi switch
        {
            0 => (v, t, p),
            1 => (q, v, p),
            2 => (p, v, t),
            3 => (p, q, v),
            4 => (t, p, v),
            _ => (v, p, q),
        };

        return new Color((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), 255);
    }

    public override readonly string ToString()
        => $"[{R}, {G}, {B}, {A}]";

    /// <summary>
    /// Calculates the distance between two colors using Euclidean distance.
    /// </summary>
    public double Diff(Color other)
    {
        var rDist = (R - other.R) * (R - other.R);
        var gDist = (G - other.G) * (G - other.G);
        var bDist = (B - other.B) * (B - other.B);
        var aDist = (A - other.A) * (A - other.A);
        var distance = Math.Sqrt(rDist + gDist + bDist + aDist);
        return distance;
    }
}
