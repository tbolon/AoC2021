using static ProgramHelper;

namespace AoC2021;

internal static class Day07
{

    public static void Part2()
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

    public static void Part1()
    {
        var crabs = Input.GetLines(7).First().Split(',').Select(int.Parse).ToList();
        crabs.Sort();

        var median = crabs[crabs.Count / 2];
        var dist = crabs.Sum(x => Math.Abs(x - median));

        WriteLine($"{median} => {dist}");
    }
}
