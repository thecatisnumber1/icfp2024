namespace Lib;

/// <summary>
/// Represents a point with double-precision coordinates.
/// </summary>
public readonly record struct PointDouble(double X, double Y)
{
    public static readonly PointDouble ORIGIN = new(0, 0);

    #region Operators

    public static Vec operator +(PointDouble p1, PointDouble p2)
        => new(p1.X + p2.X, p1.Y + p2.Y);

    public static PointDouble operator +(PointDouble p, Vec v)
        => new(p.X + v.X, p.Y + v.Y);
    
    public static PointDouble operator +(Vec v, PointDouble p)
        => p + v;

    public static Vec operator -(PointDouble p1, PointDouble p2)
        => new(p1.X - p2.X, p1.Y - p2.Y);

    public static PointDouble operator -(PointDouble p, Vec v)
        => p + -v;

    #endregion

    public override string ToString() => $"({X}, {Y})";

    /// <summary>
    /// True if this point is close to another point.
    /// Like ==, but with a tolerance for comparison after double-precision math operations.
    /// </summary>
    public bool IsClose(PointDouble other, double tolerance = 1e-6)
        => DistSq(other) < tolerance * tolerance;

    public readonly double DistSq(PointDouble other)
        => (this - other).MagnitudeSq;

    public readonly double Dist(PointDouble other)
        => (this - other).Magnitude;

    public readonly double Manhattan(PointDouble other)
        => (this - other).Manhattan;

    /// <summary>
    /// Returns the point that is the midpoint between this point and another.
    /// </summary>
    public readonly PointDouble Mid(PointDouble other)
        => new((X + other.X) / 2, (Y + other.Y) / 2);
}
