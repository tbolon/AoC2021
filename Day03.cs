using static ProgramHelper;

namespace AoC2021;

internal static class Day03
{
    public static void Part2()
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
    public static void Part1()
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
}
