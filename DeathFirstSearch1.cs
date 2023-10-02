using System;
using System.Collections.Generic;
using System.Linq;

internal class Player
{
	private static List<Node> nodes = new();

	private static void Main(string[] args)
	{
		string[] inputs;
		inputs = Console.ReadLine().Split(' ');
		int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
		int L = int.Parse(inputs[1]); // the number of links
		int E = int.Parse(inputs[2]); // the number of exit gateways

		// založíme uzly
		for (int i = 0; i < N; i++)
		{
			nodes.Add(new Node { Id = i, Neighbors = new List<Node>() });
		}

		// vytvoříme hrany
		for (int i = 0; i < L; i++)
		{
			inputs = Console.ReadLine().Split(' ');
			int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
			int N2 = int.Parse(inputs[1]);
			nodes[N1].Neighbors.Add(nodes[N2]);
			nodes[N2].Neighbors.Add(nodes[N1]);
		}

		// označíme uzly, které jsou východy
		for (int i = 0; i < E; i++)
		{
			int EI = int.Parse(Console.ReadLine()); // the index of a gateway node
			nodes[EI].IsGateway = true;
		}

		// game loop
		while (true)
		{
			int SI = int.Parse(Console.ReadLine()); // The index of the node on which the Bobnet agent is positioned this turn
			var node = nodes[SI];

			// najdeme nejbližší východ a odpojíme ho (BFS)

			var queue = new Queue<Node>();
			var visited = new HashSet<Node>();

			// označíme startovní uzel jako navštívený a přidáme do fronty
			visited.Add(node);
			queue.Enqueue(node);

			while (queue.Count > 0)
			{
				// vezmeme první uzel z fronty a označíme ho jako navštívený
				var current = queue.Dequeue();
				visited.Add(current);

				// pokud je východ, odpojíme ho
				if (current.IsGateway)
				{
					Node removed;
					// pokud sousedí s východem, odpojíme ho
					if (current.Neighbors.Contains(node))
					{
						removed = node;
					}
					else // jinak odpojíme první navštívený uzel
					{
						removed = current.Neighbors.First(n => visited.Contains(n));
					}
					current.Neighbors.Remove(removed);
					removed.Neighbors.Remove(current);

					Console.WriteLine($"{current.Id} {removed.Id}");
					break;
				}

				// přidáme sousedy do fronty
				foreach (var neighbor in current.Neighbors.Where(n => !visited.Contains(n)))
				{
					queue.Enqueue(neighbor);
				}
			}
		}
	}

	public class Node
	{
		public int Id { get; set; }
		public List<Node> Neighbors { get; set; }
		public bool IsGateway { get; set; }
	}
}