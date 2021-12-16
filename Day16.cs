using static ProgramHelper;

namespace AoC2021;

internal static class Day16
{
    public static void Part2()
    {
        var frames = ReadInput();

        foreach (var frame in frames)
        {
            foreach (var b in frame)
            {
                WriteBinary(b, 8);
            }

            WriteLine();

            var i = 0;
            var rootPacket = ReadPacket(frame, ref i);
            WriteLine();
            WriteLine(rootPacket.Solve());
        }
    }

    public static void Part1()
    {
        var frames = ReadInput();

        foreach (var frame in frames)
        {
            foreach (var b in frame)
            {
                WriteBinary(b, 8);
            }

            WriteLine();

            var i = 0;
            var rootPacket = ReadPacket(frame, ref i);
            WriteLine();
            WriteLine(rootPacket.Version);
        }
    }

    private static byte[][] ReadInput(bool sample = false) => Input
        .GetLines(16, sample)
        .Select(l => l
            .Chunk(2)
            .Select(c => new string(c))
            .Select(s => byte.Parse(s, System.Globalization.NumberStyles.HexNumber))
            .ToArray())
        .ToArray();

    private static IPacket ReadPacket(byte[] frame, ref int i)
    {
        // version
        var version = ReadByte(frame, ref i, 3);
        WriteBinary(version, 3, ConsoleColor.Green);

        // typeID
        var typeID = ReadByte(frame, ref i, 3);
        WriteBinary(typeID, 3, ConsoleColor.Cyan);

        if (typeID == 4)
        {
            // literal
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

            return new LiteralPacket(version, value);
        }
        else
        {
            // operator
            var lengthTypeID = ReadByte(frame, ref i, 1);
            WriteBinary(lengthTypeID, 1, ConsoleColor.Yellow);

            var packets = new List<IPacket>();

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

            return new OperationPacket(version, (OperationPacketType)typeID, packets);
        }
    }

    static void WriteBinary(int value, int size, ConsoleColor? color = null)
    {
        while (size > 0)
        {
            var bit = (value >> (size - 1)) & 0x1;
            Write(bit == 1 ? '1' : '0', color);
            size--;
        }
    }

    static byte ReadByte(byte[] frame, ref int i, byte size)
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

    static short ReadInt16(byte[] frame, ref int i, byte size)
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

    static byte BitAt(int position, params byte[] frame)
    {
        var byteIndex = position / 8;
        var byteValue = frame[byteIndex];
        var shift = position % 8;
        var mask = (byte)(128 >> shift);
        var bitValue = (byte)(byteValue & mask) != 0 ? (byte)1 : (byte)0;
        return bitValue;

    }

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
        int Version { get; }

        long Solve();
    }

    class OperationPacket : IPacket
    {
        private readonly OperationPacketType type;
        private readonly List<IPacket> args;
        private readonly int version;

        public OperationPacket(int version, OperationPacketType type, List<IPacket> args)
        {
            this.version = version;
            this.type = type;
            this.args = args;
        }

        public int Version => version + args.Sum(a => a.Version);

        public long Solve() => type switch
        {
            OperationPacketType.Sum => args.Sum(p => p.Solve()),
            OperationPacketType.Product => args.Aggregate(1L, (cumul, p) => cumul * p.Solve()),
            OperationPacketType.Minimum => args.Min(p => p.Solve()),
            OperationPacketType.Maximum => args.Max(p => p.Solve()),
            OperationPacketType.GreatherThan => args[0].Solve() > args[1].Solve() ? 1 : 0L,
            OperationPacketType.LowerThan => args[0].Solve() < args[1].Solve() ? 1 : 0L,
            OperationPacketType.EqualsTo => args[0].Solve() == args[1].Solve() ? 1 : 0L,
            _ => throw new NotImplementedException(),
        };
    }

    class LiteralPacket : IPacket
    {
        private readonly long value;

        public LiteralPacket(int version, long value)
        {
            Version = version;
            this.value = value;
        }

        public int Version { get; }

        public long Solve() => value;
    }
}