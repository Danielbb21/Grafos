using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteGrafos
{
    public class Grafo
    {
        public int[,] MatrizAdjacencia { get; set; }
        public List<Vertice> Conteudo { get; set; }
        public bool IsDirigido { get; set; }
		public int[][] distanceMatrix { get; set; }
		public int[][] ListaAdjacencia { get; set; }

		public LinkedList<Tuple<int, int>>[] adjacencyList;

		public Grafo(int vertices) 
        {
            this.IsDirigido = false;
            this.Conteudo = new List<Vertice>();

			adjacencyList = new LinkedList<Tuple<int, int>>[vertices];

			for (int i = 0; i < adjacencyList.Length; ++i)
			{
				adjacencyList[i] = new LinkedList<Tuple<int, int>>();
			}
		}

		// Appends a new Edge to the linked list
		public void addEdgeAtEnd(int startVertex, int endVertex, int weight)
		{
			adjacencyList[startVertex].AddLast(new Tuple<int, int>(endVertex, weight));
		}


		// Returns a copy of the Linked List of outward edges from a vertex
		public LinkedList<Tuple<int, int>> this[int index]
		{
			get
			{
				LinkedList<Tuple<int, int>> edgeList
							   = new LinkedList<Tuple<int, int>>(adjacencyList[index]);

				return edgeList;
			}
		}

		public void CreateDM(int vertices)
		{
			distanceMatrix = new int[vertices][];
			for (int i = 0; i < vertices; i++)
			{
				distanceMatrix[i] = Dijkstra(i, vertices);
			}
		}

		private int TakeMin(List<int> queue, int[] dist)
		{
			int lesser = 0;
			foreach (int vertex in queue)
			{
				lesser = vertex;
				break;
			}

			foreach (int vertex in queue)
			{
				if (dist[lesser] > dist[vertex])
					lesser = vertex;
			}
			return lesser;
		}

		public int[] Dijkstra(int start, int vertices)
		{
			int infinite = 9999;
			List<int> queue = new List<int>();
			int[] pred = new int[vertices];
			int[] dist = new int[vertices];
			for (int vertex = 0; vertex < pred.Length; vertex++)
			{
				pred[vertex] = -1;
				if (vertex != start)
					dist[vertex] = infinite;
				queue.Add(vertex);
			}
			while (queue.Count > 0)
			{
				int u = TakeMin(queue, dist);
				queue.Remove(u);
				int[] neighbors = ReturnNeighbors(u, vertices);
				for (int v = 0; v < neighbors.Length; v++)
				{
					if (neighbors[v] >= 1)
					{
						int aux = neighbors[v] + dist[u];
						if (aux < dist[v])
						{
							dist[v] = aux;
							pred[v] = u;
						}
					}
				}
			}
			//Caso queira o numero bote dist
			//Caso queira o caminho bote pred
			return pred;
		}

		private int[] ReturnNeighbors(int vertex, int vertices)
		{
			int[] neighbors = new int[vertices];
			for (int i = 0; i < neighbors.Length; i++)
			{
				neighbors[i] = MatrizAdjacencia[vertex, i];
			}
			return neighbors;
		}
	}
}
