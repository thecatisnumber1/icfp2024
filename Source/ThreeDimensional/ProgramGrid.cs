using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensional
{
    public class ProgramGrid
    {
        public List<List<Cell>> Grid { get; set; }

        public ProgramGrid()
        {
            Grid = new List<List<Cell>>();
        }

        public static ProgramGrid Parse(string input, int? aValue = null, int? bValue = null)
        {
            var programGrid = new ProgramGrid();
            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var row = new List<Cell>();
                var tokens = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var token in tokens)
                {
                    row.Add(ParseCell(token.Trim(), aValue, bValue));
                }

                if (row.Count > 0)  // Only add non-empty rows
                {
                    programGrid.Grid.Add(row);
                }
            }

            return programGrid;
        }

        public override string ToString()
        {
            if (Grid == null || Grid.Count == 0)
            {
                return "Empty Grid";
            }

            var sb = new StringBuilder();
            int maxX = Grid.Max(row => row.Count);
            int maxY = Grid.Count;

            // Add X-axis coordinates
            sb.Append("   "); // Padding for Y-axis
            for (int x = 0; x < maxX; x++)
            {
                sb.Append($"{x,3}");
            }
            sb.AppendLine();

            // Add horizontal separator
            sb.Append("  +" + new string('-', maxX * 3 + 1));
            sb.AppendLine();

            // Add grid content with Y-axis coordinates
            for (int y = 0; y < maxY; y++)
            {
                sb.Append($"{y,2}|");
                for (int x = 0; x < maxX; x++)
                {
                    if (x < Grid[y].Count)
                    {
                        sb.Append($"{Grid[y][x],3}");
                    }
                    else
                    {
                        sb.Append("   ");
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static Cell ParseCell(string token, int? aValue, int? bValue)
        {
            if (token == ".")
            {
                return new Cell { Type = CellType.Empty };
            }

            if (int.TryParse(token, out int intValue) && intValue >= -99 && intValue <= 99)
            {
                return new Cell { Type = CellType.Integer, IntegerValue = intValue };
            }

            if (token.Length == 1 && "<>^v+-*/%@=#SAB".Contains(token[0]))
            {
                if (token[0] == 'A' && aValue.HasValue)
                {
                    return new Cell { Type = CellType.Integer, IntegerValue = aValue.Value };
                }
                else if (token[0] == 'B' && bValue.HasValue)
                {
                    return new Cell { Type = CellType.Integer, IntegerValue = bValue.Value };
                }
                else
                {
                    return new Cell { Type = CellType.Operator, OperatorValue = token[0] };
                }
            }

            throw new ArgumentException($"Invalid token: {token}");
        }
    }
}

