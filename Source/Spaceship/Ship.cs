using Lib;
using Point = Lib.PointInt;

namespace Spaceship;

public class Ship
{


    public Point Pos { get; private set; }
    /// <summary>
    /// This is double-precision to represent an integer velocity.
    /// If this is an issue, use the VX and VY properties instead.
    /// </summary>
    public Vec Velocity { get; private set; }

    public int X { get { return Pos.X; } }
    public int Y { get { return Pos.Y; } }
    public int VX { get { return (int)Math.Round(Velocity.X); } }
    public int VY { get { return (int)Math.Round(Velocity.Y); } }

    public Ship()
    {
        Pos = Point.ORIGIN;
        Velocity = Vec.ZERO;
    }

    public void Move(int command)
    {
        Velocity += Command.GetVec(command);
        Pos += Velocity;
    }
}