using System.Text.Json.Serialization;
using Point = Lib.PointDouble;

namespace Lib;

/// <summary>
/// Represents a rectangle with double-precision coordinates.
/// </summary>
public record class BoxDouble(Point BottomLeft, Point TopRight)
{
    [JsonIgnore]
    public Point BottomRight => new(Right, Bottom);
    [JsonIgnore]
    public Point TopLeft => new(Left, Top);

    [JsonIgnore]
    public double Top => TopRight.Y;
    [JsonIgnore]
    public double Bottom => BottomLeft.Y;
    [JsonIgnore]
    public double Left => BottomLeft.X;
    [JsonIgnore]
    public double Right => TopRight.X;

    [JsonIgnore]
    public double Width => Right - Left;

    [JsonIgnore]
    public double Height => Top - Bottom;

    [JsonIgnore]
    public double Area => Width * Height;

    public static BoxDouble From(Point bottomLeft, double width, double height)
    {
        return new(bottomLeft, bottomLeft + new Vec(width, height));
    }

    public static BoxDouble From(double x1, double y1, double x2, double y2)
    {
        return new(new Point(Math.Min(x1, x2), Math.Min(y1, y2)),
                   new Point(Math.Max(x1, x2), Math.Max(y1, y2)));
    }

    public override string ToString()
        => $"({BottomLeft}, {TopRight})";

    /// <summary>
    /// Returns a new Box that is shifted by the given amount.
    /// </summary>
    public BoxDouble Shift(Vec amount)
        => new(BottomLeft + amount, TopRight + amount);

    /// <summary>
    /// Returns a new Box that is shifted by the given amount.
    /// </summary>
    public BoxDouble Shift(double x, double y)
        => Shift(new Vec(x, y));

    /// <summary>
    /// Returns a new Box that is resized by the given amount, maintaining the current BottomLeft position.
    /// </summary>
    public BoxDouble Resize(Vec amount)
        => new(BottomLeft, TopRight + amount);

    /// <summary>
    /// Returns a new Box that is resized by the given amount, maintaining the current BottomLeft position.
    /// </summary>
    public BoxDouble Resize(double amount)
        => Resize(new Vec(amount, amount));
}
