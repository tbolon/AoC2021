using static ProgramHelper;

namespace AoC2021;

internal static class Day05
{

    public static void Part2()
    {
        var lines = Input.GetLines(5).Select(l => l.Split("->").SelectMany(x => x.Trim().Split(',').Select(x => int.Parse(x))).ToArray()).Select(a => new { X1 = a[0], Y1 = a[1], X2 = a[2], Y2 = a[3] }).ToArray();

        var maxX = lines.Max(x => Math.Max(x.X1, x.X2));
        var maxY = lines.Max(x => Math.Max(x.Y1, x.Y2));

        var grid = Enumerable.Range(0, maxX + 1).Select(i => new int[maxY + 1]).ToArray();

        var overlapCount = 0;
        foreach (var line in lines)
        {
            var xIncrement = (line.X1 == line.X2) ? 0 : ((line.X1 < line.X2) ? 1 : -1);
            var yIncrement = (line.Y1 == line.Y2) ? 0 : ((line.Y1 < line.Y2) ? 1 : -1);
            var size = Math.Max(Math.Abs(line.X1 - line.X2), Math.Abs(line.Y1 - line.Y2));

            for (int i = 0; i <= size; i++)
            {
                var x = line.X1 + (i * xIncrement);
                var y = line.Y1 + (i * yIncrement);

                var current = grid[x][y];
                if (current == 1)
                {
                    overlapCount++;
                }

                grid[x][y] = current + 1;
            }
        }

        WriteLine(overlapCount);
    }
    public static void Part1()
    {
        var lines = Input.GetLines(5).Select(l => l.Split("->").SelectMany(x => x.Trim().Split(',').Select(x => int.Parse(x))).ToArray()).Select(a => new { X1 = a[0], Y1 = a[1], X2 = a[2], Y2 = a[3] }).ToArray();

        var maxX = lines.Max(x => Math.Max(x.X1, x.X2));
        var maxY = lines.Max(x => Math.Max(x.Y1, x.Y2));

        var grid = Enumerable.Range(0, maxX + 1).Select(i => new int[maxY + 1]).ToArray();

        var overlapCount = 0;
        foreach (var line in lines)
        {
            if (line.X1 == line.X2)
            {
                var startY = Math.Min(line.Y1, line.Y2);
                var endY = Math.Max(line.Y1, line.Y2);
                for (var y = startY; y <= endY; y++)
                {
                    if (grid[line.X1][y] == 1)
                    {
                        overlapCount++;
                    }

                    grid[line.X1][y]++;

                }
            }
            else if (line.Y1 == line.Y2)
            {
                var startX = Math.Min(line.X1, line.X2);
                var endX = Math.Max(line.X1, line.X2);
                for (var x = startX; x <= endX; x++)
                {
                    if (grid[x][line.Y1] == 1)
                    {
                        overlapCount++;
                    }

                    grid[x][line.Y1]++;

                }
            }
        }

        WriteLine(overlapCount);
    }
}
