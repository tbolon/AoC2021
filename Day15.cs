using static ProgramHelper;

namespace AoC2021;

internal static class Day15
{
    public static void Part2()
    {
        // thanks to the incredible https://www.redblobgames.com/pathfinding/a-star/introduction.html
        // initial grid
        var smallGrid = Input.GetLines(15, sample: false).AsGridOfBytes(byte.MaxValue);

        // fill final grid
        var grid = new Grid<int>(smallGrid.Width * 5, smallGrid.Height * 5);
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                foreach (var (point, value) in smallGrid)
                {
                    var newX = (smallGrid.Width * x) + point.X;
                    var newY = (smallGrid.Height * y) + point.Y;
                    var newValue = ((value + x + y - 1) % 9) + 1;
                    if (x == 0 && y == 0)
                    {
                        Assert(newValue == value);
                    }

                    grid[newX, newY] = newValue;
                }
            }
        }

        var start = Point.Empty;
        var end = new Point(grid.XMax, grid.YMax);
        Point current;

        // contient pour chaque point le coût depuis le départ
        // on va y ajouter tous les points que l'on croise avec le coût pour y arriver
        // si cette liste est vide, on a perdu (aucun chemin)
        // dès que l'on trouve la sortie on arrête d'explorer cette pile
        var frontier = new PriorityQueue<Point, int>();
        frontier.Enqueue(start, 0);

        // contient les chemins calculés les plus optimisés entre les différents noeuds du graphe
        var cameFrom = new Dictionary<Point, Point>();

        // pour chaque point, mémorise le coût pour arriver à ce point depuis le départ (sans le cout du point lui même)
        var costSoFar = new Dictionary<Point, int>
        {
            [start] = 0
        };

        // build paths
        while (frontier.Count != 0)
        {
            // on examine le point le plus intéressant pour l'instant
            // c'est l'avantage de la PriorityQueue : elle dépile les éléments de plus faible priorité en premier
            // on va donc examiner les chemins les moins couteux en priorité
            current = frontier.Dequeue();

            // trouvé
            if (current == end)
            {
                break;
            }

            // on examine les voisins
            foreach (var next in current.Neighbors(grid))
            {
                // le coût est celui d'avant ce point + coût du point
                var newCost = costSoFar[current] + grid[current];

                // si le nouveau point n'a jamais été exploré, ou que le cout pour y arriver est plus faible
                // alors ce point est intéressant et doit être conservé
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    // on stocke le cout jusqu'au point
                    costSoFar[next] = newCost;

                    // on mémorise le point avec son coût comme étant à explorer
                    var priority = newCost;
                    frontier.Enqueue(next, priority);

                    // on stocke le chemin inverse
                    cameFrom[next] = current;
                }
            }
        }

        // create reverse path
        current = end;
        var path = new List<Point>();
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start);
        path.Reverse();

        grid.VisitConsole((p, x) =>
        {
            if (!path.Contains(p))
            {
                Write((char)(x + '0'));
            }
            else
            {
                Write((char)(x + '0'), ConsoleColor.Green);
            }
        });

        WriteLine(path.Sum(p => p == start ? 0 : grid[p]));
    }

    public static void Part1()
    {
        // thanks to the incredible https://www.redblobgames.com/pathfinding/a-star/introduction.html
        // initial grid
        var grid = Input.GetLines(15, sample: true).AsGridOfBytes(byte.MaxValue);

        var start = Point.Empty;
        var end = new Point(grid.XMax, grid.YMax);
        Point current;

        var frontier = new PriorityQueue<Point, int>();
        frontier.Enqueue(start, 0);

        var cameFrom = new Dictionary<Point, Point>();
        //cameFrom[start] = null;

        var costSoFar = new Dictionary<Point, int>
        {
            [start] = 0
        };

        // build paths
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current == end)
            {
                break;
            }

            foreach (var next in current.Neighbors(grid))
            {
                var newCost = costSoFar[current] + grid[current];

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    var priority = newCost;
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        // create reverse path
        current = end;
        var path = new List<Point>();
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start);
        path.Reverse();

        grid.VisitConsole(x => Console.Write((char)(x + '0')));

        ReadKey();

        Clear();
        grid.VisitConsole((p, x) =>
        {
            if (!path.Contains(p))
            {
                Write((char)(x + '0'));
            }
            else
            {
                Write('o', ConsoleColor.Green);
            }
        });

        WriteLine(path.Sum(p => p == start ? 0 : grid[p]));
    }
}
