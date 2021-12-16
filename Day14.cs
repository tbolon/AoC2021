using static ProgramHelper;

namespace AoC2021;

internal static class Day14
{
    public static void Part2()
    {
        const int maxLevel = 40;
        var lines = Input.GetLines(14, sample: false).ToArray();
        var polymerTemplate = lines[0].ToList();

        // on stocke les permutations dans un dictionaire uint => char ou la clé correspond au résultat de AsKey()
        var permutations = lines.Skip(1).Select(l => l.Split('-')).ToDictionary(a => AsKey(a[0][0], a[0][1]), a => a[1][2]);

        // on va optimiser la recherche récursive en stockant le tableau de score pour un ensemble paire + niveau de profondeur de récursivité
        // sans ça, le calcul prend trop de temps.
        //
        // attention: le raccourci ne contient le score que pour la partie *générée* de la paire, par ex le score de (AB) n'inclut pas la présence de 'A' et 'B'.
        var shortcuts = new Dictionary<(uint, int), long[]>();

        // le tableau de score est un tableau avec 0 = 'A'
        // on l'initialise avec le nb de caractères du template vu que la méthode Visit() ne renvoie que le score de la partie *générée*.
        var finalScore = new long[26];
        foreach (var c in polymerTemplate)
        {
            finalScore[c - 'A']++;
        }

        // on va ensuite analyser chaque paire du template, et descendre en récursif x fois (= niveau)
        // pour analyser chaque paire produite
        for (int x = 1; x < polymerTemplate.Count; x++)
        {
            Visit(polymerTemplate[x - 1], polymerTemplate[x], 1, finalScore);
        }

        // score final = nb d'occurences du caractère le plus présent - nb d'occurence du caractère le moins présent
        WriteLine(finalScore.Where(p => p != 0).Max() - finalScore.Where(p => p != 0).Min());

        // obtient une clé unique sur 32 bits à partir de 2 caractères de 16 bits
        // utilisé pour essayer d'optimiser le lookup, pas sur que ce soit finalement utile
        uint AsKey(char c1, char c2) => ((uint)(c1 - 'A')) << 16 | ((uint)(c2 - 'A'));

        // visite une paire AB avec le niveau de profondeur "level" et ajoute le score dans le tableau donné
        // attention: le score n'est calculé que pour la partie *générée*
        void Visit(char a, char b, int level, long[] parentScore)
        {
            if (level > maxLevel)
            {
                return;
            }

            var key = AsKey(a, b);

            // si le résultat de cette combinaison (paire, niveau) a déjà été calculée on la réutilise
            if (shortcuts.TryGetValue((key, level), out var shortcut))
            {
                Add(shortcut, parentScore);
                return;
            }

            // nouveau score à calculer pour cette paire+profondeur
            var score = new long[26];

            // on recherche la lettre à insérer AB => AXB
            var permutation = permutations[key];

            // ajout du score de la permutation ajoutée
            score[permutation - 'A']++;

            // on ajoute le score de toutes les itérations successives pour la partie gauche (AX)
            Visit(a, permutation, level + 1, score);

            // on ajoute le score de toutes les itérations successives pour la partie droite (XB)
            Visit(permutation, b, level + 1, score);

            // on met en cache le score local (AB, level)
            shortcuts[(key, level)] = score;

            // on ajoute le score local au score parent
            Add(score, parentScore);
        }

        void Add(long[] score, long[] parentScore)
        {
            for (int i = 0; i < parentScore.Length; i++)
            {
                parentScore[i] += score[i];
            }
        }
    }

    public static void Part1()
    {
        var lines = Input.GetLines(14, sample: false).ToArray();
        var polymer = lines[0].ToList();
        var permutations = lines.Skip(1).Select(l => l.Split('-')).ToDictionary(a => a[0].Trim(), a => a[1][2]);

        Stack<char> insertions = new();

        for (int i = 0; i < 10; i++)
        {
            insertions.Clear();

            for (int x = 1; x < polymer.Count; x++)
            {
                var pair = string.Concat(polymer[x - 1], polymer[x]);
                var insertion = permutations[pair];
                insertions.Push(insertion);
            }

            Assert(insertions.Count == polymer.Count - 1);

            for (int x = insertions.Count; x >= 1; x--)
            {
                polymer.Insert(x, insertions.Pop());
            }
        }

        var score = polymer.Aggregate(new Dictionary<char, int>(), (cumul, c) =>
        {
            cumul[c] = 1 + (cumul.TryGetValue(c, out var current) ? current : 0);
            return cumul;
        });

        WriteLine(score.Max(p => p.Value) - score.Min(p => p.Value));
    }
}
