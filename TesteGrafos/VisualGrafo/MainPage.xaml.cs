using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Security.Permissions;
using TesteGrafos;
using Windows.Devices.Radios;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace VisualGrafo
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        private Grafo grafoCriado;
        private int vertices = 0;
        private int arestas = 0;
        private bool IsDirigido = false;
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
            GravarGrafo.Click += Btn_GravarGrafo;
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        
        //Conteudo do canvas do menu de seleção

        /// <summary>
        /// Botão para ir para a pagina escolhida no combobox
        /// </summary>
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
            else if (Selecao.SelectionBoxItem.ToString() == "Distribuição Circular")
            {
                EsconderMenus();

                BlocoDistribuicaoCircular.IsHitTestVisible = true;
                BlocoDistribuicaoCircular.Opacity = 100;

                Titulo.Text = "GRAFOS: DISTRIBUIÇÃO CIRCULAR";

                DistribuirCircular();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        //Conteudo para o canvas de construir grafo

        /// <summary>
        /// Botão para criar um vertice
        /// </summary>
        private void Btn_DefinicaoVertices(object sender, RoutedEventArgs e)
        {
            //Primeiro checa se o campo ficou vazio
            if(NumVertices.Text.Trim().Length > 0 )
            {
                //Define o numero de vertices com base no campo
                vertices = int.Parse(NumVertices.Text);
                //Desbloqueia a interação com a insercao de arestas
                MenuArestas.IsHitTestVisible = true;

                //Instancia o grafo
                grafoCriado = new Grafo(vertices);
                //Define ele como dirigido ou não
                grafoCriado.IsDirigido = IsDirigido;

                //Bloqueia a interação com o botão e os radio buttons
                Mensagem("O grafo foi criado com " + vertices +" vertices.", "GRAFO CRIADO");
                DefinicaoVertices.IsHitTestVisible = false;
                RadioDirigido.IsHitTestVisible = false;
                RadioNaoDirigido.IsHitTestVisible = false;
            }
        }

        /// <summary>
        /// Botão para realizar a criação de uma aresta
        /// </summary>
        private void Btn_CriarAresta(object sender, RoutedEventArgs e) 
        {
            int numDefinido1, numDefinido2;
            //Primeiro checa se os campos estão vazios
            if(BoxVertice1.Text != "" && BoxVertice2.Text != "") 
            {
                numDefinido1 = int.Parse(BoxVertice1.Text) - 1;
                numDefinido2 = int.Parse(BoxVertice2.Text) - 1;
                int peso = int.Parse(Peso.Text);

                //Depois checa se eles são validos (Valor menor que numero de vertices e maior que 0)
                if (numDefinido1 < vertices && numDefinido2 < vertices) 
                {
                    if(numDefinido1 >= 0 && numDefinido2 >= 0 && peso >= 0) 
                    {
                        //Depois checa se a ligação ja exite
                        if (grafoCriado.ChequeLigacoes(grafoCriado.Conteudo.ElementAt(numDefinido1) ,numDefinido2 + 1)) 
                        {
                            grafoCriado.CriarLigacao(numDefinido1, numDefinido2, peso);

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

        /// <summary>
        /// Tratamento para quando transitar de um radio button para outro
        /// </summary>
        private void Radio_ChangeValue(object sender, RoutedEventArgs e) 
        {
            if (RadioDirigido.IsChecked.Equals(true)) 
            {
                IsDirigido = true;
            }
            else 
            {
                IsDirigido = false;
            }
        }

        /// <summary>
        /// Botâo para apagar o grafo
        /// </summary>
        private void Btn_ClearGrafo(object sender, RoutedEventArgs e) 
        {
            grafoCriado.Conteudo.Clear();
            ArestasAdicionadas.Children.Clear();
            MenuArestas.IsHitTestVisible = false;
            DefinicaoVertices.IsHitTestVisible = true;
            RadioDirigido.IsHitTestVisible = true;
            RadioNaoDirigido.IsHitTestVisible = true;
            vertices = 0;
            arestas = 0;
            contadorComponentes = 0;
        }

        /// <summary>
        /// Botâo para exibir a janela de tutorial
        /// </summary>
        private void Btn_ExibirTutorialCriar(object sender, RoutedEventArgs e) 
        {
            TutorialCriarGrafo.IsOpen = true;
        }

        /// <summary>
        /// Botâo para salvar o grafo em um txt no padrão pajek
        /// </summary>
        private void Btn_GravarGrafo(object sender, RoutedEventArgs e) 
        {
            grafoCriado.GravarTxt(IsDirigido);
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        //Conteudo para o canvas das informações do grafo

        /// <summary>
        /// Botão para exibir o numero de componentes, vertices e arestas
        /// </summary>
        private void Btn_ExibirInformacoes(object sender, RoutedEventArgs e) 
        {
            contadorComponentes = grafoCriado.ContadorComponentes();
            ContComp.Text = contadorComponentes.ToString();
            ContComp.HorizontalAlignment = HorizontalAlignment.Center;

            contadorComponentes = 0;
            grafoCriado.ResetarCheque();

            ContArestas.Text = arestas.ToString();
            ContArestas.HorizontalAlignment = HorizontalAlignment.Center;
            ContVertices.Text = vertices.ToString();
            ContVertices.HorizontalAlignment = HorizontalAlignment.Center;
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        //Conteudo para o canvas de exbicao de caminho minimo

        /// <summary>
        /// Botão para exibir o caminho minimo entre dois vertices
        /// </summary>
        private void Btn_EncontrarCaminho(object sender, RoutedEventArgs e) 
        {
            //Cria a matriz de distancias entre os vertices
            grafoCriado.CreateDM(vertices);

            //Checa se o campo esta vazio  
            if (VerticeOrigem.Text != "" && VerticeDestino.Text != "") 
            {
                int numDefinido1 = int.Parse(VerticeOrigem.Text);
                int numDefinido2 = int.Parse(VerticeDestino.Text);

                //Checa se são valores validos
                if (numDefinido1 > 0 && numDefinido2 > 0 && numDefinido1 - 1 < vertices && numDefinido2 - 1 < vertices) 
                {
                    //Joga o numero de arestas que são passadas no processo do caminho

                    List<int> vetorCaminho = new List<int>();
                    vetorCaminho.Add(numDefinido2);
                    string caminho = " " + numDefinido1.ToString() + " ";
                    int aux = numDefinido2 - 1;
                    
                    //Calculo do caminho minimo armazenando valores em VetorCaminho
                    while(!vetorCaminho.Contains(numDefinido1))
                    {
                        vetorCaminho.Add(grafoCriado.distanceMatrix[numDefinido1 - 1][aux] + 1);
                        aux = grafoCriado.distanceMatrix[numDefinido1 - 1][aux];
                        if (aux == -1) break;
                    }

                    //se não existir caminho, caminho é infinito
                    if (aux == -1) JanelaCaminho.Text = "INFINITO";
                    //inverte o vetor e cria string
                    else
                    {
                        vetorCaminho.Reverse();
                        foreach (var i in vetorCaminho)
                        {
                            if(!i.Equals(numDefinido1))
                                caminho += "- " + i.ToString() + " ";
                        }
                        JanelaCaminho.Text = caminho;
                    }
                }
                else Mensagem("Preencha um vertice valido para realizar a busca.", "ERRO: Vertice invalido");
            }
            else Mensagem("Preencha ambos os campos para realizar a busca.", "ERRO: Campo(s) vazio(s)");
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        private void DistribuirCircular() 
        {
            double x1, y1, x2, y2;
            int ordem = grafoCriado.Conteudo.Count();
            
            int i = 1;
            foreach(var v in grafoCriado.Conteudo) 
            {
                v.Ellipse = new Ellipse();
                v.Ellipse.Width = 200/ordem + 10;
                v.Ellipse.Height = 200/ordem + 10;
                v.DefinirPosicao(ordem, i);

                v.Ellipse.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                BlocoDistribuicaoCircular.Children.Add(v.Ellipse);

                TextBlock text = new TextBlock();
                text.Text = v.Nametag.ToString();
                text.Foreground = new SolidColorBrush(Windows.UI.Colors.Yellow);
                BlocoDistribuicaoCircular.Children.Add(text);

                Canvas.SetLeft(v.Ellipse, v.X*250 + 500);
                Canvas.SetTop(v.Ellipse, v.Y*250 + 280);

                x1 = Canvas.GetLeft(v.Ellipse);
                y1 = Canvas.GetTop(v.Ellipse);

                Canvas.SetLeft(text, x1 + (200/ordem + 10)/2);
                Canvas.SetTop(text, y1 + (200 / ordem + 10) / 2);

                i++;
            }



            foreach(var v in grafoCriado.Conteudo) 
            {
                foreach(var a in v.Adjacencia) 
                {
                    x1 = Canvas.GetLeft(a.Entrada.Ellipse);
                    y1 = Canvas.GetTop(a.Entrada.Ellipse);
                    x2 = Canvas.GetLeft(a.Saida.Ellipse);
                    y2 = Canvas.GetTop(a.Saida.Ellipse);
    
                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(Windows.UI.Colors.Red);

                    if (grafoCriado.IsDirigido && a.Saida.Adjacencia.Where(m => m.Saida == a.Entrada).FirstOrDefault() != null) 
                    {
                        Line linha = new Line();
                        linha.Stroke = new SolidColorBrush(Windows.UI.Colors.Red);

                        line.X1 = x1 + (100 / ordem)/2 + 10;
                        line.X2 = x2 + (100 / ordem) / 2 + 10;
                        line.Y1 = y1 + (100 / ordem) / 2 + 10;
                        line.Y2 = y2 + (100 / ordem) / 2 + 10;

                        linha.X1 = x1 + 100 / ordem + 10;
                        linha.X2 = x2 + 100 / ordem + 10;
                        linha.Y1 = y1 + 100 / ordem + 10;
                        linha.Y2 = y2 + 100 / ordem + 10;

                        BlocoDistribuicaoCircular.Children.Add(linha);
                        Canvas.SetZIndex(linha, -99);
                    }
                    else if(a.Entrada == a.Saida) 
                    {
                        //laço
                    }
                    else 
                    {
                        line.X1 = x1 + ((200 / ordem) + 10) / 2;
                        line.X2 = x2 + ((200 / ordem) + 10) / 2;
                        line.Y1 = y1 + ((200 / ordem) + 10) / 2;
                        line.Y2 = y2 + ((200 / ordem) + 10) / 2;
                    }

                    //line.StrokeThickness = 4;

                    BlocoDistribuicaoCircular.Children.Add(line);
                    Canvas.SetZIndex(line, -99);

                    //Canvas.SetLeft(line, Canvas.GetLeft(a.Entrada.Ellipse));
                    //Canvas.SetTop(line, Canvas.GetTop(a.Entrada.Ellipse));
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Metodo para impedir a digitação de letras e simbolos nos TextBoxs
        /// </summary>
        private void TextBox_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }
        
        /// <summary>
        /// Metodo para exibir mensagens na tela
        /// </summary>
        private async void Mensagem(string Mensagem, string Titulo)
        {
            var dialog = new MessageDialog(Mensagem, Titulo);
            var result = await dialog.ShowAsync();
        }
        
        /// <summary>
        /// Metodo para ocultar os canvas dos menus
        /// </summary>
        private void EsconderMenus() 
        {
            BlocoCriarGrafo.IsHitTestVisible = false;
            BlocoCriarGrafo.Opacity = 0;

            BlocoInformacoesGrafo.IsHitTestVisible = false;
            BlocoInformacoesGrafo.Opacity = 0;

            BlocoCaminhoMinimo.IsHitTestVisible = false;
            BlocoCaminhoMinimo.Opacity = 0;

            BlocoDistribuicaoCircular.IsHitTestVisible = false;
            BlocoDistribuicaoCircular.Opacity = 0;
            BlocoDistribuicaoCircular.Children.Clear();
        }

    }
}
