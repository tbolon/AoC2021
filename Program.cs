using static System.Diagnostics.Debug;

Setup();

Day04_Part2();

#pragma warning disable CS8321

void Day04_Part2()
{
    var gridSize = 5;
    var lines = GetInputLines(4).Where(l => !string.IsNullOrEmpty(l)).ToArray();
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

void Day04()
{
    var gridSize = 5;
    var lines = GetInputLines(4).Where(l => !string.IsNullOrEmpty(l)).ToArray();
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

void Day03_Part2()
{
    // string[]
    var lines = GetInputLines(3);

    // byte[lines.Length][12];
    var values = lines
        .Select(l => l.Select(c => c - 48).ToArray())
        .ToArray();

    // 12
    var size = values[0].Length;

    var i = Reduce(true);
    var oxygen = Convert.ToInt32(lines[i], 2);

    i = Reduce(false);
    var co2 = Convert.ToInt32(lines[i], 2);

    WriteLine(oxygen * co2);

    int Reduce(bool majority)
    {
        var eligibles = Enumerable.Range(0, values!.Length).ToList(); // all indexes

        for (var i = 0; i < size; i++)
        {
            if (eligibles.Count <= 1)
                break; // found it (or failed)

            var count1 = eligibles.Sum(x => values[x][i]);
            var count0 = eligibles.Count - count1;

            if (count1 >= count0)
                eligibles = eligibles.Where(e => values[e][i] == (majority ? 1 : 0)).ToList();
            else if (count1 < count0)
                eligibles = eligibles.Where(e => values[e][i] == (majority ? 0 : 1)).ToList();
        }

        Assert(eligibles.Count == 1, eligibles.Count.ToString());

        return eligibles.First();
    }
}

void Day03()
{
    var lines = GetInputLines(3);

    var setBits = new int[12];

    for (int i = 0; i < lines.Length; i++)
    {
        for (int j = 0; j < setBits.Length; j++)
        {
            setBits[j] += lines[i][j] == '1' ? 1 : 0;
        }
    }

    var gammaRate = 0;
    var epsilonRate = 0;
    var median = lines.Length / 2; // 500
    for (int i = 0; i < setBits.Length; i++)
    {
        if (setBits[i] > median)
            gammaRate |= 1 << (setBits.Length - i - 1);
        else
            epsilonRate |= 1 << (setBits.Length - i - 1);
    }

    WriteLine($"Gamma Rate = {gammaRate} ; Epsilon Rate = {epsilonRate} ; Answer = {gammaRate * epsilonRate}");
}

void Day02()
{
    var lines = GetInputLines(2);

    (int horizontal, int depth, int aim) position = (0, 0, 0);

    for (int i = 0; i < lines.Length; i++)
    {
        var parts = lines[i].Split(' ').ToArray();
        var verb = parts[0];
        var offset = int.Parse(parts[1]);

        position = verb switch
        {
            "forward" => (position.horizontal + offset, position.depth + (position.aim * offset), position.aim),
            "down" => (position.horizontal, position.depth, position.aim + offset),
            "up" => (position.horizontal, position.depth, position.aim - offset),
            _ => position
        };
    }

    WriteLine(position.horizontal * position.depth);
}

void Day01()
{
    var lines = GetInputLines(1)
        .Select(l => int.Parse(l.Trim()))
        .ToArray();


    var increasing = 0;
    for (var i = 1; i < lines.Length; i++)
    {
        if (lines[i] > lines[i - 1]) increasing++;
    }

    WriteLine(increasing);
}

void Setup()
{
    System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener()); // redirects Debug.WriteLine() to the console
}

string[] GetInputLines(int day)
    => GetInputFile(day).Split('\n', options: StringSplitOptions.RemoveEmptyEntries).ToArray();

string GetInputFile(int day)
{
    var filename = $"Day{day:00}.txt";
    if (File.Exists(filename))
    {
        return File.ReadAllText(filename);
    }

    var sessionId = Environment.GetEnvironmentVariable("AOC_SESSION");
    if (string.IsNullOrEmpty(sessionId))
        // pwsh: $env:AOC_SESSION = "..."
        throw new InvalidOperationException($"You must set AOC_SESSION environment variable with your AoC session cookie value");

    var cookieContainer = new System.Net.CookieContainer();
    cookieContainer.Add(new System.Net.Cookie("session", sessionId, "/", ".adventofcode.com"));

    using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
    using var client = new HttpClient(handler);

    var text = client.GetStringAsync($"https://adventofcode.com/2021/day/{day}/input").Result;

    File.WriteAllText(filename, text);

    return text;
}
