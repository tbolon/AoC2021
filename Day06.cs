using static ProgramHelper;

namespace AoC2021;

internal static class Day06
{
    public static void Part2()
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

    public static void Part1()
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
}
