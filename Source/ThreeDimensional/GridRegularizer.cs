using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ThreeDimensional
{
    public static class GridRegularizer
    {
        public static void RegularizeGridFile(string filePath)
        {
            // Read the entire file content
            string fileContent = File.ReadAllText(filePath);

            // Split metadata and grid content
            var (metadata, gridContent) = SplitMetadataAndGrid(fileContent);

            // Regularize the grid
            string regularizedGrid = RegularizeGrid(gridContent);

            // Combine metadata and regularized grid
            string result = CombineMetadataAndGrid(metadata, regularizedGrid);

            // Write the result back to the file
            File.WriteAllText(filePath, result);
        }

        private static (string metadata, string gridContent) SplitMetadataAndGrid(string fileContent)
        {
            var lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int separatorIndex = Array.IndexOf(lines, "---");

            if (separatorIndex != -1)
            {
                string metadata = string.Join(Environment.NewLine, lines.Take(separatorIndex));
                string gridContent = string.Join(Environment.NewLine, lines.Skip(separatorIndex + 1).Where(line => !string.IsNullOrWhiteSpace(line)));
                return (metadata, gridContent);
            }

            return (string.Empty, fileContent);
        }

        private static string RegularizeGrid(string gridContent)
        {
            var lines = gridContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
            {
                return "Empty Grid";
            }

            var grid = lines.Select(line => line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).ToList();
            int maxY = grid.Count;
            int maxX = grid.Max(row => row.Length);

            // Calculate the maximum width and unique widths for each column
            int[] columnWidths = new int[maxX];
            HashSet<int>[] uniqueWidths = new HashSet<int>[maxX];
            for (int x = 0; x < maxX; x++)
            {
                uniqueWidths[x] = new HashSet<int>();
                for (int y = 0; y < maxY; y++)
                {
                    if (x < grid[y].Length)
                    {
                        int cellWidth = grid[y][x].Length;
                        columnWidths[x] = Math.Max(columnWidths[x], cellWidth);
                        uniqueWidths[x].Add(cellWidth);
                    }
                }
            }

            var sb = new StringBuilder();

            // Build the regularized grid
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (x < grid[y].Length)
                    {
                        string cellValue = grid[y][x];
                        int cellWidth = cellValue.Length;
                        int columnWidth = columnWidths[x];

                        if (uniqueWidths[x].Count <= 2)
                        {
                            // Right-align for columns with one or two unique widths
                            sb.Append(cellValue.PadLeft(columnWidth));
                        }
                        else
                        {
                            // Middle-align for columns with more than two unique widths
                            int leftPadding = (columnWidth - cellWidth) / 2;
                            int rightPadding = columnWidth - cellWidth - leftPadding;
                            sb.Append(new string(' ', leftPadding))
                              .Append(cellValue)
                              .Append(new string(' ', rightPadding));
                        }
                    }
                    else
                    {
                        sb.Append(new string(' ', columnWidths[x]));
                    }

                    // Add a space between columns, except for the last column
                    if (x < maxX - 1)
                    {
                        sb.Append(' ');
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }

        private static string CombineMetadataAndGrid(string metadata, string grid)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(metadata))
            {
                sb.AppendLine(metadata);
                sb.AppendLine("---");
                sb.AppendLine();
            }

            sb.Append(grid);

            return sb.ToString();
        }
    }
}