using System;
using System.Collections.Generic;

// https://www.codingame.com/ide/puzzle/flood-fill-example
// Given a grid consisting of ‘.’ (visitable points) and ‘#’ (un-visitable points),
// and other entities for the defence towers, output a map showing the area coverage
// of each tower, based on distance.
internal class Solution
{
	private static void Main(string[] args)
	{
		int W = int.Parse(Console.ReadLine());
		int H = int.Parse(Console.ReadLine());
		var grid = new Point[W, H];
		var queue = new Queue<Point>();
		int towerCounter = 0;
		var towers = new List<char>();
		towers.Add('+'); // dummy tower 0 = shared tower

		// načteme vstupní data a označíme všechny věže
		for (int i = 0; i < H; i++)
		{
			string line = Console.ReadLine();
			for (int j = 0; j < W; j++)
			{
				var point = new Point { X = j, Y = i, Symbol = line[j] };
				grid[j, i] = point;

				// pokud je to věž, přidáme ji do fronty
				if ((point.Symbol != '.') && (point.Symbol != '#'))
				{
					towers.Add(point.Symbol);
					point.NearestTowerId = ++towerCounter;
					point.Distance = 0;
					queue.Enqueue(point);
				}
			}
		}

		// projdeme frontu a všechny body v okolí věže označíme
		while (queue.Count > 0)
		{
			var point = queue.Dequeue();
			int x = point.X;
			int y = point.Y;

			void ExaminePoint(Point current)
			{
				// pokud se tam dá vstoupit
				if (current.Symbol == '.')
				{
					if ((current.NearestTowerId is not null)
						&& (current.Distance == point.Distance + 1)
						&& (current.NearestTowerId != point.NearestTowerId))
					{
						// pokud je to bod navštívený z jiné věže ve stejné vzdálenosti
						// označíme to jako společně dosažitelný bod
						current.NearestTowerId = 0;
					}
					else if (current.NearestTowerId is null) // pokud je to prázdný bod
					{
						current.Distance = point.Distance + 1;
						current.NearestTowerId = point.NearestTowerId;
						queue.Enqueue(current);
					}
				}
			}

			// všechny body v okolí prozkoumáme (pokud nejsou mimo grid)
			if (x > 0)
			{
				var left = grid[x - 1, y];
				ExaminePoint(left);
			}
			if (x < W - 1)
			{
				var right = grid[x + 1, y];
				ExaminePoint(right);
			}
			if (y > 0)
			{
				var up = grid[x, y - 1];
				ExaminePoint(up);
			}
			if (y < H - 1)
			{
				var down = grid[x, y + 1];
				ExaminePoint(down);
			}
		}

		// vypíšeme výsledek
		for (int i = 0; i < H; i++)
		{
			for (int j = 0; j < W; j++)
			{
				var point = grid[j, i];
				if (point.NearestTowerId is not null)
				{
					// pokud je to dosažitelný bod, vypíšeme symbol věže
					Console.Write(towers[point.NearestTowerId.Value]);
				}
				else
				{
					// jinak vypíšeme původní symbol
					Console.Write(point.Symbol);
				}
			}
			Console.WriteLine();
		}
	}

	private class Point
	{
		public int X { get; set; }
		public int Y { get; set; }

		public char Symbol { get; set; } // . = empty, # = wall, other = tower
		public int Distance { get; set; } // distance from the nearest tower
		public int? NearestTowerId { get; set; } // null = not visited/reachable, 0 = shared tower, 1+ = tower number
	}
}