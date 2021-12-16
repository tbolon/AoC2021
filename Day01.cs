using static ProgramHelper;

namespace AoC2021
{
    internal class Day01
    {
        public static void Solve()
        {
            var lines = Input.GetLines(1)
                .Select(l => int.Parse(l.Trim()))
                .ToArray();


            var increasing = 0;
            for (var i = 1; i < lines.Length; i++)
            {
                if (lines[i] > lines[i - 1]) increasing++;
            }

            WriteLine(increasing);
        }
    }
}
