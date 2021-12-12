namespace AoC2021.Day10;

class Cave
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

class Path
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

