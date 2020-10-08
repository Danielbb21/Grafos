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

		public Grafo() 
        {
            this.IsDirigido = false;
            this.Conteudo = new List<Vertice>();
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
			return dist;
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
