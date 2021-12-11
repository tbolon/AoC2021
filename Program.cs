using static ProgramHelper;

Day09_Part2();

#pragma warning disable CS8321

void Day11_Part2()
{
    const byte FLASHING = 10;
    const byte OOB = 0xff;
    var octopus = Input.GetLines(11, sample: false).AsGridOfBytes(OOB);
    var flashes = 0;
    var stepFlashes = 0;
    var defaultColor = Console.ForegroundColor;
    var step = 1;

    DrawGrid();

    while (true)
    {
        stepFlashes = 0;

        foreach (var (point, value) in octopus)
        {
            IncreaseEnergy(point);
        }

        DrawGrid();

        // reset energy
        foreach (var (point, value) in octopus)
        {
            if (value == FLASHING)
                octopus[point] = 0;
        }

        if (stepFlashes >= octopus.Count)
        {
            WriteLine(step);
            return;
        }

        step++;
    }

    void DrawGrid()
    {
        Console.SetCursorPosition(0, 0);
        octopus.VisitConsole(v =>
        {
            if (v == FLASHING) Write("X", ConsoleColor.Green);
            else Write(v);
        });

        WriteLine();
        WriteLine(step);
    }

    void IncreaseEnergy(Point p)
    {
        var value = octopus[p];
        if (value == OOB)
        {
            // out of bounds
        }
        else if (value == FLASHING)
        {
            // already flashed
        }
        else if (value == 9)
        {
            // flash
            octopus[p] = FLASHING;
            stepFlashes++;
            flashes++;

            // release energy
            IncreaseEnergy(p.Right);
            IncreaseEnergy(p.Left);
            IncreaseEnergy(p.RightUp);
            IncreaseEnergy(p.RightDown);
            IncreaseEnergy(p.LeftUp);
            IncreaseEnergy(p.LeftDown);
            IncreaseEnergy(p.Up);
            IncreaseEnergy(p.Down);
        }
        else
        {
            // increase energy
            octopus[p]++;
        }
    }
}

void Day11()
{
    byte[][] octopuses = Input.GetLines(11, sample: false).Select(l => l.Select(x => x).Select(x => (byte)(x - '0')).ToArray()).ToArray();
    var height = octopuses.Length;
    var width = octopuses[0].Length;
    var flashes = 0;
    var stepFlashes = 0;
    var defaultColor = Console.ForegroundColor;

    for (int i = 0; i < 100; i++)
    {
        stepFlashes = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                IncreaseEnergy(x, y);
            }
        }

        // reset energy
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (octopuses[x][y] == 10)
                    octopuses[x][y] = 0;
            }
        }
    }

    WriteLine(flashes);

    void IncreaseEnergy(int x, int y)
    {
        if (x < 0 || y < 0 || x > width - 1 || y > height - 1) return;
        var value = octopuses[x][y];

        if (value == 10)
        {
            // already flashed
        }
        else if (value == 9)
        {
            // flash
            octopuses[x][y] = 10;
            stepFlashes++;
            flashes++;

            // release energy
            IncreaseEnergy(x + 1, y);
            IncreaseEnergy(x - 1, y);
            IncreaseEnergy(x + 1, y + 1);
            IncreaseEnergy(x - 1, y - 1);
            IncreaseEnergy(x + 1, y - 1);
            IncreaseEnergy(x - 1, y + 1);
            IncreaseEnergy(x, y - 1);
            IncreaseEnergy(x, y + 1);
        }
        else
        {
            // increase energy
            octopuses[x][y]++;
        }
    }
}

void Day10_Part2()
{
    var pairs = new char[128];
    pairs['('] = 'x'; // opening => x
    pairs['['] = 'x';
    pairs['{'] = 'x';
    pairs['<'] = 'x';
    pairs[')'] = '('; // closing => opening
    pairs[']'] = '[';
    pairs['}'] = '{';
    pairs['>'] = '<';

    var scoreTable = new byte[128];
    scoreTable['('] = 1;
    scoreTable['['] = 2;
    scoreTable['{'] = 3;
    scoreTable['<'] = 4;

    Dictionary<char, int> count = new();
    List<long> scores = new();

    var lines = Input.GetLines(10, false);

    foreach (var line in lines)
    {
        var c = ExamLine(line, out var score);
        if (c == '\0')
        {
            // incomplete
            scores.Add(score);
        }
        else
        {
            // invalid (part1)
            count[c] = count.TryGetValue(c, out var ccount) ? ccount : 0;
        }
    }

    WriteLine(scores.OrderBy(s => s).ElementAt(scores.Count / 2));

    char ExamLine(string line, out long score)
    {
        score = 0;
        var stack = new Stack<char>();

        foreach (var c in line)
        {
            // detect opening
            if (pairs[c] == 'x')
            {
                stack.Push(c);
                continue;
            }

            // detect invalid (part1)
            var c2 = stack.Pop();
            if (pairs[c] != c2)
            {
                return c;
            }
        }

        // non closed characters: compute score (part2)
        foreach (var c in stack)
        {
            score = score * 5 + scoreTable[c];
        }

        return '\0';
    }
}

void Day10()
{
    var lines = Input.GetLines(10, false);

    Dictionary<char, int> count = new();

    foreach (var line in lines)
    {
        var c = ExamLine(line);
        if (c != '\0')
        {
            // invalid
            count[c] = (count.TryGetValue(c, out var ccount) ? ccount : 0) + 1;
        }
    }

    WriteLine(count[')'] * 3 + count[']'] * 57 + count['}'] * 1197 + count['>'] * 25137);

    char ExamLine(string line)
    {
        var stack = new Stack<char>();

        foreach (var c in line)
        {
            if (c == '(' || c == '[' || c == '{' || c == '<')
            {
                stack.Push(c);
                continue;
            }

            var c2 = stack.Peek();

            if (c == ')' && c2 != '(')
                return c;
            else if (c == ']' && c2 != '[')
                return c;
            else if (c == '}' && c2 != '{')
                return c;
            else if (c == '>' && c2 != '<')
                return c;

            stack.Pop();
        }

        return '\0';
    }
}

void Day09_Part2()
{
    byte[][] data = Input.GetLines(9, sample: false).Select(s => s.Select(c => (byte)(c - '0')).ToArray()).ToArray();
    int height = data.Length, width = data[0].Length;

    // détection bassins ; x = vertical ; y = horizontal
    List<(int x, int y)> points = new();
    for (int x = 0; x < height; x++)
    {
        var line = data[x];
        for (int y = 0; y < width; y++)
        {
            var depth = line[y];

            // détermine si le point est < à tous les voisins
            var lowest = (x <= 0 || data[x - 1][y] > depth)
                && (x >= height - 1 || data[x + 1][y] > depth)
                && (y <= 0 || line[y - 1] > depth)
                && (y >= width - 1 || line[y + 1] > depth);

            if (lowest)
            {
                WriteLine($"({x},{y}) = {depth}");
                points.Add((x, y));
            }
        }
    }

    // grille avec un 0 pour les zones vides, 1 pour les zones déjà découvertes d'un bassin, 0xff pour les bords (hauteur 9)
    byte[][] fill = data.Select(l => l.Select(c => (byte)(c == 9 ? 0xff : 0)).ToArray()).ToArray();

    // examen du bassin
    int[] sizes = new int[points.Count];
    for (int i = 0; i < points.Count; i++)
    {
        sizes[i] = Visit(fill, points[i]);
    }

    // calcul du produit des 3 plus grandes
    // 1045660
    WriteLine(sizes.OrderByDescending(s => s).Take(3).Aggregate(1, (agg, current) => agg * current));

    // examine une case et démarre l'examen de ses voisines
    int Visit(byte[][] fill, (int x, int y) point)
    {
        int x = point.x, y = point.y;
        if (x >= fill.Length || x < 0 || y >= fill[x].Length || y < 0)
            return 0; // out of bounds

        if (fill[x][y] != 0)
            return 0; // invalid

        fill[x][y] = 1; // marquée comme visitée

        return 1 + Visit(fill, (x - 1, y)) + Visit(fill, (x + 1, y)) + Visit(fill, (x, y - 1)) + Visit(fill, (x, y + 1));
    }
}

void Day09()
{
    var grid = Input.GetLines(9, sample: false).AsGridOfBytes(byte.MaxValue);

    var score = grid.Aggregate<int>((point, cumul, value) =>
    {
        if (grid[point.Right] > value && grid[point.Left] > value && grid[point.Up] > value && grid[point.Down] > value)
        {
            return cumul + value + 1;
        }

        return cumul;
    });

    WriteLine(score);
}

void Day08_Part2()
{
    /*
      0:      1:      2:      3:      4:
     aaaa    ....    aaaa    aaaa    ....
    b    c  .    c  .    c  .    c  b    c
    b    c  .    c  .    c  .    c  b    c
     ....    ....    dddd    dddd    dddd
    e    f  .    f  e    .  .    f  .    f
    e    f  .    f  e    .  .    f  .    f
     gggg    ....    gggg    gggg    ....

      5:      6:      7:      8:      9:
     aaaa    aaaa    aaaa    aaaa    aaaa
    b    .  b    .  .    c  b    c  b    c
    b    .  b    .  .    c  b    c  b    c
     dddd    dddd    ....    dddd    dddd
    .    f  e    f  .    f  e    f  .    f
    .    f  e    f  .    f  e    f  .    f
     gggg    gggg    ....    gggg    gggg

    //                234567 abcdefg
    // 0 = 6 segments     x  xxx xxx
    // 1 = 2 segments x        x  x
    // 2 = 5 segments    x   x xxx x
    // 3 = 5 segments    x   x xx xx
    // 4 = 4 segments   x     xxx x
    // 5 = 5 segments    x   xx x xx
    // 6 = 6 segments     x  xx xxxx
    // 7 = 3 segments  x     x x  x
    // 8 = 7 segments      x xxxxxxx
    // 9 = 6 segments     x  xxxx xx
    //                111331 8687497
    */
    var symbols = new List<string> { "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg" };

    var lines = Input.GetLines(8, sample: false).Select(x =>
    {
        var parts = x.Split('|');
        return (digits: parts[0].Trim().Split(' ').Select(SortChars).ToList(), displays: parts[1].Trim().Split(' ').Select(SortChars).ToArray());
    }).ToList();

    var sum = 0L;
    foreach (var line in lines)
    {
        sum += SolveLine(line);
    }

    WriteLine(sum);

    long SolveLine((List<string> digits, string[] displays) line)
    {
        var digitIndexes = Enumerable.Repeat(-1, 10).ToArray();
        var chars = Enumerable.Range('a', 7).Select(x => (char)x).ToArray();
        const int swap_a = 0; const int swap_b = 1; const int swap_c = 2; const int swap_d = 3; const int swap_e = 4; const int swap_f = 5; const int swap_g = 6;

        // trouver les 4 chiffres avec nb de segments unique
        for (int i = 0; i < line.digits.Count; i++)
        {
            switch (line.digits[i].Length)
            {
                case 2: digitIndexes[1] = i; break;
                case 3: digitIndexes[7] = i; break;
                case 4: digitIndexes[4] = i; break;
                case 7: digitIndexes[8] = i; break;
            }
        }

        Assert(digitIndexes.Count(d => d != -1) == 4);

        // abcdefg
        // 0123456
        // table de swap : pour chaque caractère, on a le mauvais caractère affiché
        var swaps = new char[7];

        // le f est le seul utilisé 9 fois, on utilise le 1 pour le trouver
        // l'autre est forcément le c
        var digit1 = line.digits[digitIndexes[1]];
        foreach (var ch in digit1)
        {
            var count = line.digits.Count(d => d.Contains(ch));
            if (count == 9)
            {
                // trouvé
                swaps[swap_f] = ch;
                swaps[swap_c] = digit1.First(c => c != ch);
                break;
            }
        }

        Assert(swaps.Count(c => c != 0) == 2);

        // on détermine le 'a' grace au 7
        swaps[swap_a] = line.digits[digitIndexes[7]].First(c => c != swaps[swap_f] && c != swaps[swap_c]);

        // on a le c, f et a
        // le 'e' est le seul utilisé 4 fois
        swaps[swap_e] = chars.First(c => line.digits.Count(d => d.Contains(c)) == 4);

        // le 'b' est le seul utilisé 6 fois
        swaps[swap_b] = chars.First(c => line.digits.Count(d => d.Contains(c)) == 6);

        // on a le a, b, c, e, f, et les digit 1, 4, 7 et 8
        // il manque le d et le g
        Assert(swaps.Count(c => c == 0) == 2);

        // le d est le caractère non découvert présent sur le 4 et pas le 7
        for (int i = 0; i < swaps.Length; i++)
        {
            var c = chars[i];
            if (swaps.Any(s => s == c)) continue; // on ignore les digits déjà trouvés

            if (line.digits[digitIndexes[4]].Contains(c) && !line.digits[digitIndexes[7]].Contains(c))
            {
                // trouvé
                swaps[swap_d] = c;
            }
        }

        Assert(swaps.Count(c => c == 0) == 1);

        // il reste juste le swap_g
        swaps[swap_g] = chars.First(c => !swaps.Contains(c));

        Assert(swaps.All(c => c != 0));

        // on a la table de transposition complète => on l'inverse
        // pour chaque caractère faussé, on aura le vrai caractère à utiliser
        var swapFix = new char[7];
        foreach (var c in chars)
        {
            swapFix[c - 'a'] = (char)('a' + swaps.IndexOf(c));
        }

        // fix des caractères affichés
        var fixedDisplays = line.displays.Select(d => new string(d.Select(c => swapFix[c - 'a']).ToArray())).Select(SortChars).ToArray();

        // construction du nombre
        var result = 0L;
        for (int i = 0; i < fixedDisplays.Length; i++)
        {
            var factor = fixedDisplays.Length - i - 1;
            var fixedDisplay = fixedDisplays[i];
            var index = symbols.IndexOf(fixedDisplay);

            Assert(index != -1);

            result += index * (long)Math.Pow(10, factor);
        }

        return result;
    }

    string SortChars(string value)
    {
        return new string(value.OrderBy(c => c).ToArray());
    }
}

void Day08()
{
    //                234567 abcdefg
    // 0 = 6 segments     x  xxx xxx
    // 1 = 2 segments x        x  x
    // 2 = 5 segments    x   x xxx x
    // 3 = 5 segments    x   x xx xx
    // 4 = 4 segments   x     xxx x
    // 5 = 5 segments    x   xx x xx
    // 6 = 6 segments     x  xx xxxx
    // 7 = 3 segments  x     x x  x
    // 8 = 7 segments      x xxxxxxx
    // 9 = 6 segments     x  xxxx xx
    //                111331 8687397
    var lines = Input.GetLines(8).Select(x =>
    {
        var parts = x.Split('|');
        return new { Digits = parts[0].Trim().Split(' '), Displays = parts[1].Trim().Split(' ') };
    });

    int count1 = 0, count4 = 0, count7 = 0, count8 = 0;
    foreach (var line in lines)
    {
        foreach (var display in line.Displays)
        {
            switch (display.Length)
            {
                case 2: count1++; break;
                case 4: count4++; break;
                case 3: count7++; break;
                case 7: count8++; break;
            }
        }
    }

    WriteLine(count1 + count4 + count7 + count8);
}

void Day07_Part2()
{
    var crabs = Input.GetLines(7).First().Split(',').Select(int.Parse).ToList();
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
    var crabs = Input.GetLines(7).First().Split(',').Select(int.Parse).ToList();
    crabs.Sort();

    var median = crabs[crabs.Count / 2];
    var dist = crabs.Sum(x => Math.Abs(x - median));

    WriteLine($"{median} => {dist}");
}

void Day06_Part2()
{
    var fishes = Input.GetLines(6).First().Split(',').Select(int.Parse).ToList();

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
    var fishs = Input.GetLines(6).First().Split(',').Select(int.Parse).ToList();

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

void Day05()
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

void Day04_Part2()
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

void Day04()
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

void Day03_Part2()
{
    // string[]
    var lines = Input.GetLinesArray(3);

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
    var lines = Input.GetLinesArray(3);

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
    var lines = Input.GetLinesArray(2);

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
    var lines = Input.GetLines(1)
        .Select(l => int.Parse(l.Trim()))
        .ToArray();


    var increasing = 0;
    for (var i = 1; i < lines.Length; i++)
    {
        if (lines[i] > lines[i - 1]) increasing++;
    }

    WriteLine(increasing);
}

/// <summary>
/// Helper for puzzle input.
/// </summary>
static class Input
{
    public static string[] GetLinesArray(int day, bool sample = false) => GetLines(day, sample).ToArray();

    public static IEnumerable<string> GetLines(int day, bool sample = false) => GetFile(day, sample).Split('\n', options: StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim());

    public static string GetFile(int day, bool sample = false)
    {
        var filename = $"Day{day:00}.txt";

        if (sample)
        {
            filename = $"Day{day:00}.sample.txt";
            if (File.Exists(filename))
            {
                return File.ReadAllText(filename);
            }

            throw new NotSupportedException($"Impossible de charger le fichier exemple s'il n'est pas déjà présent sur le disque. Vous devez créer le fichier {filename} sur le disque avec le contenu de l'exemple");
        }

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
}

static class ProgramHelper
{
    public static void Assert(bool condition) => System.Diagnostics.Debug.Assert(condition);
    public static void Assert(bool condition, string? message) => System.Diagnostics.Debug.Assert(condition, message);
    public static void ReadKey(bool intercept = true) => Console.ReadKey(intercept);
    public static void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);
    public static void WriteLine(object value, ConsoleColor? color = null) => WriteLine(value?.ToString(), color);
    public static void WriteLine(string? message, ConsoleColor? color = null) => Console.Write(message + Environment.NewLine, color);
    public static void WriteLine() => Console.Write(Environment.NewLine);
    public static void Write(object value, ConsoleColor? color = null) => Write(value?.ToString(), color);
    public static void Write(string? message, ConsoleColor? color = null)
    {
        ConsoleColor? previousColor = null;
        if (color != null)
        {
            previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color.Value;
        }

        Console.Write(message);

        if (previousColor != null)
        {
            Console.ForegroundColor = previousColor.Value;
        }
    }
}

static class Extensions
{
    public static int IndexOf<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (Equals(array[i], value))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Assumes that each line is composed of the same amount of characters and returns a grid with all lines.
    /// </summary>
    public static Grid<char> AsGridOfChars(this IEnumerable<string> lines, char? outOfBoundsValue = default) => new(lines, outOfBoundsValue);

    /// <summary>
    /// Assumes that each line is composed of characters '0' to '9', convert them to an array of bytes and returns a grid with all lines.
    /// </summary>
    public static Grid<byte> AsGridOfBytes(this IEnumerable<string> lines, byte? outOfBoundsValue = default) => AsGrid(lines, l => l.Select(c => (byte)(c - '0')), outOfBoundsValue);

    /// <summary>
    /// Converts all lines to a grid, assuming the with of the grid will be based on the number of values returned by the first line.
    /// </summary>
    /// <param name="lines">Lines to convert to grid.</param>
    /// <param name="transform">Function to use to transform each line of text into a collection of values.</param>
    /// <param name="outOfBoundsValue">
    /// A specific value to return when out of bounds coordinates are used when calling <see cref="Grid{T}.Item(long,long)"/>.
    /// Use <see langword="null" /> to raise an <see cref="ArgumentOutOfRangeException"/> when and out of bounds index is used.
    /// </param>
    public static Grid<T> AsGrid<T>(this IEnumerable<string> lines, Func<string, IEnumerable<T>> transform, T? outOfBoundsValue = default) where T : struct => new(lines.Select(l => transform(l)), outOfBoundsValue);

    public static IEnumerable<char> AsChars(this string @this) => @this;
}

/// <summary>
/// Represents a point in a space where X move from left to right and Y move from top to bottom.
/// </summary>
readonly struct Point
{
    public static readonly Point Empty = new(0, 0);

    public Point(in long x, in long y)
    {
        X = x;
        Y = y;
    }

    /// <summary>Horizontal coordinate from left to right.</summary>
    public long X { get; }

    /// <summary>Vertical coordinate from top to bottom.</summary>
    public long Y { get; }

    public Point Left => new(X - 1, Y);
    public Point Right => new(X + 1, Y);
    public Point Up => new(X, Y - 1);
    public Point Down => new(X, Y + 1);
    public Point RightDown => new(X + 1, Y + 1);
    public Point RightUp => new(X + 1, Y - 1);
    public Point LeftDown => new(X - 1, Y + 1);
    public Point LeftUp => new(X - 1, Y - 1);

    public static Point operator +(in Point p1, in Point p2) => p1.Add(p2);
    public static Point operator -(in Point p1, in Point p2) => p1.Subtract(p2);

    public Point Add(in Point other) => new(X + other.X, Y + other.Y);
    public Point Subtract(in Point other) => new(X - other.X, Y - other.Y);

    public void Deconstruct(out long x, out long y)
    {
        x = X;
        y = Y;
    }

    public override string ToString() => $"({X},{Y})";
}

/// <summary>
/// Represents a grid where values are organized in multiple rows in vertical order from top to bottom, and where each row is constituted of values ordered from left to right.
/// </summary>
class Grid<T> : IEnumerable<(Point point, T value)> where T : struct
{
    private readonly T[] _data;
    private readonly T? _outOfBoundsValue;

    public Grid(long width, long height, T? initialValue = default, T? outOfBoundsValue = default)
    {
        _data = new T[width * height];
        _outOfBoundsValue = outOfBoundsValue;

        if (initialValue != null)
        {
            for (int i = 0; i < _data.Length; i++)
                _data[i] = initialValue.Value;
        }
    }

    public Grid(in IEnumerable<IEnumerable<T>> source, T? outOfBoundsValue = default)
    {
        _outOfBoundsValue = outOfBoundsValue;
        var tempItems = new List<T>();
        foreach (var row in source)
        {
            foreach (var cell in row)
            {
                if (Height == 0) Width++;
                tempItems.Add(cell);
            }

            Height++;
        }

        _data = tempItems.ToArray();
    }

    /// <summary>Gets the width of the grid.</summary>
    public long Width { get; }

    /// <summary>Gets the height of the grid.</summary>
    public long Height { get; }

    /// <summary>Gets the max acceptable value for x in <see cref="this[in long, in long]"/>.</summary>
    public long XMax => Width - 1;

    /// <summary>Gets the max acceptable value for y in <see cref="this[in long, in long]"/>.</summary>
    public long YMax => Height - 1;

    /// <summary>Gets the total number of values in the grid.</summary>
    public long Count => Width * Height;

    public bool IsInBounds(in Point p) => IsInBounds(p.X, p.Y);

    public bool IsInBounds(in long x, in long y) => x >= 0 && x <= XMax && y >= 0 && y <= YMax;

    public bool TryGetValue(in Point p, out T value)
    {
        value = default;
        if (!IsInBounds(p.X, p.Y)) return false;
        value = this[p];
        return true;
    }
    public bool TryGetValue(in long x, in long y, out T value)
    {
        value = default;
        if (!IsInBounds(x, y)) return false;
        value = this[x, y];
        return true;
    }

    public T this[in Point p]
    {
        get => this[p.X, p.Y];
        set => this[p.X, p.Y] = value;
    }

    public T this[in long x, in long y]
    {
        get
        {
            if (x < 0 || x > XMax) return _outOfBoundsValue ?? throw new ArgumentOutOfRangeException(nameof(x), x, $"x must be between 0 and {XMax}");
            if (y < 0 || y > YMax) return _outOfBoundsValue ?? throw new ArgumentOutOfRangeException(nameof(y), y, $"y must be between 0 and {YMax}");
            return _data[y * Width + x];
        }
        set
        {
            if (x < 0 || x > XMax) throw new ArgumentOutOfRangeException(nameof(x), x, $"x must be between 0 and {XMax}");
            if (y < 0 || y > YMax) throw new ArgumentOutOfRangeException(nameof(y), y, $"y must be between 0 and {YMax}");
            _data[y * Width + x] = value;
        }
    }

    public T ValueOr(in Point p, T defaultValue)
    {
        if (!IsInBounds(p)) return defaultValue;
        return this[p];
    }

    public T ValueOr(in long x, in long y, T defaultValue)
    {
        if (!IsInBounds(x, y)) return defaultValue;
        return this[x, y];
    }

    public IEnumerator<(Point point, T value)> GetEnumerator()
    {
        for (var y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return (new Point(x, y), this[x, y]);
            }
        }
    }

    public T2 Aggregate<T2>(Func<Point, T2, T, T2> aggregator, in T2 seed = default) where T2 : struct
    {
        var value = seed;

        foreach (var x in this)
        {
            value = aggregator(x.point, value, x.value);
        }

        return value;
    }

    /// <summary>
    /// Calls an aggregation function on each value of the grid.
    /// </summary>
    public T2 Aggregate<T2>(Func<T2, T, T2> aggregator, in T2 seed = default) where T2 : struct
    {
        var value = seed;

        for (var y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                value = aggregator(value, this[x, y]);
            }
        }

        return value;
    }

    /// <summary>
    /// Visit each value in grid order (left to right then top to bottom) and call the <paramref name="visit"/>.
    /// Calls <see cref="Console.WriteLine"/> at the end of each row.
    /// </summary>
    public void VisitConsole(Action<T> visit) => Visit(visit, () => WriteLine());

    public void Visit(Action<T> visit, Action? endOfRow = null)
    {
        for (var y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                visit(this[x, y]);
            }

            endOfRow?.Invoke();
        }
    }

    public void Visit(Action<Point, T> visit, Action? endOfRow = null)
    {
        for (var y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                visit(new Point(x, y), this[x, y]);
            }

            endOfRow?.Invoke();
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}