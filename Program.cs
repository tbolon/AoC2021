using static ProgramHelper;

Day16_Part2();

#pragma warning disable CS8321

void Day16_Part2()
{
    var frames = Input.GetLines(16, sample: false).Select(l => l.Chunk(2).Select(c => new string(c)).Select(s => byte.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray()).ToArray();

    foreach (var frame in frames)
    {
        foreach (var b in frame)
        {
            WriteBinary(b, 8);
        }

        WriteLine();

        var i = 0;
        var size = frame.Length * 8;
        var rootPacket = ReadPacket(frame, ref i);
        WriteLine();
        WriteLine(rootPacket.Solve());
    }

    AoC2021.Day16.IPacket ReadPacket(byte[] frame, ref int i)
    {
        // version
        var v = ReadByte(frame, ref i, 3);
        WriteBinary(v, 3, ConsoleColor.Green);

        // typeID
        var t = ReadByte(frame, ref i, 3);
        WriteBinary(t, 3, ConsoleColor.Cyan);

        if (t == 4)
        {
            // literal
            var payloadIndex = i;
            byte continuation;
            long value = 0;
            do
            {
                continuation = ReadByte(frame, ref i, 1);
                WriteBinary(continuation, 1, ConsoleColor.Red);
                var dataByte = ReadByte(frame, ref i, 4);
                WriteBinary(dataByte, 4, ConsoleColor.White);
                value <<= 4;
                value |= dataByte;
            }
            while (continuation != 0);

            return new AoC2021.Day16.LiteralPacket(value);
        }
        else
        {
            // operator
            var lengthTypeID = ReadByte(frame, ref i, 1);
            WriteBinary(lengthTypeID, 1, ConsoleColor.Yellow);

            var packets = new List<AoC2021.Day16.IPacket>();

            if (lengthTypeID == 0)
            {
                // total length
                var len = ReadInt16(frame, ref i, 15);
                WriteBinary(len, 15, ConsoleColor.Magenta);
                var stop = i + len;
                while (i < stop)
                {
                    packets.Add(ReadPacket(frame, ref i));
                }
            }
            else
            {
                // number of packets
                var count = ReadInt16(frame, ref i, 11);
                WriteBinary(count, 11, ConsoleColor.Blue);
                for (int x = 0; x < count; x++)
                {
                    packets.Add(ReadPacket(frame, ref i));
                }
            }

            return new AoC2021.Day16.OperationPacked((AoC2021.Day16.OperationPacketType)t, packets);
        }
    }

    void WriteBinary(int value, int size, ConsoleColor? color = null)
    {
        while (size > 0)
        {
            var bit = (value >> (size - 1)) & 0x1;
            if (bit == 1) Write('1', color);
            else Write('0', color);
            size--;
        }
    }

    byte ReadByte(byte[] frame, ref int i, byte size)
    {
        Assert(size <= 8);
        byte final = 0;
        while (size >= 1)
        {
            var bit = BitAt(i, frame);
            final |= (byte)(bit << (size - 1));
            size--;
            i++;
        }
        return final;
    }

    short ReadInt16(byte[] frame, ref int i, byte size)
    {
        Assert(size <= 16);
        short final = 0;
        while (size >= 1)
        {
            final |= (short)(BitAt(i, frame) << (size - 1));
            size--;
            i++;
        }

        return final;
    }

    byte BitAt(int position, params byte[] frame)
    {
        var byteIndex = position / 8;
        var byteValue = frame[byteIndex];
        var shift = position % 8;
        var mask = (byte)(128 >> shift);
        var bitValue = (byte)(byteValue & mask) != 0 ? (byte)1 : (byte)0;
        return bitValue;
    }
}

void Day16()
{
    var frames = Input.GetLines(16, sample: false).Select(l => l.Chunk(2).Select(c => new string(c)).Select(s => byte.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray()).ToArray();

    long part1;
    foreach (var frame in frames)
    {
        part1 = 0;
        foreach (var b in frame)
        {
            WriteData(b, 8);
        }
        WriteLine();

        var i = 0;
        var size = frame.Length * 8;
        while (i < size)
        {
            ReadPacket(frame, ref i);

            // ignore end padding
            if (size - i < 8)
            {
                break;
            }
        }
        WriteLine();
        WriteLine(part1);
    }

    void ReadPacket(byte[] frame, ref int i)
    {
        // version
        var v = ReadByte(frame, ref i, 3);
        WriteData(v, 3, ConsoleColor.Green);
        part1 += v;

        // typeID
        var t = ReadByte(frame, ref i, 3);
        WriteData(t, 3, ConsoleColor.Cyan);

        if (t == 4)
        {
            // literal
            var payloadIndex = i;
            byte continuation;
            List<byte> data = new();
            do
            {
                continuation = ReadByte(frame, ref i, 1);
                WriteData(continuation, 1, ConsoleColor.Red);
                var dataByte = ReadByte(frame, ref i, 4);
                WriteData(dataByte, 4, ConsoleColor.White);
                data.Add(dataByte);
            }
            while (continuation != 0);
        }
        else
        {
            // operator
            var lengthTypeID = ReadByte(frame, ref i, 1);
            WriteData(lengthTypeID, 1, ConsoleColor.Yellow);
            if (lengthTypeID == 0)
            {
                // total length
                var len = ReadInt16(frame, ref i, 15);
                WriteData(len, 15, ConsoleColor.Magenta);
                var stop = i + len;
                while (i < stop)
                {
                    ReadPacket(frame, ref i);
                }
            }
            else
            {
                // number of packets
                var count = ReadInt16(frame, ref i, 11);
                WriteData(count, 11, ConsoleColor.Blue);
                for (int x = 0; x < count; x++)
                {
                    ReadPacket(frame, ref i);
                }
            }
        }
    }

    void WriteData(int value, int size, ConsoleColor? color = null)
    {
        while (size > 0)
        {
            var bit = (value >> (size - 1)) & 0x1;
            if (bit == 1) Write('1', color);
            else Write('0', color);
            size--;
        }
    }

    byte ReadByte(byte[] frame, ref int i, byte size)
    {
        Assert(size <= 8);
        byte final = 0;
        while (size >= 1)
        {
            var bit = BitAt(i, frame);
            final |= (byte)(bit << (size - 1));
            size--;
            i++;
        }
        return final;
    }

    short ReadInt16(byte[] frame, ref int i, byte size)
    {
        Assert(size <= 16);
        short final = 0;
        while (size >= 1)
        {
            final |= (short)(BitAt(i, frame) << (size - 1));
            size--;
            i++;
        }

        return final;
    }

    int ReadInt32(byte[] frame, ref int i, byte size)
    {
        Assert(size <= 32);
        int final = 0;
        while (size >= 1)
        {
            final |= BitAt(i, frame) << (size - 1);
            size--;
            i++;
        }

        return final;
    }

    long ReadInt64(byte[] frame, ref int i, byte size)
    {
        Assert(size <= 64);
        long final = 0;
        while (size >= 1)
        {
            final |= (long)BitAt(i, frame) << (size - 1);
            size--;
            i++;
        }

        return final;
    }

    byte BitAt(int position, params byte[] frame)
    {
        var byteIndex = position / 8;
        var byteValue = frame[byteIndex];
        var shift = position % 8;
        var mask = (byte)(128 >> shift);
        var bitValue = (byte)(byteValue & mask) != 0 ? (byte)1 : (byte)0;
        return bitValue;
    }
}

void Day15_Part2()
{
    // thanks to the incredible https://www.redblobgames.com/pathfinding/a-star/introduction.html
    // initial grid
    var smallGrid = Input.GetLines(15, sample: false).AsGridOfBytes(byte.MaxValue);

    // fill final grid
    var grid = new Grid<int>(smallGrid.Width * 5, smallGrid.Height * 5);
    for (int x = 0; x < 5; x++)
    {
        for (int y = 0; y < 5; y++)
        {
            foreach (var (point, value) in smallGrid)
            {
                var newX = (smallGrid.Width * x) + point.X;
                var newY = (smallGrid.Height * y) + point.Y;
                var newValue = ((value + x + y - 1) % 9) + 1;
                if (x == 0 && y == 0)
                {
                    Assert(newValue == value);
                }

                grid[newX, newY] = newValue;
            }
        }
    }

    var start = Point.Empty;
    var end = new Point(grid.XMax, grid.YMax);
    Point current;

    // contient pour chaque point le coût depuis le départ
    // on va y ajouter tous les points que l'on croise avec le coût pour y arriver
    // si cette liste est vide, on a perdu (aucun chemin)
    // dès que l'on trouve la sortie on arrête d'explorer cette pile
    var frontier = new PriorityQueue<Point, int>();
    frontier.Enqueue(start, 0);

    // contient les chemins calculés les plus optimisés entre les différents noeuds du graphe
    var cameFrom = new Dictionary<Point, Point>();

    // pour chaque point, mémorise le coût pour arriver à ce point depuis le départ (sans le cout du point lui même)
    var costSoFar = new Dictionary<Point, int>();
    costSoFar[start] = 0;

    // build paths
    while (frontier.Count != 0)
    {
        // on examine le point le plus intéressant pour l'instant
        // c'est l'avantage de la PriorityQueue : elle dépile les éléments de plus faible priorité en premier
        // on va donc examiner les chemins les moins couteux en priorité
        current = frontier.Dequeue();

        // trouvé
        if (current == end)
        {
            break;
        }

        // on examine les voisins
        foreach (var next in current.Neighbors(grid))
        {
            // le coût est celui d'avant ce point + coût du point
            var newCost = costSoFar[current] + grid[current];

            // si le nouveau point n'a jamais été exploré, ou que le cout pour y arriver est plus faible
            // alors ce point est intéressant et doit être conservé
            if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
            {
                // on stocke le cout jusqu'au point
                costSoFar[next] = newCost;

                // on mémorise le point avec son coût comme étant à explorer
                var priority = newCost;
                frontier.Enqueue(next, priority);

                // on stocke le chemin inverse
                cameFrom[next] = current;
            }
        }
    }

    // create reverse path
    current = end;
    var path = new List<Point>();
    while (current != start)
    {
        path.Add(current);
        current = cameFrom[current];
    }
    path.Add(start);
    path.Reverse();

    grid.VisitConsole((p, x) =>
    {
        if (!path.Contains(p))
        {
            Write((char)(x + '0'));
        }
        else
        {
            Write((char)(x + '0'), ConsoleColor.Green);
        }
    });

    WriteLine(path.Sum(p => p == start ? 0 : grid[p]));
}

void Day15()
{
    // thanks to the incredible https://www.redblobgames.com/pathfinding/a-star/introduction.html
    // initial grid
    var grid = Input.GetLines(15, sample: true).AsGridOfBytes(byte.MaxValue);

    var start = Point.Empty;
    var end = new Point(grid.XMax, grid.YMax);
    Point current;

    var frontier = new PriorityQueue<Point, int>();
    frontier.Enqueue(start, 0);

    var cameFrom = new Dictionary<Point, Point>();
    //cameFrom[start] = null;

    var costSoFar = new Dictionary<Point, int>();
    costSoFar[start] = 0;

    // build paths
    while (frontier.Count != 0)
    {
        current = frontier.Dequeue();

        if (current == end)
        {
            break;
        }

        foreach (var next in current.Neighbors(grid))
        {
            var newCost = costSoFar[current] + grid[current];

            if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
            {
                costSoFar[next] = newCost;
                var priority = newCost;
                frontier.Enqueue(next, priority);
                cameFrom[next] = current;
            }
        }
    }

    // create reverse path
    current = end;
    var path = new List<Point>();
    while (current != start)
    {
        path.Add(current);
        current = cameFrom[current];
    }
    path.Add(start);
    path.Reverse();

    grid.VisitConsole(x => Console.Write((char)(x + '0')));

    ReadKey();

    Clear();
    grid.VisitConsole((p, x) =>
    {
        if (!path.Contains(p))
        {
            Write((char)(x + '0'));
        }
        else
        {
            Write('o', ConsoleColor.Green);
        }
    });

    WriteLine(path.Sum(p => p == start ? 0 : grid[p]));
}

void Day14_Part2()
{
    const int maxLevel = 40;
    var lines = Input.GetLines(14, sample: false).ToArray();
    var polymerTemplate = lines[0].ToList();

    // on stocke les permutations dans un dictionaire uint => char ou la clé correspond au résultat de AsKey()
    var permutations = lines.Skip(1).Select(l => l.Split('-')).ToDictionary(a => AsKey(a[0][0], a[0][1]), a => a[1][2]);

    // on va optimiser la recherche récursive en stockant le tableau de score pour un ensemble paire + niveau de profondeur de récursivité
    // sans ça, le calcul prend trop de temps.
    //
    // attention: le raccourci ne contient le score que pour la partie *générée* de la paire, par ex le score de (AB) n'inclut pas la présence de 'A' et 'B'.
    var shortcuts = new Dictionary<(uint, int), long[]>();

    // le tableau de score est un tableau avec 0 = 'A'
    // on l'initialise avec le nb de caractères du template vu que la méthode Visit() ne renvoie que le score de la partie *générée*.
    var finalScore = new long[26];
    foreach (var c in polymerTemplate)
    {
        finalScore[c - 'A']++;
    }

    // on va ensuite analyser chaque paire du template, et descendre en récursif x fois (= niveau)
    // pour analyser chaque paire produite
    for (int x = 1; x < polymerTemplate.Count; x++)
    {
        Visit(polymerTemplate[x - 1], polymerTemplate[x], 1, finalScore);
    }

    // score final = nb d'occurences du caractère le plus présent - nb d'occurence du caractère le moins présent
    WriteLine(finalScore.Where(p => p != 0).Max() - finalScore.Where(p => p != 0).Min());

    // obtient une clé unique sur 32 bits à partir de 2 caractères de 16 bits
    // utilisé pour essayer d'optimiser le lookup, pas sur que ce soit finalement utile
    uint AsKey(char c1, char c2) => ((uint)(c1 - 'A')) << 16 | ((uint)(c2 - 'A'));

    // visite une paire AB avec le niveau de profondeur "level" et ajoute le score dans le tableau donné
    // attention: le score n'est calculé que pour la partie *générée*
    void Visit(char a, char b, int level, long[] parentScore)
    {
        if (level > maxLevel)
        {
            return;
        }

        var key = AsKey(a, b);

        // si le résultat de cette combinaison (paire, niveau) a déjà été calculée on la réutilise
        if (shortcuts.TryGetValue((key, level), out var shortcut))
        {
            Add(shortcut, parentScore);
            return;
        }

        // nouveau score à calculer pour cette paire+profondeur
        var score = new long[26];

        // on recherche la lettre à insérer AB => AXB
        var permutation = permutations[key];

        // ajout du score de la permutation ajoutée
        score[permutation - 'A']++;

        // on ajoute le score de toutes les itérations successives pour la partie gauche (AX)
        Visit(a, permutation, level + 1, score);

        // on ajoute le score de toutes les itérations successives pour la partie droite (XB)
        Visit(permutation, b, level + 1, score);

        // on met en cache le score local (AB, level)
        shortcuts[(key, level)] = score;

        // on ajoute le score local au score parent
        Add(score, parentScore);
    }

    void Add(long[] score, long[] parentScore)
    {
        for (int i = 0; i < parentScore.Length; i++)
        {
            parentScore[i] += score[i];
        }
    }
}

void Day14()
{
    var lines = Input.GetLines(14, sample: false).ToArray();
    var polymer = lines[0].ToList();
    var permutations = lines.Skip(1).Select(l => l.Split('-')).ToDictionary(a => a[0].Trim(), a => a[1][2]);

    Stack<char> insertions = new();

    for (int i = 0; i < 10; i++)
    {
        insertions.Clear();

        for (int x = 1; x < polymer.Count; x++)
        {
            var pair = string.Concat(polymer[x - 1], polymer[x]);
            var insertion = permutations[pair];
            insertions.Push(insertion);
        }

        Assert(insertions.Count == polymer.Count - 1);

        for (int x = insertions.Count; x >= 1; x--)
        {
            polymer.Insert(x, insertions.Pop());
        }
    }

    var score = polymer.Aggregate(new Dictionary<char, int>(), (cumul, c) =>
    {
        cumul[c] = 1 + (cumul.TryGetValue(c, out var current) ? current : 0);
        return cumul;
    });

    WriteLine(score.Max(p => p.Value) - score.Min(p => p.Value));
}

void Day13_Part2()
{
    var lines = Input.GetLines(13, sample: false);
    var coords = lines.Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("fold along")).Select(x => x.Split(',').Select(int.Parse).ToArray());
    var operations = lines.Where(l => !string.IsNullOrEmpty(l) && l.StartsWith("fold along")).Select(l => new
    {
        Vertical = l.StartsWith("fold along x="),
        Coordinate = int.Parse(l.Substring("fold along x=".Length))
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

    void DrawGrid(Grid<bool> grid, bool? vertical = null, int coordinate = 0)
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

void Day13()
{
    var lines = Input.GetLines(13, sample: false);
    var coords = lines.Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("fold along")).Select(x => x.Split(',').Select(int.Parse).ToArray());
    var operations = lines.Where(l => !string.IsNullOrEmpty(l) && l.StartsWith("fold along")).Select(l => new
    {
        Vertical = l.StartsWith("fold along x="),
        Coordinate = int.Parse(l.Substring("fold along x=".Length))
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

void Day12_Part2_Alt()
{
    var segments = Input.GetLines(12, sample: false).Select(x => x.Split('-'));

    // init
    Dictionary<string, AoC2021.Day10.Cave> caves = new();
    foreach (var segment in segments)
    {
        var fromName = segment[0];
        var toName = segment[1];

        if (!caves.TryGetValue(fromName, out var from))
        {
            from = new AoC2021.Day10.Cave(fromName);
            caves.Add(fromName, from);
        }

        if (!caves.TryGetValue(toName, out var to))
        {
            to = new AoC2021.Day10.Cave(toName);
            caves.Add(toName, to);
        }

        if (!from.IsEnd && !to.IsStart)
        {
            from.Caves.Add(to);
        }

        if (!from.IsStart && !to.IsEnd)
        {
            to.Caves.Add(from);
        }
    }

    // solve
}

void Day12_Part2()
{
    var segments = Input.GetLines(12, sample: false).Select(x => x.Split('-'));
    Dictionary<string, List<string>> maps = new();

    foreach (var segment in segments)
    {
        var from = segment[0];
        var to = segment[1];
        if (from != "end" && to != "start")
        {
            if (!maps.TryGetValue(from, out var nodes))
            {
                maps[from] = nodes = new List<string>();
            }

            if (!nodes.Contains(to))
                nodes.Add(to);
        }

        if (from != "start" && to != "end")
        {
            if (!maps.TryGetValue(to, out var nodes))
            {
                maps[to] = nodes = new List<string>();
            }

            if (!nodes.Contains(from))
                nodes.Add(from);
        }
    }

    HashSet<string> paths = new();

    // chemins à explorer
    var missingPaths = new Stack<string>();
    foreach (var node in maps["start"])
    {
        missingPaths.Push($"start,{node}");
    }

    while (missingPaths.Count > 0)
    {
        var path = missingPaths.Pop();

        var lastNode = path.Split(',').Last();

        if (maps.TryGetValue(lastNode, out var nextNodes))
        {
            foreach (var nextNode in nextNodes)
            {
                var nextPath = $"{path},{nextNode}";

                var isSmall = nextNode.All(c => char.IsLower(c));

                if (isSmall)
                {
                    // cave small déjà explorée
                    if (path.Contains($",{nextNode}"))
                    {
                        if (path.StartsWith("start2,"))
                        {
                            // on a déjà exploré une cave deux fois, on ne peut plus
                            continue;
                        }
                        else
                        {
                            // on marque comme quoi on a exploré deux fois une cave, et on laisse continuer
                            nextPath = nextPath.Replace("start,", "start2,");
                        }
                    }
                }

                if (nextNode == "end")
                {
                    if (!paths.Contains(nextPath))
                    {
                        paths.Add(nextPath);
                    }
                }
                else if (!missingPaths.Contains(nextPath))
                {
                    missingPaths.Push(nextPath);
                }
            }
        }
    }

    WriteLine(paths.Count);
}

void Day12()
{
    var segments = Input.GetLines(12, sample: false).Select(x => x.Split('-'));
    Dictionary<string, List<string>> maps = new();

    foreach (var segment in segments)
    {
        var from = segment[0];
        var to = segment[1];
        if (from != "end" && to != "start")
        {
            if (!maps.TryGetValue(from, out var nodes))
            {
                maps[from] = nodes = new List<string>();
            }

            if (!nodes.Contains(to))
                nodes.Add(to);
        }

        if (from != "start" && to != "end")
        {
            if (!maps.TryGetValue(to, out var nodes))
            {
                maps[to] = nodes = new List<string>();
            }

            if (!nodes.Contains(from))
                nodes.Add(from);
        }
    }

    HashSet<string> paths = new();

    // chemins à explorer
    var missingPaths = new Stack<string>();
    foreach (var node in maps["start"])
    {
        missingPaths.Push($"start,{node}");
    }

    while (missingPaths.Count > 0)
    {
        var path = missingPaths.Pop();

        var lastNode = path.Split(',').Last();

        if (maps.TryGetValue(lastNode, out var nextNodes))
        {
            foreach (var nextNode in nextNodes)
            {
                var nextPath = $"{path},{nextNode}";

                var isSmall = nextNode.All(c => char.IsLower(c));

                // cave small déjà explorée
                if (isSmall && path.Contains($",{nextNode}"))
                {
                    continue;
                }

                if (nextNode == "end")
                {
                    if (!paths.Contains(nextPath))
                    {
                        paths.Add(nextPath);
                    }
                }
                else if (!missingPaths.Contains(nextPath))
                {
                    missingPaths.Push(nextPath);
                }
            }
        }
    }

    WriteLine(paths.Count);


}

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
    var grid = Input.GetLines(9, sample: false).AsGridOfBytes(byte.MaxValue);

    // détection bassins
    List<Point> points = new();
    foreach (var (point, value) in grid)
    {
        if (grid[point.Right] > value && grid[point.Left] > value && grid[point.Up] > value && grid[point.Down] > value)
        {
            points.Add(point);
        }
    }

    // grille avec un 0 pour les zones vides, 1 pour les zones déjà découvertes d'un bassin, 0xff pour les bords (hauteur 9)
    var fill = grid.Copy((point, value) => value == 9 ? byte.MaxValue : 0);

    // examen du bassin
    int[] sizes = new int[points.Count];
    for (int i = 0; i < points.Count; i++)
    {
        sizes[i] = Visit(points[i]);
    }

    // calcul du produit des 3 plus grandes
    WriteLine(sizes.OrderByDescending(s => s).Take(3).Aggregate(1, (agg, current) => agg * current));

    // examine une case et démarre l'examen de ses voisines
    int Visit(Point point)
    {
        if (!fill.Contains(point) || fill[point] != 0)
            return 0; // out of bounds or already visited

        fill[point] = 1; // marquée comme visitée

        return 1 + Visit(point.Left) + Visit(point.Right) + Visit(point.Up) + Visit(point.Down);
    }
}

void Day09()
{
    var grid = Input.GetLines(9, sample: false).AsGridOfBytes(byte.MaxValue);

    var score = grid.Aggregate<int>((point, cumul, value) =>
    {
        if (grid[point.Right] > value
            && grid[point.Left] > value
            && grid[point.Up] > value
            && grid[point.Down] > value)
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

    public static IEnumerable<string> GetLines(int day, bool sample = false) => GetFile(day, sample).Split('\n', options: StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l));

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
    /// <summary>
    /// Checks for a condition; if the condition is false, displays a message box that shows the call stack.
    /// </summary>
    /// <param name="condition">The conditional expression to evaluate. If the condition is true, a failure message is not sent and the message box is not displayed.</param>
    public static void Assert(bool condition) => System.Diagnostics.Debug.Assert(condition);

    /// <summary>
    /// Checks for a condition; if the condition is false, outputs a specified message and displays a message box that shows the call stack.
    /// </summary>
    /// <param name="condition">The conditional expression to evaluate. If the condition is true, the specified message is not sent and the message box is not displayed.</param>
    /// <param name="message">The message to display.</param>
    public static void Assert(bool condition, string? message) => System.Diagnostics.Debug.Assert(condition, message);

    public static void ReadKey(bool intercept = true) => Console.ReadKey(intercept);
    public static void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);
    public static void Clear() => Console.Clear();
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
    public static bool operator ==(in Point p1, in Point p2) => p1.X == p2.X && p1.Y == p2.Y;
    public static bool operator !=(in Point p1, in Point p2) => p1.X != p2.X || p1.Y != p2.Y;

    public Point Add(in Point other) => new(X + other.X, Y + other.Y);
    public Point Subtract(in Point other) => new(X - other.X, Y - other.Y);

    public void Deconstruct(out long x, out long y)
    {
        x = X;
        y = Y;
    }

    public IEnumerable<Point> Neighbors()
    {
        yield return Up;
        yield return Left;
        yield return Down;
        yield return Right;
    }

    public IEnumerable<Point> Neighbors(IGrid grid)
    {
        if (grid.Contains(Up))
            yield return Up;
        if (grid.Contains(Left))
            yield return Left;
        if (grid.Contains(Down))
            yield return Down;
        if (grid.Contains(Right))
            yield return Right;
    }


    public override string ToString() => $"({X},{Y})";
}

interface IGrid
{
    bool Contains(Point p);
}

/// <summary>
/// Represents a grid where values are organized in multiple rows in vertical order from top to bottom, and where each row is constituted of values ordered from left to right.
/// </summary>
class Grid<T> : IGrid, IEnumerable<(Point point, T value)> where T : struct
{
    private readonly T[] _data;
    private readonly T? _outOfBoundsValue;

    public Grid(long width, long height, T? initialValue = default, T? outOfBoundsValue = default)
    {
        _data = new T[width * height];
        _outOfBoundsValue = outOfBoundsValue;
        Width = width;
        Height = height;

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

    /// <summary>
    /// Visit each value in grid order (left to right then top to bottom) and call the <paramref name="visit"/>.
    /// Calls <see cref="Console.WriteLine"/> at the end of each row.
    /// </summary>
    public void VisitConsole(Action<Point, T> visit) => Visit(visit, () => WriteLine());

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

    public bool Contains(Point p) => p.X >= 0 && p.X <= XMax && p.Y >= 0 && p.Y <= YMax;

    /// <summary>
    /// Effectue une copie de cette grille, en transformant chaque élément.
    /// </summary>
    public Grid<T2> Copy<T2>(Func<Point, T, T2> transform) where T2 : struct
    {
        var result = new Grid<T2>(Width, Height);
        foreach (var (point, value) in this)
        {
            result[point] = transform(point, value);
        }
        return result;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}