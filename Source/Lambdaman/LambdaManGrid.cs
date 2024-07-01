using Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LambdaMan
{
    public class LambdaManGrid
    {
        public char[,] Grid { get; private set; }
        public (int X, int Y) StartPosition { get; private set; }
        public List<(int X, int Y)> Pills { get; private set; }
        public int Width => Grid.GetLength(1);
        public int Height => Grid.GetLength(0);
        public string Name { get; private set; }
        public int Number { get; private set; }

        public LambdaManGrid(string name, List<string> input)
        {
            Name = name;
            Number = int.Parse(name.Replace("lambdaman", ""));
            ParseInput(input);
        }

        private LambdaManGrid(string name, char[,] grid, (int X, int Y) startPosition, List<(int X, int Y)> pills)
        {
            Name = name;
            Number = int.Parse(name.Replace("lambdaman", ""));
            Grid = grid;
            StartPosition = startPosition;
            Pills = pills;
        }

        public Str SolvePrefix()
        {
            return Shorthand.S($"solve {Name} ");
        }

        private void ParseInput(List<string> prefiltered)
        {
            List<string> input = prefiltered.Where(x => !x.Equals("")).ToList();
            int height = input.Count;
            int width = input[0].Length;

            Grid = new char[height, width];
            Pills = new List<(int, int)>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Grid[y, x] = input[y][x];
                    switch (Grid[y, x])
                    {
                        case 'L':
                            StartPosition = (x, y);
                            break;
                        case '.':
                            Pills.Add((x, y));
                            break;
                    }
                }
            }
        }

        public void Display()
        {
            Console.Clear();
            Console.WriteLine(this);
        }

        public LambdaManGrid Duplicate()
        {
            var height = Height;
            var width = Width;

            var newGrid = new char[height, width];
            var newPills = new List<(int, int)>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newGrid[y, x] = Grid[y, x];
                    if (Grid[y, x] == '.')
                    {
                        newPills.Add((x, y));
                    }
                }
            }

            return new LambdaManGrid(Name, newGrid, StartPosition, newPills);
        }

        public bool IsWall(int x, int y)
        {
            return x < 0 || x >= Width || y < 0 || y >= Height || Grid[y, x] == '#';
        }

        public List<(int X, int Y)> GetAdjacentPositions(int x, int y)
        {
            var directions = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
            return directions
                .Select(d => (X: x + d.Item1, Y: y + d.Item2))
                .Where(p => !IsWall(p.X, p.Y))
                .ToList();
        }

        public char GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException();
            return Grid[y, x];
        }

        public void SetCell(int x, int y, char value)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException();
            Grid[y, x] = value;
        }

        public override string ToString()
        {
            var rows = new List<string>();
            for (int y = 0; y < Height; y++)
            {
                var row = new string(Enumerable.Range(0, Width).Select(x => Grid[y, x]).ToArray());
                rows.Add(row);
            }
            return string.Join("\n", rows);
        }

        public void DisplayClose((int x, int y) pos)
        {
            Console.Clear();

            var rows = new List<string>();
            for (int y = Math.Max(0, pos.y - 25); y < Math.Min(Height, pos.y + 25); y++)
            {
                var row = new string(Enumerable.Range(Math.Max(0, pos.x - 25), Math.Min(Width-1, pos.x + 25)).Select(x => Grid[y, x]).ToArray());
                rows.Add(row);
            }
            Console.WriteLine(string.Join("\n", rows));
        }
    }
}
