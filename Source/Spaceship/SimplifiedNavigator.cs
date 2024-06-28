using System;
using System.Collections.Generic;

namespace Spaceship
{
    public class SimplifiedNavigator
    {
        public List<int> FindPath(int startX, int startY, int targetX, int targetY)
        {
            return CombineMoves(GetAccelList(startX, targetX), GetAccelList(startY, targetY));
        }

        private List<int> GetAccelList(int start, int target)
        {
            var moves = new List<int>();

            if (start > target)
            {
                GenerateAccelList(target, 0, start, moves);
                List<int> movesReversed = new List<int>();
                foreach (var move in moves)
                {
                    movesReversed.Add(-move);
                }

                return movesReversed;
            }

            GenerateAccelList(start, 0, target, moves);
            return moves;
        }

        private int GenerateAccelList(int p, int velocity, int target, List<int> path)
        {
            // We're only getting called to see if there's any room to accelerate.
            // The caller above us can handle coasting!  To know if we can accelerate,
            // we need to think about the remaining distance to the target.
            if (p + velocity + 1 <= target)
            {
                // We can speed up!
                int nextV = velocity + 1;
                path.Add(1);
                int currentP = p + nextV;
                currentP = GenerateAccelList(currentP, nextV, target - nextV, path);

                while (currentP + nextV <= target)
                {
                    path.Add(0);
                    currentP += nextV;
                }

                path.Add(-1);
                currentP += velocity;

                return currentP;
            }

            return p;
        }

        private List<int> CombineMoves(List<int> xMoves, List<int> yMoves)
        {
            var combinedMoves = new List<int>();
            int xIndex = 0, yIndex = 0;

            while (xIndex < xMoves.Count || yIndex < yMoves.Count)
            {
                int xMove = xIndex < xMoves.Count ? xMoves[xIndex] : 0;
                int yMove = yIndex < yMoves.Count ? yMoves[yIndex] : 0;

                combinedMoves.Add(ConvertToCommand(xMove, yMove));

                if (xIndex < xMoves.Count) xIndex++;
                if (yIndex < yMoves.Count) yIndex++;
            }

            return combinedMoves;
        }

        private int ConvertToCommand(int xMove, int yMove)
        {
            int x = xMove + 1;
            int y = yMove + 1;
            return y * 3 + x + 1;
        }
    }
}