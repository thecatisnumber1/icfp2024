using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Numerics;

namespace ThreeDimensional
{
    public class ProgramGrid
    {
        public List<List<Cell>> Grid { get; set; }
        public Dictionary<string, JsonElement> Metadata { get; set; }
        private Dictionary<char, (int x, int y)> symbolLocations;

        public ProgramGrid()
        {
            Grid = new List<List<Cell>>();
            Metadata = new Dictionary<string, JsonElement>();
            symbolLocations = new Dictionary<char, (int x, int y)>();
        }

        public static ProgramGrid Parse(string input, int? aValue = null, int? bValue = null)
        {
            var programGrid = new ProgramGrid();
            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Parse metadata
            int metadataEndIndex = Array.FindIndex(lines, line => line.Trim() == "---");
            if (metadataEndIndex != -1)
            {
                string metadataJson = string.Join("\n", lines.Take(metadataEndIndex));
                programGrid.Metadata = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(metadataJson);
                lines = lines.Skip(metadataEndIndex + 1).ToArray();
            }

            // Parse grid
            for (int y = 0; y < lines.Length; y++)
            {
                var row = new List<Cell>();
                var tokens = lines[y].Trim().Split(new[] { ' ', '	' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < tokens.Length; x++)
                {
                    var (cell, symbol) = programGrid.ParseCell(tokens[x].Trim(), aValue, bValue);
                    row.Add(cell);
                    if (symbol.HasValue)
                    {
                        programGrid.symbolLocations[symbol.Value] = (x, y);
                    }
                }
                if (row.Count > 0)  // Only add non-empty rows
                {
                    programGrid.Grid.Add(row);
                }
            }

            // Process time warps
            programGrid.ProcessTimeWarps();

            return programGrid;
        }

        private (Cell cell, char? symbol) ParseCell(string token, int? aValue, int? bValue)
        {
            var symbolMatch = Regex.Match(token, @"^([a-z])(.+)$");
            if (symbolMatch.Success)
            {
                char symbol = symbolMatch.Groups[1].Value[0];
                string actualToken = symbolMatch.Groups[2].Value;
                return (ParseCellContent(actualToken, aValue, bValue), symbol);
            }
            return (ParseCellContent(token, aValue, bValue), null);
        }

        private Cell ParseCellContent(string token, int? aValue, int? bValue)
        {
            if (token == ".")
            {
                return new Cell { Type = CellType.Empty };
            }
            if (int.TryParse(token, out int intValue) && intValue >= -99 && intValue <= 99)
            {
                return new Cell { Type = CellType.Integer, IntegerValue = intValue };
            }
            if (token.Length == 1)
            {
                char ch = token[0];
                if ("<>^v+-*/%@=#SAB".Contains(ch) || char.IsLower(ch))
                {
                    if (ch == 'A' && aValue.HasValue)
                    {
                        return new Cell { Type = CellType.Integer, IntegerValue = aValue.Value };
                    }
                    else if (ch == 'B' && bValue.HasValue)
                    {
                        return new Cell { Type = CellType.Integer, IntegerValue = bValue.Value };
                    }
                    else
                    {
                        return new Cell { Type = CellType.Operator, OperatorValue = ch };
                    }
                }
            }
            throw new ArgumentException($"Invalid token: {token}");
        }

        public string ToMinimalString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < Grid.Count; y++)
            {
                for (int x = 0; x < Grid[y].Count; x++)
                {
                    Cell cell = Grid[y][x];
                    string cellValue = GetCellValue(cell);
                    sb.Append($"{cellValue,3}");

                    if (x < Grid[y].Count - 1)
                    {
                        sb.Append(" ");
                    }
                }

                if (y < Grid.Count - 1)
                {
                    sb.AppendLine();
                }
            }

            string result = sb.ToString();
            result = result.Replace("\r", "");
            return result;
        }

        private string GetCellValue(Cell cell)
        {
            switch (cell.Type)
            {
                case CellType.Empty:
                    return ".";
                case CellType.Integer:
                    return cell.IntegerValue.ToString();
                case CellType.Operator:
                    return cell.OperatorValue.ToString();
                default:
                    throw new InvalidOperationException($"Unknown cell type: {cell.Type}");
            }
        }

        private void ProcessTimeWarp(int x, int y)
        {
            if (y > 0 && y < Grid.Count - 1 && x > 0 && x < Grid[y].Count - 1)
            {
                var left = Grid[y][x - 1];
                var right = Grid[y][x + 1];
                var up = Grid[y - 1][x];
                var down = Grid[y + 1][x];

                if (left.Type == CellType.Operator && left.OperatorValue.HasValue && char.IsLower(left.OperatorValue.Value))
                {
                    char symbol = left.OperatorValue.Value;
                    if (symbolLocations.TryGetValue(symbol, out var targetLocation))
                    {
                        int dx = x - targetLocation.x;
                        Grid[y][x - 1] = new Cell { Type = CellType.Integer, IntegerValue = dx };
                    }
                }

                if (right.Type == CellType.Operator && right.OperatorValue.HasValue && char.IsLower(right.OperatorValue.Value))
                {
                    char symbol = right.OperatorValue.Value;
                    if (symbolLocations.TryGetValue(symbol, out var targetLocation))
                    {
                        int dy = y - targetLocation.y;
                        Grid[y][x + 1] = new Cell { Type = CellType.Integer, IntegerValue = dy };
                    }
                }
            }
        }

        private void ProcessTimeWarps()
        {
            for (int y = 0; y < Grid.Count; y++)
            {
                for (int x = 0; x < Grid[y].Count; x++)
                {
                    if (Grid[y][x].Type == CellType.Operator && Grid[y][x].OperatorValue == '@')
                    {
                        ProcessTimeWarp(x, y);
                    }
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            List<(int row, int col, BigInteger val)> cellValues = new List<(int row, int col, BigInteger val)>();
            if (Grid == null || Grid.Count == 0)
            {
                return "Empty Grid";
            }

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
                        string cell = $"{Grid[y][x],3}";
                        if (cell.Length > 3)
                        {
                            sb.Append("~~~");
                            cellValues.Add((x, y, Grid[y][x].IntegerValue.Value));
                        }
                        else
                        {
                            sb.Append(cell);
                        }
                    }
                    else
                    {
                        sb.Append("   ");
                    }
                }
                sb.AppendLine();
            }

            foreach (var (row, col, val) in cellValues)
            {
                sb.AppendLine($"Cell at ({col}, {row}) has value {val}");
            }

            return sb.ToString();
        }

        public string MetadataToString()
        {
            if (Metadata.Count == 0)
            {
                return "No Metadata";
            }
            return JsonSerializer.Serialize(Metadata, new JsonSerializerOptions { WriteIndented = true });
        }

        // Helper method to access metadata
        public T GetMetadata<T>(string key, T defaultValue = default)
        {
            if (Metadata.TryGetValue(key, out JsonElement value))
            {
                return JsonSerializer.Deserialize<T>(value.GetRawText());
            }
            return defaultValue;
        }
    }
}