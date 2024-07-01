namespace Lib;

/// <summary>
/// Represents a point with integer coordinates.
/// </summary>
public readonly record struct PointInt(int X, int Y)
{
    public static readonly PointInt ORIGIN = new(0, 0);

    #region Operators

    public static Vec operator +(PointInt p1, PointInt p2)
        => new(p1.X + p2.X, p1.Y + p2.Y);

    public static PointInt operator +(PointInt p, Vec v)
        => new(p.X + (int)v.X, p.Y + (int)v.Y);
    
    public static PointInt operator +(Vec v, PointInt p)
        => p + v;

    public static Vec operator -(PointInt p1, PointInt p2)
        => new(p1.X - p2.X, p1.Y - p2.Y);

    public static PointInt operator -(PointInt p, Vec v)
        => p + -v;

    #endregion

    public override string ToString() => $"({X}, {Y})";

    public readonly long DistSq(PointInt other)
        => (this - other).MagnitudeSq;

    public readonly double Dist(PointInt other)
        => (this - other).Magnitude;

    public readonly double Manhattan(PointInt other)
        => (this - other).Manhattan;

    /// <summary>
    /// Returns the point that is the midpoint between this point and another.
    /// </summary>
    public readonly PointInt Mid(PointInt other)
        => new((X + other.X) / 2, (Y + other.Y) / 2);
}
