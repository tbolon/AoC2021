using static ProgramHelper;

namespace AoC2021;

internal class Day02
{
    public static void Solve()
    {
        var lines = Input.GetLinesArray(2);

        (int horizontal, int depth, int aim) position = (0, 0, 0);

        for (int i = 0; i < lines.Length; i++)
        {
            var parts = lines[i].Split(' ').ToArray();
            var verb = parts[0];
            var offset = int.Parse(parts[1]);

            position = verb switch
            {
                "forward" => (position.horizontal + offset, position.depth + (position.aim * offset), position.aim),
                "down" => (position.horizontal, position.depth, position.aim + offset),
                "up" => (position.horizontal, position.depth, position.aim - offset),
                _ => position
            };
        }

        WriteLine(position.horizontal * position.depth);
    }
}
