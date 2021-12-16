using static ProgramHelper;

namespace AoC2021;

internal static class Day11
{
    public static void Part2()
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
    public static void Part1()
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

}
