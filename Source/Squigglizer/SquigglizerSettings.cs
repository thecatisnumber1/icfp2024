namespace Squigglizer;

public record class SquigglizerSettings(
    int WindowX,
    int WindowY,
    int WindowWidth,
    int WindowHeight,
    bool IsFullscreen,
    string? Solver
)
{
}
