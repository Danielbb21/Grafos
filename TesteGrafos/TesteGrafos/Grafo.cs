using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TesteGrafos
{
    public class Grafo
    {
        public int[,] MatrizAdjacencia { get; set; }
        public List<Vertice> Conteudo { get; set; }
        public bool IsDirigido { get; set; }
		public int[][] distanceMatrix { get; set; }

		public LinkedList<Tuple<int, int>>[] adjacencyList;

		public Grafo(int vertices) 
        {
            this.IsDirigido = false;
            this.Conteudo = new List<Vertice>();

			//Inicialisa a matriz de adjacencia
			this.MatrizAdjacencia = new int[vertices, vertices];

			adjacencyList = new LinkedList<Tuple<int, int>>[vertices];

			for (int i = 0; i < adjacencyList.Length; ++i)
			{
				adjacencyList[i] = new LinkedList<Tuple<int, int>>();
			}

			IniciarMatriz(vertices);
		}

		public void IniciarMatriz(int vertices)
		{
			//Zera todos os campos da matriz e inicializa os vertices dentro do grafo
			for (int i = 0; i < vertices; i++)
			{
				for (int j = 0; j < vertices; j++)
				{
					this.MatrizAdjacencia[i, j] = 0;
				}

				Vertice v = new Vertice();
				v.Nametag = i + 1;
				v.Adjacencia = new List<Aresta>();

				this.Conteudo.Add(v);
			}
		}

		public void CriarLigacao(int numDefinido1, int numDefinido2, int peso) 
		{
			//Cria a aresta que liga os dois vertices
			Aresta Ligacao = new Aresta();
			Ligacao.Entrada = this.Conteudo.ElementAt(numDefinido1);
			Ligacao.Saida = this.Conteudo.ElementAt(numDefinido2);
			Ligacao.Peso = peso;

			//Define na matriz que há uma ligacao
			this.MatrizAdjacencia[numDefinido1, numDefinido2] = Ligacao.Peso;

			//Adiciona a ligacao no primeiro elemento
			this.Conteudo.ElementAt(numDefinido1).Adjacencia.Add(Ligacao);

			//Caso nao for dirigido ele cria uma outra ligacao para ser jogada no segundo vertice
			if (!this.IsDirigido)
			{
				Aresta a = new Aresta();
				a.Entrada = this.Conteudo.ElementAt(numDefinido2);
				a.Saida = this.Conteudo.ElementAt(numDefinido1);

				this.MatrizAdjacencia[numDefinido2, numDefinido1] = Ligacao.Peso;

				this.Conteudo.ElementAt(numDefinido2).Adjacencia.Add(a);
			}

			this.addEdgeAtEnd(Ligacao.Entrada.Nametag - 1, Ligacao.Saida.Nametag, 1);
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

		public void ResetarCheque()
		{
			foreach (Vertice v in this.Conteudo)
			{
				v.IsChecked = false;
			}
		}

		public int ContadorComponentes()
		{
			int contadorComponentes = 0;
			foreach (Vertice v in this.Conteudo)
			{
				if (!v.IsChecked)
				{
					ChequeNoGrafo(v);
					contadorComponentes += 1;
				}
			}
			return contadorComponentes;
		}

		public bool ChequeLigacoes(Vertice v, int saida)
		{
			foreach (Aresta a in v.Adjacencia)
			{
				if (a.Saida.Nametag == saida)
				{
					return false;
				}
			}
			return true;
		}

		private void ChequeNoGrafo(Vertice v)
		{
			v.IsChecked = true;
			foreach (Aresta a in v.Adjacencia)
			{
				if (!a.Saida.IsChecked)
				{
					ChequeNoGrafo(a.Saida);
				}
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

		public void GravarTxt(bool IsDirigido)
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			//criando arquivo
			string filePath = localFolder.Path + "\\" + "teste.txt";
			string DataFile = "*Vertices ";
			//Cria o arquivo se ele não existir
			StreamWriter sw;
			if (!File.Exists(filePath))
			{
				sw = File.CreateText(filePath);
				sw.Close();
			}

			//Não grava-se o \n no arquivo
			//Using isola uma declaração que nao funcionara fora dele
			//Escreve no arquivo pulando linha
			using (sw = File.CreateText(filePath))
			{
				//concatenação da string
				List<Aresta> ListadeArestas = new List<Aresta>();
				

				DataFile += this.Conteudo.Count() + "\n";
				
				foreach(Vertice v in Conteudo)
                {
					DataFile += v.Nametag + " " + v.Nametag + "\n";
					//guarda todas as arestas de todos os vertices
					ListadeArestas.AddRange(v.Adjacencia);
                }
                if (IsDirigido) //checa se o grafo dirigido
                {
					//grafo dirigido
					DataFile += "*Arcs\n";
                    foreach (var aresta in ListadeArestas)
                    {
						DataFile += aresta.Entrada.Nametag + " " + aresta.Saida.Nametag + " " + aresta.Peso + "\n";
                    }
                }
                else
                {
					//grafo não dirigido
					DataFile += "*Edges\n";
					//remove arestas repetidas
					
					for(int  i=0; i<ListadeArestas.Count(); i++)
                    {
						Aresta aresta = ListadeArestas[i];
						Aresta a = aresta.ArestaAoContrario();
						Aresta removivel = ListadeArestas.Find(x => x.Entrada == a.Entrada && x.Saida == a.Saida);

						if (removivel!=null)
                        {
							ListadeArestas.Remove(removivel);
                        }
                    }
					foreach (var aresta in ListadeArestas)
					{
						DataFile += aresta.Entrada.Nametag + " " + aresta.Saida.Nametag + " " + aresta.Peso + "\n";
					}
				}
				sw.WriteLine(DataFile);
				sw.Close();
			}

			//Leitura dos dados
			using (StreamReader sr = File.OpenText(filePath))
			{
				string s;
				while ((s = sr.ReadLine()) != null)
				{
					Console.WriteLine(s);
				}
			}
		}
	}
}
