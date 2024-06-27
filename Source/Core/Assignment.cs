using Lib;
using Point = Lib.PointInt;
namespace Core;
 
/// <summary>
/// Assignment is the primary entrypoint of the Core library.
/// The Core library controls the state of the thing being solved.
/// It contains the current state of the game or problem, and the logic for updating that state.
/// In this dumb example for the Template, the task is to guess a point in 2D space.
/// </summary>
public class Assignment
{
    private readonly Point targetPoint;
    private readonly ILogger logger;

    public int Height { get; }
    public int Width { get; }
    public bool IsSolved { get; private set; }
    public Point PreviousGuess { get; private set; }

    public Assignment(int width, int height, ILogger logger)
    {
        this.logger = logger;
        var r = new Random();
        targetPoint = new Point(r.Next(width), r.Next(height));
        Height = height;
        Width = width;
    }

    public Vec Guess(int x, int y)
    {
        PreviousGuess = new Point(x, y);
        Vec diff = targetPoint - new Point(x, y);
        Vec result = new(diff.X > 0 ? 1 : diff.X < 0 ? -1 : 0, diff.Y > 0 ? 1 : diff.Y < 0 ? -1 : 0);
        
        if (result == Vec.ZERO)
        {
            logger.LogMessage("You guessed the point!");
            IsSolved = true;
        }
        
        return new Vec(diff.X > 0 ? 1 : diff.X < 0 ? -1 : 0, diff.Y > 0 ? 1 : diff.Y < 0 ? -1 : 0);
    }
}