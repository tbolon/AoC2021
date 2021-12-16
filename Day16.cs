using static ProgramHelper;

namespace AoC2021;

internal static class Day16
{
    public static void Part2()
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

        IPacket ReadPacket(byte[] frame, ref int i)
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

                return new LiteralPacket(value);
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

                return new OperationPacked((OperationPacketType)t, packets);
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

    public static void Part1()
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
            return type switch
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
}