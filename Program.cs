using static System.Diagnostics.Debug;

Setup();

Day07();
//Day07_Part2();

#pragma warning disable CS8321

void Day07_Part2()
{
    var crabs = GetInputLines(7).First().Split(',').Select(int.Parse).ToList();
    crabs.Sort();

    var median = crabs[crabs.Count / 2];

    var x = median;
    var fuel = GetFuel(crabs, x);
    var increment = fuel < GetFuel(crabs, x + 1) ? -1 : 1;

    while (true)
    {
        var newX = x + increment;
        var newFuel = GetFuel(crabs, newX);

        if (newFuel > fuel)
        {
            WriteLine($"{x} => {fuel}");
            return;
        }

        x = newX;
        fuel = newFuel;
    }

    int GetFuel(IEnumerable<int> list, int index) => list.Sum(val => Enumerable.Range(1, Math.Abs(val - index)).Sum());
}

void Day07()
{
    var crabs = GetInputLines(7).First().Split(',').Select(int.Parse).ToList();
    crabs.Sort();

    var median = crabs[crabs.Count / 2];
    var dist = crabs.Sum(x => Math.Abs(x - median));

    WriteLine($"{median} => {dist}");
}

void Day06_Part2()
{
    var fishes = GetInputLines(6).First().Split(',').Select(x => int.Parse(x)).ToList();

    var timers = Enumerable.Repeat<long>(0, 10).ToList();

    foreach (var fish in fishes)
        timers[fish]++;

    for (int i = 0; i < 256; i++)
    {
        var day = timers[0];
        timers.RemoveAt(0);

        while (timers.Count < 9) timers.Add(0);

        timers[6] += day;
        timers[8] += day;
    }

    WriteLine(timers.Sum());
}

void Day06()
{
    var fishs = GetInputLines(6).First().Split(',').Select(x => int.Parse(x)).ToList();

    for (int i = 0; i < 80; i++)
    {
        var size = fishs.Count;
        for (var j = 0; j < size; j++)
        {
            var timer = fishs[j];
            if (timer == 0)
            {
                fishs[j] = 6;
                fishs.Add(8);
            }
            else
            {
                fishs[j] = timer - 1;
            }
        }
    }

    WriteLine(fishs.Count);
}

void Day05_Part2()
{
    var lines = GetInputLines(5).Select(l => l.Split("->").SelectMany(x => x.Trim().Split(',').Select(x => int.Parse(x))).ToArray()).Select(a => new { X1 = a[0], Y1 = a[1], X2 = a[2], Y2 = a[3] }).ToArray();

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

void Day05()
{
    var lines = GetInputLines(5).Select(l => l.Split("->").SelectMany(x => x.Trim().Split(',').Select(x => int.Parse(x))).ToArray()).Select(a => new { X1 = a[0], Y1 = a[1], X2 = a[2], Y2 = a[3] }).ToArray();

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
