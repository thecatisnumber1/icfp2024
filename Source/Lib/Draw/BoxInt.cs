using System.Text.Json.Serialization;
using Point = Lib.PointInt;

namespace Lib;

/// <summary>
/// Represents a rectangle with integer coordinates.
/// </summary>
public record class BoxInt(Point BottomLeft, Point TopRight)
{
    [JsonIgnore]
    public Point BottomRight => new(Right, Bottom);
    [JsonIgnore]
    public Point TopLeft => new(Left, Top);

    [JsonIgnore]
    public int Top => TopRight.Y;
    [JsonIgnore]
    public int Bottom => BottomLeft.Y;
    [JsonIgnore]
    public int Left => BottomLeft.X;
    [JsonIgnore]
    public int Right => TopRight.X;

    [JsonIgnore]
    public int Width => Right - Left;

    [JsonIgnore]
    public int Height => Top - Bottom;

    [JsonIgnore]
    public int Area => Width * Height;

    public static BoxInt From(Point bottomLeft, int width, int height)
    {
        return new(bottomLeft, bottomLeft + new Vec(width, height));
    }

    public static BoxInt From(int x1, int y1, int x2, int y2)
    {
        return new(new Point(Math.Min(x1, x2), Math.Min(y1, y2)),
                   new Point(Math.Max(x1, x2), Math.Max(y1, y2)));
    }

    public override string ToString()
        => $"({BottomLeft}, {TopRight})";

    /// <summary>
    /// Returns a new Box that is shifted by the given amount.
    /// </summary>
    public BoxInt Shift(Vec amount)
        => new(BottomLeft + amount, TopRight + amount);

    /// <summary>
    /// Returns a new Box that is shifted by the given amount.
    /// </summary>
    /// <returns></returns>
    public BoxInt Shift(int x, int y)
        => Shift(new Vec(x, y));

    /// <summary>
    /// Returns a new Box that is resized by the given amount, maintaining the current BottomLeft position.
    /// </summary>
    public BoxInt Resize(Vec amount)
        => new(BottomLeft, TopRight + amount);

    /// <summary>
    /// Returns a new Box that is resized by the given amount, maintaining the current BottomLeft position.
    /// </summary>
    public BoxInt Resize(int amount)
        => Resize(new Vec(amount, amount));
}
