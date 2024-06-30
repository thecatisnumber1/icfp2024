using Lib;
using Point = Lib.PointInt;

namespace Spaceship;

public class VectorSolver
{
    private SpaceshipProblem Problem { get; }
    private Ship Ship { get; }

    public VectorSolver(SpaceshipProblem problem)
    {
        Problem = problem;
        Ship = new Ship();
    }

    public List<int> Solve()
    {
        List<int> solution = [];

        while (!Step(solution)){ }

        return solution;
    }

    private bool Step(List<int> solution)
    {
        int command = BestCommand();

        Ship.Move(command);
        solution.Add(command);
        Problem.TargetPoints.Remove(Ship.Pos);

        return Problem.TargetPoints.Count == 0;
    }

    private int BestCommand()
    {
        var currentPos = Ship.Pos;
        var currentVec = Ship.Velocity;

        var candidateVecs = Command.GetCandidates(currentVec).Select(x => x.Normalized());

        var targetVecs = Problem.TargetPoints
            .Select(p => new Vec(p.X - currentPos.X, p.Y - currentPos.Y).Normalized());

        double tolerance = 0.01;
        var candidateVecCounts = candidateVecs
            .Select(v => targetVecs.Count(tv => tv.IsClose(v, tolerance)));

        while (candidateVecCounts.All(x => x <= 0))
        {
            tolerance *= 2;
            candidateVecCounts = candidateVecs
                .Select(v => targetVecs.Count(tv => tv.IsClose(v, tolerance)));
        }

        int max = candidateVecCounts.Max();
        int command = Array.FindIndex(candidateVecCounts.ToArray(), c => c == max) + 1;

        return command;
    }
}
