using static ProgramHelper;

namespace AoC2021;

internal class Day08
{
    public static void Part2()
    {
        /*
          0:      1:      2:      3:      4:
         aaaa    ....    aaaa    aaaa    ....
        b    c  .    c  .    c  .    c  b    c
        b    c  .    c  .    c  .    c  b    c
         ....    ....    dddd    dddd    dddd
        e    f  .    f  e    .  .    f  .    f
        e    f  .    f  e    .  .    f  .    f
         gggg    ....    gggg    gggg    ....

          5:      6:      7:      8:      9:
         aaaa    aaaa    aaaa    aaaa    aaaa
        b    .  b    .  .    c  b    c  b    c
        b    .  b    .  .    c  b    c  b    c
         dddd    dddd    ....    dddd    dddd
        .    f  e    f  .    f  e    f  .    f
        .    f  e    f  .    f  e    f  .    f
         gggg    gggg    ....    gggg    gggg

        //                234567 abcdefg
        // 0 = 6 segments     x  xxx xxx
        // 1 = 2 segments x        x  x
        // 2 = 5 segments    x   x xxx x
        // 3 = 5 segments    x   x xx xx
        // 4 = 4 segments   x     xxx x
        // 5 = 5 segments    x   xx x xx
        // 6 = 6 segments     x  xx xxxx
        // 7 = 3 segments  x     x x  x
        // 8 = 7 segments      x xxxxxxx
        // 9 = 6 segments     x  xxxx xx
        //                111331 8687497
        */
        var symbols = new List<string> { "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg" };

        var lines = Input.GetLines(8, sample: false).Select(x =>
        {
            var parts = x.Split('|');
            return (digits: parts[0].Trim().Split(' ').Select(SortChars).ToList(), displays: parts[1].Trim().Split(' ').Select(SortChars).ToArray());
        }).ToList();

        var sum = 0L;
        foreach (var line in lines)
        {
            sum += SolveLine(line);
        }

        WriteLine(sum);

        long SolveLine((List<string> digits, string[] displays) line)
        {
            var digitIndexes = Enumerable.Repeat(-1, 10).ToArray();
            var chars = Enumerable.Range('a', 7).Select(x => (char)x).ToArray();
            const int swap_a = 0; const int swap_b = 1; const int swap_c = 2; const int swap_d = 3; const int swap_e = 4; const int swap_f = 5; const int swap_g = 6;

            // trouver les 4 chiffres avec nb de segments unique
            for (int i = 0; i < line.digits.Count; i++)
            {
                switch (line.digits[i].Length)
                {
                    case 2: digitIndexes[1] = i; break;
                    case 3: digitIndexes[7] = i; break;
                    case 4: digitIndexes[4] = i; break;
                    case 7: digitIndexes[8] = i; break;
                }
            }

            Assert(digitIndexes.Count(d => d != -1) == 4);

            // abcdefg
            // 0123456
            // table de swap : pour chaque caractère, on a le mauvais caractère affiché
            var swaps = new char[7];

            // le f est le seul utilisé 9 fois, on utilise le 1 pour le trouver
            // l'autre est forcément le c
            var digit1 = line.digits[digitIndexes[1]];
            foreach (var ch in digit1)
            {
                var count = line.digits.Count(d => d.Contains(ch));
                if (count == 9)
                {
                    // trouvé
                    swaps[swap_f] = ch;
                    swaps[swap_c] = digit1.First(c => c != ch);
                    break;
                }
            }

            Assert(swaps.Count(c => c != 0) == 2);

            // on détermine le 'a' grace au 7
            swaps[swap_a] = line.digits[digitIndexes[7]].First(c => c != swaps[swap_f] && c != swaps[swap_c]);

            // on a le c, f et a
            // le 'e' est le seul utilisé 4 fois
            swaps[swap_e] = chars.First(c => line.digits.Count(d => d.Contains(c)) == 4);

            // le 'b' est le seul utilisé 6 fois
            swaps[swap_b] = chars.First(c => line.digits.Count(d => d.Contains(c)) == 6);

            // on a le a, b, c, e, f, et les digit 1, 4, 7 et 8
            // il manque le d et le g
            Assert(swaps.Count(c => c == 0) == 2);

            // le d est le caractère non découvert présent sur le 4 et pas le 7
            for (int i = 0; i < swaps.Length; i++)
            {
                var c = chars[i];
                if (swaps.Any(s => s == c)) continue; // on ignore les digits déjà trouvés

                if (line.digits[digitIndexes[4]].Contains(c) && !line.digits[digitIndexes[7]].Contains(c))
                {
                    // trouvé
                    swaps[swap_d] = c;
                }
            }

            Assert(swaps.Count(c => c == 0) == 1);

            // il reste juste le swap_g
            swaps[swap_g] = chars.First(c => !swaps.Contains(c));

            Assert(swaps.All(c => c != 0));

            // on a la table de transposition complète => on l'inverse
            // pour chaque caractère faussé, on aura le vrai caractère à utiliser
            var swapFix = new char[7];
            foreach (var c in chars)
            {
                swapFix[c - 'a'] = (char)('a' + swaps.IndexOf(c));
            }

            // fix des caractères affichés
            var fixedDisplays = line.displays.Select(d => new string(d.Select(c => swapFix[c - 'a']).ToArray())).Select(SortChars).ToArray();

            // construction du nombre
            var result = 0L;
            for (int i = 0; i < fixedDisplays.Length; i++)
            {
                var factor = fixedDisplays.Length - i - 1;
                var fixedDisplay = fixedDisplays[i];
                var index = symbols.IndexOf(fixedDisplay);

                Assert(index != -1);

                result += index * (long)Math.Pow(10, factor);
            }

            return result;
        }

        string SortChars(string value)
        {
            return new string(value.OrderBy(c => c).ToArray());
        }
    }

    public static void Part1()
    {
        //                234567 abcdefg
        // 0 = 6 segments     x  xxx xxx
        // 1 = 2 segments x        x  x
        // 2 = 5 segments    x   x xxx x
        // 3 = 5 segments    x   x xx xx
        // 4 = 4 segments   x     xxx x
        // 5 = 5 segments    x   xx x xx
        // 6 = 6 segments     x  xx xxxx
        // 7 = 3 segments  x     x x  x
        // 8 = 7 segments      x xxxxxxx
        // 9 = 6 segments     x  xxxx xx
        //                111331 8687397
        var lines = Input.GetLines(8).Select(x =>
        {
            var parts = x.Split('|');
            return new { Digits = parts[0].Trim().Split(' '), Displays = parts[1].Trim().Split(' ') };
        });

        int count1 = 0, count4 = 0, count7 = 0, count8 = 0;
        foreach (var line in lines)
        {
            foreach (var display in line.Displays)
            {
                switch (display.Length)
                {
                    case 2: count1++; break;
                    case 4: count4++; break;
                    case 3: count7++; break;
                    case 7: count8++; break;
                }
            }
        }

        WriteLine(count1 + count4 + count7 + count8);
    }
}
