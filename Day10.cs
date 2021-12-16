using static ProgramHelper;

namespace AoC2021;

internal static class Day10
{
    public static void Part2()
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

    public static void Part1()
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
    }

    private static char ExamLine(string line)
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

    internal class Cave
    {
        public Cave(string name)
        {
            Name = name;
            IsSmall = name.All(c => char.IsLower(c));
            IsStart = name == "start";
            IsEnd = name == "end";
        }

        public string Name { get; }

        public bool IsStart { get; }

        public bool IsEnd { get; }

        public bool IsSmall { get; }

        public HashSet<Cave> Caves { get; } = new HashSet<Cave>();

        public override string ToString() => Name;

        public override int GetHashCode() => Name.GetHashCode();
    }

    internal class Path
    {
        private string raw = string.Empty;
        private string? smallVisited;

        public Path()
        { }

        private Path(string raw, string? smallVisited)
        {
            this.raw = raw;
            this.smallVisited = smallVisited;
        }

        public bool Finished { get; private set; }

        public IEnumerable<Path> Add(Cave cave)
        {
            if (cave.IsStart)
                raw = cave.Name;
            else
                raw += $",{cave}";

            if (cave.IsEnd)
            {
                yield return this;
            }

            foreach (var next in cave.Caves)
            {
                if (next.IsEnd)
                {
                    yield return Append(next);
                }
                else
                {
                    // TODO
                }
            }
        }

        private Path Append(Cave cave)
        {
            string nextRaw;
            if (cave.IsStart)
                nextRaw = cave.Name;
            else
                nextRaw = $"{raw},{cave}";

            return new Path(nextRaw, smallVisited);
        }

        public bool Accept(Cave next)
        {
            var nextRaw = $",{next}";

            if (next.IsEnd)
            {
                Finished = true;
                raw += nextRaw;
                return true;
            }

            if (next.IsSmall)
            {
                if (raw.Contains(nextRaw) && smallVisited != null)
                {
                    return false;
                }

                smallVisited = next.Name;
            }

            raw += nextRaw;
            return true;
        }
    }


}