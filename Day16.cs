namespace AoC2021.Day16;

enum OperationPacketType
{
    Sum = 0,

    Product = 1,

    Minimum = 2,

    Maximum = 3,

    GreatherThan = 5,

    LowerThan = 6,

    EqualsTo = 7
}

interface IPacket
{
    long Solve();
}

class OperationPacked : IPacket
{
    private readonly OperationPacketType type;
    private readonly List<IPacket> args;

    public OperationPacked(OperationPacketType type, List<IPacket> args)
    {
        this.type = type;
        this.args = args;
    }

    public long Solve()
    {
        switch (type)
        {
            case OperationPacketType.Sum:
                return args.Sum(p => p.Solve());
            case OperationPacketType.Product:
                return args.Aggregate(1L, (cumul, p) => cumul * p.Solve());
            case OperationPacketType.Minimum:
                return args.Min(p => p.Solve());
            case OperationPacketType.Maximum:
                return args.Max(p => p.Solve());
            case OperationPacketType.GreatherThan:
                return args[0].Solve() > args[1].Solve() ? 1 : 0L;
            case OperationPacketType.LowerThan:
                return args[0].Solve() < args[1].Solve() ? 1 : 0L;
            case OperationPacketType.EqualsTo:
                return args[0].Solve() == args[1].Solve() ? 1 : 0L;
            default:
                throw new NotImplementedException();
        }
    }
}

class LiteralPacket : IPacket
{
    private readonly long value;

    public LiteralPacket(long value)
    {
        this.value = value;
    }

    public long Solve() => value;
}


