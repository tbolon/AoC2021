using static ProgramHelper;

namespace AoC2021;

internal static class Day04
{
    public static void Part2()
    {
        var gridSize = 5;
        var lines = Input.GetLines(4).Where(l => !string.IsNullOrEmpty(l)).ToArray();
        var numbers = lines[0].Split(',').Select(x => int.Parse(x)).ToArray();
        var grids = lines.Skip(1).Select(l => l.Trim().Split(' ', options: StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray()).Chunk(gridSize).ToArray();
        var bands = Enumerable.Range(0, grids.Length).Select(x => new int[10]).ToArray();
        var finishedGrids = new List<int>();

        int winningGridIndex = -1;
        int winningNumber = -1;
        foreach (var number in numbers)
        {
            winningGridIndex = CheckGrids(grids, bands, number, finishedGrids);
            if (winningGridIndex != -1)
            {
                winningNumber = number;
                break;
            }
        }

        Assert(winningGridIndex != -1, "Une grille aurait du être trouvée");

        var playedNumbers = numbers.TakeWhile(n => n != winningNumber).Append(winningNumber).ToHashSet();

        var winningGrid = grids[winningGridIndex];

        var unmarked = 0;
        for (var x = 0; x < gridSize; x++)
        {
            for (var y = 0; y < gridSize; y++)
            {
                if (!playedNumbers.Contains(winningGrid[x][y]))
                {
                    unmarked += winningGrid[x][y];
                }
            }
        }

        WriteLine(unmarked * winningNumber);

        int CheckGrids(int[][][] grids, int[][] bands, int n, List<int> finishedGrids)
        {
            for (var i = 0; i < grids.Length; i++)
            {
                if (finishedGrids.Contains(i))
                {
                    continue;
                }

                var grid = grids[i];
                var gridBands = bands[i];

                for (var x = 0; x < gridSize; x++)
                {
                    for (var y = 0; y < gridSize; y++)
                    {
                        if (grid[x][y] == n)
                        {
                            gridBands[x]++;
                            gridBands[gridSize + y]++;

                            if (gridBands[x] == 5 || gridBands[gridSize + y] == 5)
                            {
                                finishedGrids.Add(i);
                                if (finishedGrids.Count == grids.Length)
                                {
                                    return i;
                                }
                            }
                        }
                    }
                }
            }

            return -1;
        }

    }

    public static void Part1()
    {
        var gridSize = 5;
        var lines = Input.GetLines(4).Where(l => !string.IsNullOrEmpty(l)).ToArray();
        var numbers = lines[0].Split(',').Select(x => int.Parse(x)).ToArray();
        var grids = lines.Skip(1).Select(l => l.Trim().Split(' ', options: StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray()).Chunk(gridSize).ToArray();
        var bands = Enumerable.Range(0, grids.Length).Select(x => new int[10]).ToArray();

        int winningGridIndex = -1;
        int winningNumber = -1;
        foreach (var number in numbers)
        {
            winningGridIndex = CheckGrids(grids, bands, number);
            if (winningGridIndex != -1)
            {
                winningNumber = number;
                break;
            }
        }

        Assert(winningGridIndex != -1, "Une grille aurait du être trouvée");

        var playedNumbers = numbers.TakeWhile(n => n != winningNumber).Append(winningNumber).ToHashSet();

        var winningGrid = grids[winningGridIndex];

        var unmarked = 0;
        for (var x = 0; x < gridSize; x++)
        {
            for (var y = 0; y < gridSize; y++)
            {
                if (!playedNumbers.Contains(winningGrid[x][y]))
                {
                    unmarked += winningGrid[x][y];
                }
            }
        }

        WriteLine(unmarked * winningNumber);

        int CheckGrids(int[][][] grids, int[][] bands, int n)
        {
            for (var i = 0; i < grids.Length; i++)
            {
                var grid = grids[i];
                var gridBands = bands[i];

                for (var x = 0; x < gridSize; x++)
                {
                    for (var y = 0; y < gridSize; y++)
                    {
                        if (grid[x][y] == n)
                        {
                            gridBands[x]++;
                            gridBands[gridSize + y]++;

                            if (gridBands[x] == 5 || gridBands[gridSize + y] == 5)
                            {
                                return i;
                            }
                        }
                    }
                }
            }

            return -1;
        }

    }
}
