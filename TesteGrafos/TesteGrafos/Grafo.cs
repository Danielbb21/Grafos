using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace TesteGrafos
{
    public class Grafo
    {
        public int[,] MatrizAdjacencia { get; set; }
        public List<Vertice> Conteudo { get; set; }
        public bool IsDirigido { get; set; }
		public int[][] distanceMatrix { get; set; }

		public LinkedList<Tuple<int, int>>[] adjacencyList;

		//Construtor e inicializador de componentes
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

		//Inicializa a matriz de adjacência
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

		//Adiciona um vértice na matriz de acordo com o tipo de grafo (dirigido ou não dirigido)
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

		//Metodo para marcar todos os vertices como não checados após de uma busca
		public void ResetarCheque()
		{
			foreach (Vertice v in this.Conteudo)
			{
				v.IsChecked = false;
			}
		}
		
		//Metodo para contar os componentes dentro de um grafo a partir da busca de profundidade
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

		//Confere a existência da aresta
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

		//Metodo recursivo para conferir todos os vertices dentro do grafo a partir da busca de profundidade
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

		//Metodo para criar a matriz de distancia a partir do algoritmo de Dijkstra
		public void CreateDM(int vertices)
		{
			distanceMatrix = new int[vertices][];
			for (int i = 0; i < vertices; i++)
			{
				distanceMatrix[i] = Dijkstra(i, vertices);
			}
		}

		//Metodo complementar do metodo Dijkstra para pegar o menor valor dentro da matriz de distancia
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

		//Metodo para retornar o caminho minimo entre dois vertices
		public int[] Dijkstra(int start, int vertices)
		{
			//Marca o maior valor inicial
			int infinite = 9999;

			//Inicia a lista de espera
			List<int> queue = new List<int>();

			//Inicia o vetor de predecessores
			int[] pred = new int[vertices];

			//Inicia o vetor das distancias
			int[] dist = new int[vertices];

			//Loop para marcar todos os pontos com a distancia infinite
			for (int vertex = 0; vertex < pred.Length; vertex++)
			{
				pred[vertex] = -1;
				if (vertex != start)
					dist[vertex] = infinite;
				queue.Add(vertex);
			}

			//Loop para encontrar a distancia entre todos os adjacentes de um vertice
			while (queue.Count > 0)
			{
				//Pega o menor valor entre os elementos da matriz de distancia
				int u = TakeMin(queue, dist);

				//Remove o elemento da lista de espera
				queue.Remove(u);

				//Inicia o vetor de adjacentes de um vertice
				int[] neighbors = ReturnNeighbors(u, vertices);

				//Loop para checar todos os elementos dentro do vetor de adjacentes 
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

		//Retorna uma lista dos vertices adjacentes ao vertice vertex com base na matriz de adjacencia
		private int[] ReturnNeighbors(int vertex, int vertices)
		{
			int[] neighbors = new int[vertices];
			for (int i = 0; i < neighbors.Length; i++)
			{
				neighbors[i] = MatrizAdjacencia[vertex, i];
			}
			return neighbors;
		}

		//Grava o grafo em Texto no formato pajek
		public async void GravarTxt(bool IsDirigido)
		{
			string DataFile = "*Vertices ";

			var savePicker = new FileSavePicker();
			savePicker.DefaultFileExtension = ".txt";
			savePicker.FileTypeChoices.Add(".txt", new List<string> { ".txt" });
			savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			savePicker.SuggestedFileName = "Grafo" + ".txt";

			var saveFile = await savePicker.PickSaveFileAsync();

			//Não grava-se o \n no arquivo
			//Using isola uma declaração que nao funcionara fora dele
			//Escreve no arquivo pulando linha
			using (var filestream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
			{
				//concatenação da string
				List<Aresta> ListadeArestas = new List<Aresta>();
				

				DataFile += this.Conteudo.Count() + "\n";
				
				foreach(Vertice v in Conteudo)
                {
					DataFile += v.Nametag + " " + "\"" + v.Nametag + "\"" + "\n";
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

			}

			if (saveFile != null)
			{
				CachedFileManager.DeferUpdates(saveFile);
				await FileIO.WriteTextAsync(saveFile, DataFile);
				FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(saveFile);
			}

			////Leitura dos dados
			//using (StreamReader sr = File.OpenText(filePath))
			//{
			//	string s;
			//	while ((s = sr.ReadLine()) != null)
			//	{
			//		Console.WriteLine(s);
			//	}
			//}
		}
	}
}
