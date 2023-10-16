using System;
using System.Collections.Generic;

internal class AbcdSolution
{
	private static void Main(string[] args)
	{
		int n = int.Parse(Console.ReadLine());
		var grid = new char[n, n];
		var starts = new List<(int, int)>();
		for (int i = 0; i < n; i++)
		{
			string m = Console.ReadLine();
			for (int j = 0; j < n; j++)
			{
				grid[j, i] = m[j];
				if (m[j] == 'a')
				{
					starts.Add((j, i));
				}
			}
		}

		// najdeme cestu
		var path = new List<(int, int)>();
		foreach (var start in starts)
		{
			if (IsPath(grid, path, start))
			{
				break;
			}
			path.Clear();
		}

		// vypíšeme výsledek
		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < n; j++)
			{
				if (path.Contains((j, i)))
				{
					Console.Write(grid[j, i]);
				}
				else
				{
					Console.Write('-');
				}
			}
			Console.WriteLine();
		}
	}

	private static bool IsPath(char[,] grid, List<(int x, int y)> path, (int x, int y) next)
	{
		// pokud je next mimo grid, není to cesta
		if ((next.x < 0) || (next.x >= grid.GetLength(0)) || (next.y < 0) || (next.y >= grid.GetLength(1)))
		{
			return false;
		}

		// pokud je to první bod (a), přidáme ho
		if ((path.Count == 0) && (grid[next.x, next.y] == 'a'))
		{
			path.Add(next);
		}
		else
		{
			// pokud je to další bod v pořadí, přidáme ho do zkoumané cesty
			var last = path[^1];
			if (grid[next.x, next.y] == (char)(grid[last.x, last.y] + 1))
			{
				path.Add(next);
			}
			else
			{
				// jinak to není cesta
				return false;
			}
		}

		// pokud jsme našli z, je to cesta
		if (grid[next.x, next.y] == 'z')
		{
			return true;
		}

		// pokud je některý z okolních bodů cesta, je to cesta
		if (IsPath(grid, path, (next.x - 1, next.y))
			|| IsPath(grid, path, (next.x + 1, next.y))
			|| IsPath(grid, path, (next.x, next.y - 1))
			|| IsPath(grid, path, (next.x, next.y + 1)))
		{
			return true;
		}

		// jinak to nebyla cesta a vrátíme se o krok zpět
		path.RemoveAt(path.Count - 1);
		return false;
	}
}