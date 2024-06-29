using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeDimensional
{
    public class ProgramRunner
    {
        private List<ProgramGrid> _history;
        private int _currentTick;
        private int? _resultValue;
        private const int MaxTicks = 1_000_000;
        private int _timeWarpTick = -1;
        private bool[,] _written;

        public ProgramRunner(ProgramGrid initialGrid)
        {
            _history = new List<ProgramGrid> { initialGrid };
            _currentTick = 1;
        }

        public bool Step()
        {
            if (_currentTick >= MaxTicks || _resultValue.HasValue)
            {
                return false;
            }

            var currentGrid = _history[_currentTick - 1];
            _written = new bool[currentGrid.Grid.Count, currentGrid.Grid[0].Count];
            var nextGrid = ExecuteTick(currentGrid);

            if (_timeWarpTick != -1)
            {
                // Truncate history and recalculate from the time warp point
                _history = _history.Take(_timeWarpTick).ToList();
                _currentTick = _timeWarpTick;
                _timeWarpTick = -1;
            }
            else
            {
                _history.Add(nextGrid);
                _currentTick++;
            }


            if (_resultValue.HasValue)
            {
                return false;
            }

            return true;
        }

        public void RunToCompletion()
        {
            while (Step()) { }
        }

        public int? GetResult()
        {
            return _resultValue;
        }

        public bool IsCompleted()
        {
            return _resultValue.HasValue || _currentTick >= MaxTicks;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Current Tick: {_currentTick}");
            sb.AppendLine($"Result: {(_resultValue.HasValue ? _resultValue.ToString() : "Not submitted yet")}");
            sb.AppendLine($"Current Grid State:");
            sb.AppendLine(_history.Last().ToString());
            return sb.ToString();
        }

        public int GetSpacetimeVolume()
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            int maxT = _currentTick;

            foreach (var grid in _history)
            {
                for (int y = 0; y < grid.Grid.Count; y++)
                {
                    for (int x = 0; x < grid.Grid[y].Count; x++)
                    {
                        if (grid.Grid[y][x].Type != CellType.Empty)
                        {
                            minX = Math.Min(minX, x);
                            maxX = Math.Max(maxX, x);
                            minY = Math.Min(minY, y);
                            maxY = Math.Max(maxY, y);
                        }
                    }
                }
            }

            return (maxX - minX + 1) * (maxY - minY + 1) * maxT;
        }

        private ProgramGrid ExecuteTick(ProgramGrid currentGrid)
        {
            var nextGrid = new ProgramGrid();
            nextGrid.Grid = currentGrid.Grid.Select(row => row.Select(cell => cell.Clone()).ToList()).ToList();

            var operations = new List<Action>();

            for (int y = 0; y < currentGrid.Grid.Count; y++)
            {
                for (int x = 0; x < currentGrid.Grid[y].Count; x++)
                {
                    var cell = currentGrid.Grid[y][x];
                    if (cell.Type == CellType.Operator)
                    {
                        int capturedX = x;
                        int capturedY = y;
                        switch (cell.OperatorValue)
                        {
                            case '<':
                                operations.Add(() => MoveLeft(currentGrid, nextGrid, capturedX, capturedY));
                                break;
                            case '>':
                                operations.Add(() => MoveRight(currentGrid, nextGrid, capturedX, capturedY));
                                break;
                            case '^':
                                operations.Add(() => MoveUp(currentGrid, nextGrid, capturedX, capturedY));
                                break;
                            case 'v':
                                operations.Add(() => MoveDown(currentGrid, nextGrid, capturedX, capturedY));
                                break;
                            case '+':
                            case '-':
                            case '*':
                            case '/':
                            case '%':
                                if (HasTwoAdjacentValues(currentGrid, capturedX, capturedY))
                                    operations.Add(() => PerformArithmetic(currentGrid, nextGrid, capturedX, capturedY, cell.OperatorValue.Value));
                                break;
                            case '=':
                                if (HasTwoAdjacentValues(currentGrid, capturedX, capturedY))
                                    operations.Add(() => Equal(currentGrid, nextGrid, capturedX, capturedY));
                                break;
                            case '#':
                                if (HasTwoAdjacentValues(currentGrid, capturedX, capturedY))
                                    operations.Add(() => NotEqual(currentGrid, nextGrid, capturedX, capturedY));
                                break;
                            case '@':
                                if (HasTimeWarpOperands(currentGrid, capturedX, capturedY))
                                    operations.Add(() => TimeWarp(nextGrid, capturedX, capturedY));
                                break;
                                // 'A', 'B', and 'S' don't have specific actions in ExecuteTick
                        }
                    }
                }
            }

            // Execute all operations
            foreach (var operation in operations)
            {
                operation();
            }

            return nextGrid;
        }

        private void PerformArithmetic(ProgramGrid currentGrid, ProgramGrid nextGrid, int x, int y, char operation)
        {
            if (TryGetOperands(currentGrid, x, y, out int left, out int top))
            {
                int result = 0;
                switch (operation)
                {
                    case '+': result = left + top; break;
                    case '-': 
                        result = left - top; 
                        break;
                    case '*': result = left * top; break;
                    case '/': result = left / top; break;
                    case '%': result = left % top; break;
                }

                if (!_written[y, x -1])
                {
                    nextGrid.Grid[y][x - 1] = new Cell { Type = CellType.Empty };
                }
                SetResult(nextGrid, x + 1, y, result);

                if (!_written[y - 1, x])
                {
                    nextGrid.Grid[y - 1][x] = new Cell { Type = CellType.Empty };
                }
                SetResult(nextGrid, x, y + 1, result);
            }
        }

        private bool HasTwoAdjacentValues(ProgramGrid grid, int x, int y)
        {
            return HasAdjacentValue(grid, x - 1, y) && HasAdjacentValue(grid, x, y - 1);
        }

        private bool HasAdjacentValue(ProgramGrid grid, int x, int y)
        {
            return IsValidPosition(grid, x, y) && grid.Grid[y][x].Type == CellType.Integer;
        }

        private bool HasTimeWarpOperands(ProgramGrid grid, int x, int y)
        {
            return HasAdjacentValue(grid, x - 1, y) && // dx
                   HasAdjacentValue(grid, x + 1, y) && // dy
                   HasAdjacentValue(grid, x, y + 1) && // dt
                   IsValidPosition(grid, x, y - 1) && grid.Grid[y - 1][x].Type != CellType.Empty; // v
        }

        private bool IsValidPosition(ProgramGrid grid, int x, int y)
        {
            return y >= 0 && y < grid.Grid.Count && x >= 0 && x < grid.Grid[y].Count;
        }

        private void MoveLeft(ProgramGrid current, ProgramGrid next, int x, int y)
        {
            if (_written[y, x - 1])
            {
                throw new Exception("Writing multiple values to the same cell");
            }

            _written[y, x - 1] = true;
            if (x > 0 && current.Grid[y][x + 1].Type != CellType.Empty)
            {
                if (next.Grid[y][x - 1].OperatorValue == 'S')
                {
                    _resultValue = current.Grid[y][x + 1].IntegerValue;
                }

                next.Grid[y][x - 1] = current.Grid[y][x + 1];
                if (!_written[y, x + 1])
                {
                    next.Grid[y][x + 1] = new Cell { Type = CellType.Empty };
                }
            }
        }

        private void MoveRight(ProgramGrid current, ProgramGrid next, int x, int y)
        {
            if (_written[y, x + 1])
            {
                throw new Exception("Writing multiple values to the same cell");
            }

            _written[y, x + 1] = true;
            if (x < next.Grid[y].Count - 1 && current.Grid[y][x - 1].Type != CellType.Empty)
            {
                if (next.Grid[y][x + 1].OperatorValue == 'S')
                {
                    _resultValue = current.Grid[y][x - 1].IntegerValue;
                }

                next.Grid[y][x + 1] = current.Grid[y][x - 1];
                if (!_written[y, x - 1])
                {
                    next.Grid[y][x - 1] = new Cell { Type = CellType.Empty };
                }
            }
        }

        private void MoveUp(ProgramGrid current, ProgramGrid next, int x, int y)
        {
            if (_written[y - 1, x])
            {
                throw new Exception("Writing multiple values to the same cell");
            }

            _written[y - 1, x] = true;
            if (y > 0 && current.Grid[y + 1][x].Type != CellType.Empty)
            {
                if (next.Grid[y - 1][x].OperatorValue == 'S')
                {
                    _resultValue = current.Grid[y + 1][x].IntegerValue;
                }

                next.Grid[y - 1][x] = current.Grid[y + 1][x];
                if (!_written[y + 1, x])
                {
                    next.Grid[y + 1][x] = new Cell { Type = CellType.Empty };
                }
            }
        }

        private void MoveDown(ProgramGrid current, ProgramGrid next, int x, int y)
        {
            if (_written[y + 1, x])
            {
                throw new Exception("Writing multiple values to the same cell");
            }

            _written[y + 1, x] = true;
            if (y < next.Grid.Count - 1 && current.Grid[y - 1][x].Type != CellType.Empty)
            {
                if (next.Grid[y + 1][x].OperatorValue == 'S')
                {
                    _resultValue = current.Grid[y - 1][x].IntegerValue;
                }

                next.Grid[y + 1][x] = current.Grid[y - 1][x];
                if (!_written[y - 1, x])
                {
                    next.Grid[y - 1][x] = new Cell { Type = CellType.Empty };
                }
            }
        }

        private void Equal(ProgramGrid currentGrid, ProgramGrid nextGrid, int x, int y)
        {
            if (TryGetOperands(currentGrid, x, y, out int left, out int top) && left == top)
            {
                if (!_written[y, x - 1])
                {
                    nextGrid.Grid[y][x - 1] = new Cell { Type = CellType.Empty };
                }
                SetResult(nextGrid, x + 1, y, left);

                if (!_written[y - 1, x])
                {
                    nextGrid.Grid[y - 1][x] = new Cell { Type = CellType.Empty };
                }
                SetResult(nextGrid, x, y + 1, left);
            }
        }

        private void NotEqual(ProgramGrid currentGrid, ProgramGrid nextGrid, int x, int y)
        {
            if (TryGetOperands(currentGrid, x, y, out int left, out int top) && left != top)
            {
                if (!_written[y, x - 1])
                {
                    nextGrid.Grid[y][x - 1] = new Cell { Type = CellType.Empty };
                }
                SetResult(nextGrid, x + 1, y, top);

                if (!_written[y - 1, x])
                {
                    nextGrid.Grid[y - 1][x] = new Cell { Type = CellType.Empty };
                }
                SetResult(nextGrid, x, y + 1, left);
            }
        }

        private void TimeWarp(ProgramGrid grid, int x, int y)
        {
            if (TryGetTimeWarpOperands(grid, x, y, out int dx, out int dy, out int dt, out Cell v))
            {
                TimeWarp(dt, x - dx, y - dy, v);
            }
        }

        private bool TryGetOperands(ProgramGrid grid, int x, int y, out int left, out int top)
        {
            left = top = 0;
            bool hasLeft = TryGetAdjacentValue(grid, x - 1, y, out left);
            bool hasTop = TryGetAdjacentValue(grid, x, y - 1, out top);
            return hasLeft && hasTop;
        }

        private bool TryGetAdjacentValue(ProgramGrid grid, int x, int y, out int value)
        {
            value = 0;
            if (y >= 0 && y < grid.Grid.Count && x >= 0 && x < grid.Grid[y].Count)
            {
                var cell = grid.Grid[y][x];
                if (cell.Type == CellType.Integer)
                {
                    value = cell.IntegerValue.Value;
                    return true;
                }
            }
            return false;
        }

        private void SetResult(ProgramGrid grid, int x, int y, int result)
        {
            if (_written[y, x])
            {
                throw new Exception("Writing multiple values to the same cell");
            }

            _written[y, x] = true;
            if (grid.Grid[y][x].OperatorValue == 'S')
            {
                _resultValue = result;
            }

            if (x < grid.Grid[y].Count)
            {
                grid.Grid[y][x] = new Cell { Type = CellType.Integer, IntegerValue = result };
            }
        }

        private bool TryGetTimeWarpOperands(ProgramGrid grid, int x, int y, out int dx, out int dy, out int dt, out Cell v)
        {
            dx = dy = dt = 0;
            v = new Cell { Type = CellType.Empty };

            bool hasDx = TryGetAdjacentValue(grid, x - 1, y, out dx);
            bool hasDy = TryGetAdjacentValue(grid, x + 1, y, out dy);
            bool hasDt = TryGetAdjacentValue(grid, x, y + 1, out dt);
            bool hasV = y > 0 && grid.Grid[y - 1][x].Type != CellType.Empty;

            if (hasV)
            {
                v = grid.Grid[y - 1][x].Clone();
                //grid.Grid[y - 1][x] = new Cell { Type = CellType.Empty }; I think this is dumb and not needed, but keeping in case
            }

            return hasDx && hasDy && hasDt && hasV;
        }

        public void TimeWarp(int dt, int x, int y, Cell newValue)
        {
            if (dt <= 0 || dt >= _currentTick)
            {
                throw new ArgumentException("Invalid time warp delta");
            }

            int targetTick = _currentTick - dt;

            if (_timeWarpTick != -1 && _timeWarpTick != targetTick)
            {
                throw new Exception("Multiple time warps to different times are not allowed");
            }

            _timeWarpTick = targetTick;
            var targetGrid = _history[targetTick - 1];

            int targetX = x;
            int targetY = y;

            if (targetY < 0 || targetY >= targetGrid.Grid.Count ||
                targetX < 0 || targetX >= targetGrid.Grid[targetY].Count)
            {
                throw new ArgumentException("Invalid target coordinates for time warp");
            }

            targetGrid.Grid[targetY][targetX] = newValue;
        }
    }
}