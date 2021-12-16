using static ProgramHelper;

namespace AoC2021;

internal static class Day13
{
    public static void Part2()
    {
        var lines = Input.GetLines(13, sample: false);
        var coords = lines.Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("fold along")).Select(x => x.Split(',').Select(int.Parse).ToArray());
        var operations = lines.Where(l => !string.IsNullOrEmpty(l) && l.StartsWith("fold along")).Select(l => new
        {
            Vertical = l.StartsWith("fold along x="),
            Coordinate = int.Parse(l["fold along x=".Length..])
        }).ToArray();

        var grid = new Grid<bool>(coords.Max(x => x[0]) + 1, coords.Max(x => x[1] + 1));

        foreach (var point in coords)
        {
            grid[point[0], point[1]] = true;
        }

        foreach (var op in operations)
        {
            // new grid after folding
            var newGrid = new Grid<bool>(op.Vertical ? op.Coordinate : grid.Width, !op.Vertical ? op.Coordinate : grid.Height);

            // copy existing values
            foreach (var (point, value) in newGrid)
            {
                newGrid[point] = grid[point];
            }

            // add folded values
            for (int x = op.Vertical ? op.Coordinate : 0; x < grid.Width; x++)
            {
                for (int y = !op.Vertical ? op.Coordinate : 0; y < grid.Height; y++)
                {
                    if (grid[x, y])
                    {
                        var newPoint = new Point(
                            op.Vertical ? 2 * op.Coordinate - x : x,
                            !op.Vertical ? 2 * op.Coordinate - y : y
                            );

                        newGrid[newPoint] = true;
                    }
                }
            }

            grid = newGrid;
        }

        DrawGrid(grid);
    }

    public static void Part1()
    {
        var lines = Input.GetLines(13, sample: false);
        var coords = lines.Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("fold along")).Select(x => x.Split(',').Select(int.Parse).ToArray());
        var operations = lines.Where(l => !string.IsNullOrEmpty(l) && l.StartsWith("fold along")).Select(l => new
        {
            Vertical = l.StartsWith("fold along x="),
            Coordinate = int.Parse(l["fold along x=".Length..])
        }).ToArray();

        var grid = new Grid<bool>(coords.Max(x => x[0]) + 1, coords.Max(x => x[1] + 1));

        foreach (var point in coords)
        {
            grid[point[0], point[1]] = true;
        }

        foreach (var op in operations)
        {
            // new grid after folding
            var newGrid = new Grid<bool>(op.Vertical ? op.Coordinate : grid.Width, !op.Vertical ? op.Coordinate : grid.Height);

            // copy existing values
            foreach (var (point, value) in newGrid)
            {
                newGrid[point] = grid[point];
            }

            // add folded values
            for (int x = op.Vertical ? op.Coordinate : 0; x < grid.Width; x++)
            {
                for (int y = !op.Vertical ? op.Coordinate : 0; y < grid.Height; y++)
                {
                    if (grid[x, y])
                    {
                        var newPoint = new Point(
                            op.Vertical ? 2 * op.Coordinate - x : x,
                            !op.Vertical ? 2 * op.Coordinate - y : y
                            );

                        newGrid[newPoint] = true;
                    }
                }
            }

            break; // part 1
        }

        WriteLine(grid.Sum(x => x.value ? 1 : 0));
    }

    static void DrawGrid(Grid<bool> grid, bool? vertical = null, int coordinate = 0)
    {
        Clear();
        grid.VisitConsole(x => Write(x ? '#' : '.'));
        if (vertical != null)
        {
            if (vertical == true)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    SetCursorPosition(coordinate, y);
                    Write('|');
                }
            }
            else
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    SetCursorPosition(x, coordinate);
                    Write('-');
                }

            }
        }
    }
}
