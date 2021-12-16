using static ProgramHelper;

namespace AoC2021;

internal static class Day12
{
    public static void Part2_Alt()
    {
        var segments = Input.GetLines(12, sample: false).Select(x => x.Split('-'));

        // init
        Dictionary<string, Day10.Cave> caves = new();
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

    public static void Part2()
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

    public static void Part1()
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
}