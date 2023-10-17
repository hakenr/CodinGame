using System;
using System.Collections.Generic;
using System.Linq;

public static class BlunderSolution
{
	private static char[,] map;

	private static void Main(string[] args)
	{
		string[] inputs = Console.ReadLine().Split(' ');
		int linesCount = int.Parse(inputs[0]);
		int columnsCount = int.Parse(inputs[1]);

		map = new char[columnsCount, linesCount];

		var path = new List<PathPoint>();
		var teleporters = new List<(int X, int Y)>();
		for (int i = 0; i < linesCount; i++)
		{
			string row = Console.ReadLine();
			Console.Error.WriteLine(row);
			for (int j = 0; j < columnsCount; j++)
			{
				map[j, i] = row[j];
				if (row[j] == '@')
				{
					path.Add(new PathPoint()
					{
						X = j,
						Y = i,
						State = new BlunderState() { Direction = Direction.South, DirectionInvertor = false, BreakerMode = false }
					});
				}
				else if (row[j] == 'T')
				{
					teleporters.Add((X: j, Y: i));
				}
			}
		}

		bool TryBuildPathToSuicideBooth()
		{
			while (true)
			{
				var lastPathPoint = path[^1];
				var nextPosition = GetPointInDirection(lastPathPoint.X, lastPathPoint.Y, lastPathPoint.State.Direction);
				var nextState = lastPathPoint.State;

				switch (map[nextPosition.X, nextPosition.Y])
				{
					case ' ':
					case '@':
						// pokračujeme ve směru, nic se nemění
						path.Add(new PathPoint()
						{
							X = nextPosition.X,
							Y = nextPosition.Y,
							State = nextState,
						});
						break;
					case '#':
						// nezbouratelná zeď, změna směru
						var newDirection = GetNextDirectionWhenBlocked(lastPathPoint);
						lastPathPoint.State = lastPathPoint.State with { Direction = newDirection };
						break;
					case 'X':
						// bouratelná zeď
						if (lastPathPoint.State.BreakerMode)
						{
							// pokud jsme v BreakerMode, tak zbouráme zeď a pokračujeme, jako by se jednalo o volné místo
							lastPathPoint.State = lastPathPoint.State with { BreakerModeUsed = true };
							map[nextPosition.X, nextPosition.Y] = ' ';
							goto case ' ';
						}
						else
						{
							// pokud nejsme v BreakerMode, tak pokračujeme, jako by se jednalo o nezbouratelnou zeď
							goto case '#';
						}
					case 'I':
						// inverter - nastavíme invertor směru a pokračujeme, jako by se jednalo o volné místo
						nextState = lastPathPoint.State with { DirectionInvertor = !lastPathPoint.State.DirectionInvertor };
						goto case ' ';
					case 'B':
						// pivo - přepneme BreakerMode a pokračujeme, jako by se jednalo o volné místo (+ vypneme si sledování použití BreakerMode)
						nextState = lastPathPoint.State with { BreakerMode = !lastPathPoint.State.BreakerMode, BreakerModeUsed = false };
						goto case ' ';
					case 'S':
						// south - nastavíme směr na SOUTH a pokračujeme, jako by se jednalo o volné místo
						nextState = lastPathPoint.State with { Direction = Direction.South };
						goto case ' ';
					case 'E':
						// east - nastavíme směr na EAST a pokračujeme, jako by se jednalo o volné místo
						nextState = lastPathPoint.State with { Direction = Direction.East };
						goto case ' ';
					case 'N':
						// north - nastavíme směr na NORTH a pokračujeme, jako by se jednalo o volné místo
						nextState = lastPathPoint.State with { Direction = Direction.North };
						goto case ' ';
					case 'W':
						// west - nastavíme směr na WEST a pokračujeme, jako by se jednalo o volné místo
						nextState = lastPathPoint.State with { Direction = Direction.West };
						goto case ' ';
					case 'T':
						// teleport - teleportujeme se na druhý teleport
						var otherTeleporter = teleporters[0] == (nextPosition.X, nextPosition.Y) ? teleporters[1] : teleporters[0];
						path.Add(new PathPoint()
						{
							X = otherTeleporter.X,
							Y = otherTeleporter.Y,
							State = nextState,
						});
						break;
					case '$':
						// konec, sebevražda
						return true;
				}

				// pokud je ve stávající cestě smyčka, tak končíme
				// nejprve najdeme, kdy jsme naposledy v daném bodě už byli (pokud vůbec)
				var samePoint = path.GetRange(0, path.Count - 1).LastIndexOf(path[^1]);
				if (samePoint > 0)
				{
					// zkontrolujeme, jestli jsme v této smyčce využili BreakerMode
					var breakerModeUsed = path.GetRange(samePoint, path.Count - samePoint - 1).Any(p => p.State.BreakerModeUsed);
					if (!breakerModeUsed)
					{
						// pokud ne, tak končíme, cesta se už nemění a ocitli jsme se ve stejném místě
						Console.Error.WriteLine($"{lastPathPoint.X} {lastPathPoint.Y} {lastPathPoint.State.Direction} smyčka {samePoint}");
						return false;
					}
				}
			}
		}

		if (TryBuildPathToSuicideBooth())
		{
			foreach (var pathPoint in path)
			{
				Console.WriteLine(pathPoint.State.Direction.ToString().ToUpper());
			}
		}
		else
		{
			Console.WriteLine("LOOP");
		}
	}

	private static void DumpMap(PathPoint pathPointToDisplay)
	{
		for (int i = 0; i < map.GetLength(1); i++)
		{
			for (int j = 0; j < map.GetLength(0); j++)
			{
				if ((j == pathPointToDisplay.X) && (i == pathPointToDisplay.Y))
				{
					Console.Error.Write('!');
				}
				else
				{
					Console.Error.Write(map[j, i]);
				}
			}
			Console.Error.WriteLine();
		}
	}

	private static Direction GetNextDirectionWhenBlocked(PathPoint pathPoint)
	{
		var state = pathPoint.State;

		// pořadí zkoušených směrů podle nastavení invertoru
		var directions = state.DirectionInvertor
			? new[] { Direction.West, Direction.North, Direction.East, Direction.South }
			: new[] { Direction.South, Direction.East, Direction.North, Direction.West };

		// pomocná funkce, která zjistí, jestli je na daných souřadnicích překážka
		bool IsObstacle(int x, int y)
		{
			if (map[x, y] == '#')
			{
				return true;
			}
			if ((map[x, y] == 'X') && (!state.BreakerMode))
			{
				return true;
			}
			return false;
		}

		// zkouším směry dle určeného pořadí
		foreach (var direction in directions)
		{
			var (x, y) = GetPointInDirection(pathPoint.X, pathPoint.Y, direction);
			if (!IsObstacle(x, y))
			{
				return direction;
			}
		}

		throw new InvalidOperationException("No direction available");
	}

	// pomocná funkce, která vrátí souřadnice bodu v daném směru
	private static (int X, int Y) GetPointInDirection(int x, int y, Direction direction)
	{
		return direction switch
		{
			Direction.South => (x, y + 1),
			Direction.East => (x + 1, y),
			Direction.North => (x, y - 1),
			Direction.West => (x - 1, y),
			_ => throw new Exception("Invalid direction")
		};
	}

	public record BlunderState
	{
		public Direction Direction { get; init; }
		public bool DirectionInvertor { get; init; }
		public bool BreakerMode { get; init; }
		public bool BreakerModeUsed { get; init; }
	}

	public record PathPoint
	{
		public int X { get; set; }
		public int Y { get; set; }
		public BlunderState State { get; set; }
	}

	public enum Direction
	{
		South = 0,
		East = 1,
		North = 2,
		West = 3
	}
}