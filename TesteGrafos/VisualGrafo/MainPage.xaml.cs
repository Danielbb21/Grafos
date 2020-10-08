using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TesteGrafos;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace VisualGrafo
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Grafo grafoCriado = new Grafo();
        private int vertices = 0;
        private int arestas = 0;
        private int contadorComponentes = 0;

        public MainPage()
        {
            this.InitializeComponent();
            
            SelecionarPagina.Click += Btn_SelecionarPagina;
            DefinicaoVertices.Click += Btn_DefinicaoVertices;
            CriarAresta.Click += Btn_CriarAresta;
            RadioDirigido.Click += Radio_ChangeValue;
            RadioNaoDirigido.Click += Radio_ChangeValue;
            ClearGrafo.Click += Btn_ClearGrafo;
            ExibirInformacoes.Click += Btn_ExibirInformacoes;
            ExibirTutorialCriar.Click += Btn_ExibirTutorialCriar;
            EncontrarCaminho.Click += Btn_EncontrarCaminho;
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        
        //Conteudo do canvas do menu de seleção

        private void Btn_SelecionarPagina(object sender, RoutedEventArgs e)
        {
            if (Selecao.SelectionBoxItem.ToString() == "Criar grafo")
            {
                EsconderMenus();

                BlocoCriarGrafo.IsHitTestVisible = true;
                BlocoCriarGrafo.Opacity = 100;

                Titulo.Text = "GRAFOS: CRIAR GRAFO";
            }
            else if (Selecao.SelectionBoxItem.ToString() == "Informações sobre o grafo")
            {
                EsconderMenus();

                BlocoInformacoesGrafo.IsHitTestVisible = true;
                BlocoInformacoesGrafo.Opacity = 100;

                Titulo.Text = "GRAFOS: INFORMAÇÕES DO GRAFO";
            }
            else if (Selecao.SelectionBoxItem.ToString() == "Caminho mínimo entre vértices")
            {
                EsconderMenus();

                BlocoCaminhoMinimo.IsHitTestVisible = true;
                BlocoCaminhoMinimo.Opacity = 100;

                Titulo.Text = "GRAFOS: CAMINHO MINIMO ENTRE VERTICES";
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        //Conteudo para o canvas de construir grafo

        private void Btn_DefinicaoVertices(object sender, RoutedEventArgs e)
        {
            //Primeiro checa se o campo ficou vazio
            if(NumVertices.Text != null)
            {
                //Define o numero de vertices com base no campo
                vertices = int.Parse(NumVertices.Text);
                //Desbloqueia a interação com a insercao de arestas
                MenuArestas.IsHitTestVisible = true;

                //Inicialisa a matriz de adjacencia
                grafoCriado.MatrizAdjacencia = new int[vertices, vertices];

                //Zera todos os campos da matriz e inicializa os vertices dentro do grafo
                for (int i = 0; i < vertices; i++)
                {
                    for (int j = 0; j < vertices; j++)
                    {
                        grafoCriado.MatrizAdjacencia[i, j] = 0;
                    }

                    Vertice v = new Vertice();
                    v.Nametag = i + 1;
                    v.Adjacencia = new List<Aresta>();

                    grafoCriado.Conteudo.Add(v);
                }
                Mensagem("O grafo foi criado com " + vertices +" vertices.", "GRAFO CRIADO");
                DefinicaoVertices.IsHitTestVisible = false;
            }
        }
        private void Btn_CriarAresta(object sender, RoutedEventArgs e) 
        {
            int numDefinido1, numDefinido2;
            //Primeiro checa se os campos estão vazios
            if(BoxVertice1.Text != "" && BoxVertice2.Text != "") 
            {
                numDefinido1 = int.Parse(BoxVertice1.Text) - 1;
                numDefinido2 = int.Parse(BoxVertice2.Text) - 1;
                //Depois checa se eles são validos (Valor menor que numero de vertices e maior que 0)
                if (numDefinido1 < vertices && numDefinido2 < vertices) 
                {
                    if(numDefinido1 >= 0 && numDefinido2 >= 0) 
                    {
                        //Depois checa se a ligação ja exite
                        if (ChequeLigacoes(grafoCriado.Conteudo.ElementAt(numDefinido1) ,numDefinido2 + 1)) 
                        {
                            //Cria a aresta que liga os dois vertices
                            Aresta Ligacao = new Aresta();
                            Ligacao.Entrada = grafoCriado.Conteudo.ElementAt(numDefinido1);
                            Ligacao.Saida = grafoCriado.Conteudo.ElementAt(numDefinido2);

                            //Define na matriz que há uma ligacao
                            grafoCriado.MatrizAdjacencia[numDefinido1, numDefinido2] = 1;

                            //Adiciona a ligacao no primeiro elemento
                            grafoCriado.Conteudo.ElementAt(numDefinido1).Adjacencia.Add(Ligacao);

                            //Caso nao for dirigido ele cria uma outra ligacao para ser jogada no segundo vertice
                            if (!grafoCriado.IsDirigido)
                            {
                                Aresta a = new Aresta();
                                a.Entrada = grafoCriado.Conteudo.ElementAt(numDefinido2);
                                a.Saida = grafoCriado.Conteudo.ElementAt(numDefinido1);

                                grafoCriado.MatrizAdjacencia[numDefinido2, numDefinido1] = 1;

                                grafoCriado.Conteudo.ElementAt(numDefinido2).Adjacencia.Add(a);
                            }

                            //Cria um textblock pra ser exibido a ligacao no stackpanel
                            TextBlock NovaAresta = new TextBlock();
                            NovaAresta.HorizontalAlignment = HorizontalAlignment.Center;
                            NovaAresta.FontSize = 20;
                            NovaAresta.Text = (numDefinido1 + 1) + "   -   " + (numDefinido2 + 1);

                            arestas += 1;

                            //Caso for maior que 8 ele removera a exibicao da ligacao mais antiga para caber a nova
                            if (ArestasAdicionadas.Children.Count == 8)
                            {
                                ArestasAdicionadas.Children.RemoveAt(0);
                            }

                            //Adiciona no stackpanel
                            ArestasAdicionadas.Children.Add(NovaAresta);
                        }
                        else
                        {
                            Mensagem("A ligação já está presente no grafo.", "ERRO: Ligação já existe");
                        }
                    }
                    else
                    {
                        Mensagem("Vertice invalido para o grafo criado.", "ERRO: Vertice inválido");
                    }
                }
                else
                {
                    Mensagem("Vertice invalido para o grafo criado.", "ERRO: Vertice inválido");
                }
            }
            else 
            {
                Mensagem("É necessário preencher todos os campos para inserir a aresta.", "ERRO: Campos vazios");
            }
        }
        private void Radio_ChangeValue(object sender, RoutedEventArgs e) 
        {
            if (RadioDirigido.IsChecked.Equals(true)) 
            {
                grafoCriado.IsDirigido = true;
            }
            else 
            {
                grafoCriado.IsDirigido = false;
            }
        }
        private void Btn_ClearGrafo(object sender, RoutedEventArgs e) 
        {
            grafoCriado.Conteudo.Clear();
            ArestasAdicionadas.Children.Clear();
            MenuArestas.IsHitTestVisible = false;
            DefinicaoVertices.IsHitTestVisible = true;
            vertices = 0;
            arestas = 0;
            contadorComponentes = 0;
        }
        private void Btn_ExibirTutorialCriar(object sender, RoutedEventArgs e) 
        {
            TutorialCriarGrafo.IsOpen = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        //Conteudo para o canvas das informações do grafo

        private void Btn_ExibirInformacoes(object sender, RoutedEventArgs e) 
        {
            ContadorComponentes();
            ContComp.Text = contadorComponentes.ToString();
            ContComp.HorizontalAlignment = HorizontalAlignment.Center;

            contadorComponentes = 0;
            ResetarCheque();

            ContArestas.Text = arestas.ToString();
            ContArestas.HorizontalAlignment = HorizontalAlignment.Center;
            ContVertices.Text = vertices.ToString();
            ContVertices.HorizontalAlignment = HorizontalAlignment.Center;
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        //Conteudo para o canvas de exbicao de caminho minimo

        private void Btn_EncontrarCaminho(object sender, RoutedEventArgs e) 
        {
            //Cria a matriz de distancias entre os vertices
            grafoCriado.CreateDM(vertices);

            //Checa se o campo esta vazio
            if (VerticeOrigem.Text != "" && VerticeSaida.Text != "") 
            {
                int numDefinido1 = int.Parse(VerticeOrigem.Text);
                int numDefinido2 = int.Parse(VerticeSaida.Text);

                //Checa se são valores validos
                if (numDefinido1 > 0 && numDefinido2 > 0 && numDefinido1 - 1 < vertices && numDefinido2 - 1 < vertices) 
                {
                    //Joga o numero de arestas que são passadas no processo do caminho
                    //TODO exibir o caminho em si
                    JanelaCaminho.Text = grafoCriado.distanceMatrix[numDefinido1 - 1][numDefinido2 - 1].ToString();
                    
                }
                else Mensagem("Preencha um vertice valido para realizar a busca.", "ERRO: Vertice invalido");
            }
            else Mensagem("Preencha ambos os campos para realizar a busca.", "ERRO: Campo(s) vazio(s)");

        }

        //////////////////////////////////////////////////////////////////////////////////////////

        private void TextBox_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }
        private async void Mensagem(string Mensagem, string Titulo)
        {
            var dialog = new MessageDialog(Mensagem, Titulo);
            var result = await dialog.ShowAsync();
        }
        private bool ChequeLigacoes(Vertice v, int saida) 
        {
            foreach(Aresta a in v.Adjacencia) 
            {
                if(a.Saida.Nametag == saida) 
                {
                    return false;
                }
            }
            return true;
        }
        private void EsconderMenus() 
        {
            BlocoCriarGrafo.IsHitTestVisible = false;
            BlocoCriarGrafo.Opacity = 0;

            BlocoInformacoesGrafo.IsHitTestVisible = false;
            BlocoInformacoesGrafo.Opacity = 0;

            BlocoCaminhoMinimo.IsHitTestVisible = false;
            BlocoCaminhoMinimo.Opacity = 0;
        }
        private void ContadorComponentes() 
        {
            foreach(Vertice v in grafoCriado.Conteudo) 
            {
                if (!v.IsChecked) 
                {
                    ChequeNoGrafo(v);
                    contadorComponentes += 1;
                }
            }
        }
        private void ChequeNoGrafo(Vertice v) 
        {
            v.IsChecked = true;
            foreach(Aresta a in v.Adjacencia) 
            {
                if (!a.Saida.IsChecked) 
                {
                    ChequeNoGrafo(a.Saida);
                }
            }
        }
        private void ResetarCheque() 
        {
            foreach(Vertice v in grafoCriado.Conteudo) 
            {
                v.IsChecked = false;
            }
        }
    }
}
