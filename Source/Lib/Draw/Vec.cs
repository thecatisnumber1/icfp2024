namespace Lib;

/// <summary>
/// Represents a 2D vector with double-precision coordinates.
/// </summary>
public readonly record struct Vec(int X, int Y)
{
    public static readonly Vec ZERO = new(0, 0);
    public static readonly Vec WEST = new(-1, 0);
    public static readonly Vec EAST = -WEST;
    public static readonly Vec NORTH = new(0, 1);
    public static readonly Vec SOUTH = -NORTH;

    public static readonly Vec[] DIRECTIONS = [ NORTH, EAST, SOUTH, WEST ];

    #region Operators

    public static Vec operator +(Vec v1, Vec v2)
        => new(v1.X + v2.X, v1.Y + v2.Y);

    public static Vec operator -(Vec v)
        => new(-v.X, -v.Y);

    public static Vec operator -(Vec v1, Vec v2)
        => v1 + -v2;

    public static Vec operator *(int scale, Vec v)
        => new(scale * v.X, scale * v.Y);

    public static Vec operator *(Vec v, int scale)
        => scale * v;

    public static Vec operator /(Vec v, int scale)
        => new(v.X / scale, v.Y / scale);

    #endregion

    public override readonly string ToString()
        => $"<{X}, {Y}>";

    /// <summary>
    /// True if this vector is close to another vector.
    /// Like ==, but with a tolerance for comparison after double-precision math operations.
    /// </summary>
    public bool IsClose(Vec other, double tolerance = 1e-6)
        => Math.Abs(X - other.X) < tolerance && Math.Abs(Y - other.Y) < tolerance;

    public readonly long MagnitudeSq
        => (long)X * X + (long)Y * Y;

    public readonly double Magnitude
        => Math.Sqrt(MagnitudeSq);

    public readonly double Manhattan
        => Math.Abs(X) + Math.Abs(Y);

    public double CrossProduct(Vec b)
        => X * b.Y - Y * b.X;

    public double DotProduct(Vec b)
        => X * b.X + Y * b.Y;

    /// <summary>
    /// Rotates this vector 90 degrees clockwise.
    /// </summary>
    public Vec RotateCCW()
        => new(-Y, X);
}