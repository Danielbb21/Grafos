using System;
using System.Collections.Generic;
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
using Windows.UI;
using Windows.UI.Input.Inking;
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

        /// <summary>
        /// Definição das ações dos botões
        /// </summary>
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
            else if (Selecao.SelectionBoxItem.ToString() == "Distribuição Circular" && grafoCriado != null)
            {
                EsconderMenus();

                BlocoDistribuicaoCircular.IsHitTestVisible = true;
                BlocoDistribuicaoCircular.Opacity = 100;

                Titulo.Text = "GRAFOS: DISTRIBUIÇÃO CIRCULAR";

                DistribuirCircular();
            }
            else if (Selecao.SelectionBoxItem.ToString() == "Distribuição Aleatória" && grafoCriado != null)
            {
                EsconderMenus();

                BlocoDistribuicaoAleatoria.IsHitTestVisible = true;
                BlocoDistribuicaoAleatoria.Opacity = 100;

                Titulo.Text = "GRAFOS: DISTRIBUIÇÃO ALEATÓRIA";

                DistribuirAleatorio();
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
                CriarAresta.IsHitTestVisible = true;

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

                            //Cria um textblock pra ser exibido a ligacao no grid
                            Border b1 = new Border();
                            b1.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                            b1.BorderThickness = new Thickness(1,0,1,1);

                            Border b2 = new Border();
                            b2.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                            b2.BorderThickness = new Thickness(0, 0, 1,1);

                            Border b3 = new Border();
                            b3.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                            b3.BorderThickness = new Thickness(0,0,1,1);

                            TextBlock VerticeEntrada = new TextBlock();
                            VerticeEntrada.HorizontalAlignment = HorizontalAlignment.Center;
                            VerticeEntrada.FontSize = 20;
                            VerticeEntrada.Text = (numDefinido1 + 1).ToString();
                            b1.Child = VerticeEntrada;

                            TextBlock VerticeSaida = new TextBlock();
                            VerticeSaida.HorizontalAlignment = HorizontalAlignment.Center;
                            VerticeSaida.FontSize = 20;
                            VerticeSaida.Text = (numDefinido2 + 1).ToString();
                            b2.Child = VerticeSaida;

                            TextBlock PesoBlock = new TextBlock();
                            PesoBlock.HorizontalAlignment = HorizontalAlignment.Center;
                            PesoBlock.FontSize = 20;
                            PesoBlock.Text = Peso.Text;
                            b3.Child = PesoBlock;

                            arestas += 1;

                            MenuArestas.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                            MenuArestas.Children.Add(b1);
                            MenuArestas.Children.Add(b2);
                            MenuArestas.Children.Add(b3);

                            int row = MenuArestas.RowDefinitions.Count() - 1;

                            Grid.SetColumn(b1, 0);
                            Grid.SetColumn(b2, 1);
                            Grid.SetColumn(b3, 2);
                            Grid.SetRow(b1, row);
                            Grid.SetRow(b2, row);
                            Grid.SetRow(b3, row);

                            MenuArestas.Height += 20;
                            b1.Tapped += VerticeEntrada_Tapped;
                            b2.Tapped += VerticeEntrada_Tapped;
                            b3.Tapped += VerticeEntrada_Tapped;
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
        /// Evento para excluir a aresta clicada no grid
        /// </summary>
        private void VerticeEntrada_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Define o linha o qual o elemento está
            int row = Grid.GetRow((FrameworkElement)sender);
            
            //Define a posicao dos 3 elementos a serem excluidos com base na linha
            int entrada = 3 * row;
            int saida = entrada + 1;
            int peso = saida + 1;

            //Retira o elemento border que contem os textblocks
            Border enBorder = (Border)MenuArestas.Children.ElementAt(entrada);
            Border saBorder = (Border)MenuArestas.Children.ElementAt(saida);

            //Retira os textblocks em si para retirar o nome do vertice de entrada e saida
            TextBlock en = (TextBlock)enBorder.Child;
            TextBlock sa = (TextBlock)saBorder.Child;

            //Remove a ligacao entre o vertice entrada e o vertice saida
            grafoCriado.Conteudo .Where(v => v.Nametag == int.Parse(en.Text)).FirstOrDefault()
                                 .Adjacencia.RemoveAll(m => m.Saida.Nametag == int.Parse(sa.Text));

            //Caso o grafo não for dirigido, é retirado a ligação de sua saida com ele
            if (!grafoCriado.IsDirigido) 
            {
                grafoCriado.Conteudo.Where(v => v.Nametag == int.Parse(sa.Text)).FirstOrDefault()
                                 .Adjacencia.RemoveAll(m => m.Saida.Nametag == int.Parse(en.Text));
            }
            
            //Remove os textblocks da grid
            MenuArestas.Children.RemoveAt(peso);
            MenuArestas.Children.RemoveAt(saida);
            MenuArestas.Children.RemoveAt(entrada);

            //Diminui o numero de arestaas
            arestas--;

            //Diminui por um todos os elementos com as linhas maiores com a que foi excluido
            foreach(FrameworkElement child in MenuArestas.Children) 
            {
                if (Grid.GetRow(child) > row) 
                {
                    Grid.SetRow(child, Grid.GetRow(child) - 1);
                }
            }

            //Exlui a linha em si
            MenuArestas.RowDefinitions.RemoveAt(row);
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
            if(grafoCriado == null || grafoCriado.Conteudo.Count == 0) 
            {
                Mensagem("Grafo já foi apagado. Nada aconteceu.", "AVISO");
            }
            else 
            {
                grafoCriado.Conteudo.Clear();
                MenuArestas.IsHitTestVisible = false;
                CriarAresta.IsHitTestVisible = false;
                DefinicaoVertices.IsHitTestVisible = true;
                RadioDirigido.IsHitTestVisible = true;
                RadioNaoDirigido.IsHitTestVisible = true;
                vertices = 0;
                arestas = 0;
                contadorComponentes = 0;

                for(int i = MenuArestas.Children.Count() - 1; i >= 6; i--) 
                {
                    MenuArestas.Children.RemoveAt(i);
                }
            }
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
                    if (aux == -1) JanelaCaminho.Text = "NÃO HÁ CAMINHO";
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

        // Distribuição aleatoria e circular

        /// <summary>
        /// Metodo para exibir a rede de forma circular
        /// </summary>
        private void DistribuirCircular() 
        {
            //Armazenam os valores do dimensões do canvas 
            double x1, y1;

            //Quantidade total de vertices
            int ordem = grafoCriado.Conteudo.Count();
            
            //Contador
            int i = 1;

            //Pra cada vertice é criado uma ellipse pra ser jogada no canvas
            foreach(var v in grafoCriado.Conteudo) 
            {
                //Pra manter visivel as elipses a medida que for adicionando vertices
                //é feito um calculo para definir o tamanho dependendo da ordem
                v.Ellipse = new Ellipse();
                v.Ellipse.Width = 200/ordem + 10;
                v.Ellipse.Height = 200/ordem + 10;

                //Calculo para definir sua posição x e y
                v.DefinirPosicao(ordem, i);

                //Estetica da ellipse
                v.Ellipse.Fill = new SolidColorBrush(Windows.UI.Colors.Black);

                //Adicionando a ellipse no canvas
                BlocoDistribuicaoCircular.Children.Add(v.Ellipse);

                //Textblock pra armazenar o valor do vertice
                TextBlock text = new TextBlock();
                text.Text = v.Nametag.ToString();
                text.Foreground = new SolidColorBrush(Windows.UI.Colors.Yellow);
                BlocoDistribuicaoCircular.Children.Add(text);

                //Calculo para distanciar os vertices dentro da circunferencia criada a partir do x e y
                Canvas.SetLeft(v.Ellipse, v.X*250 + 500);
                Canvas.SetTop(v.Ellipse, v.Y*250 + 280);

                //Posicionando o textblock em cima da ellipse
                x1 = Canvas.GetLeft(v.Ellipse);
                y1 = Canvas.GetTop(v.Ellipse);
                Canvas.SetLeft(text, x1 + (200/ordem + 10)/2);
                Canvas.SetTop(text, y1 + (200 / ordem + 10) / 2);

                i++;
            }

            //Definição de arestas
            PegaArestas(ordem, BlocoDistribuicaoCircular);
        }

        /// <summary>
        /// Realiza a distribuição aleatoria para exibir a rede
        /// </summary>
        private void DistribuirAleatorio() 
        {
            //Gera um elemento random para gerar os numeros aleatorios
            Random random = new Random();

            //Numero de elementos dentro da string
            int ordem = grafoCriado.Conteudo.Count();
            
            //Pra cada vertice no grafo é criado uma ellipse
            foreach(var v in grafoCriado.Conteudo) 
            {
                //Pra manter visivel as elipses a medida que for adicionando vertices
                //é feito um calculo para definir o tamanho dependendo da ordem
                v.Ellipse = new Ellipse();
                v.Ellipse.Width = 200 / ordem + 10;
                v.Ellipse.Height = 200 / ordem + 10;
                v.Ellipse.Fill = new SolidColorBrush(Windows.UI.Colors.Black);

                //Definição dos numeros aleatorios dentro do escopo do canvas
                v.X = random.Next(0, 1021);
                v.Y = random.Next(0, 607);

                //Adição da ellipse no canvas
                BlocoDistribuicaoAleatoria.Children.Add(v.Ellipse);

                //Denições de suas posições no canvas
                Canvas.SetLeft(v.Ellipse, v.X);
                Canvas.SetTop(v.Ellipse, v.Y);
            }

            PegaArestas(ordem, BlocoDistribuicaoAleatoria);
        }

        /// <summary>
        /// Metodo para exbir as arestas no canvas
        /// </summary>
        public void PegaArestas(int ordem, Canvas bloco)
        {
            //Valores que armazenam a posicao do vertice dentro do canvas
            double x1, y1, x2, y2;

            //Para cada vertice é feito um loop para exibir todas suas ligações no canvas
            foreach (var v in grafoCriado.Conteudo)
            {
                foreach (var a in v.Adjacencia)
                {
                    x1 = Canvas.GetLeft(a.Entrada.Ellipse);
                    y1 = Canvas.GetTop(a.Entrada.Ellipse);
                    x2 = Canvas.GetLeft(a.Saida.Ellipse);
                    y2 = Canvas.GetTop(a.Saida.Ellipse);

                    //Criação da linha que representa a ligação
                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(Windows.UI.Colors.Red);

                    //Mudança de comportamento caso ele for dirido, não dirigido ou um laço

                    //Caso ele for dirigido e possuir ligação paralela
                    if (grafoCriado.IsDirigido && a.Saida.Adjacencia.Where(m => m.Saida == a.Entrada).FirstOrDefault() != null)
                    {
                        Line linha = new Line();
                        linha.Stroke = new SolidColorBrush(Windows.UI.Colors.Red);

                        line.X1 = x1 + (100 / ordem) / 2 + 10;
                        line.X2 = x2 + (100 / ordem) / 2 + 10;
                        line.Y1 = y1 + (100 / ordem) / 2 + 10;
                        line.Y2 = y2 + (100 / ordem) / 2 + 10;

                        linha.X1 = x1 + 100 / ordem + 10;
                        linha.X2 = x2 + 100 / ordem + 10;
                        linha.Y1 = y1 + 100 / ordem + 10;
                        linha.Y2 = y2 + 100 / ordem + 10;

                        bloco.Children.Add(linha);
                        Canvas.SetZIndex(linha, -99);
                    }
                    //Caso ele for um laço
                    if (a.Entrada == a.Saida)
                    {
                        Ellipse laco = new Ellipse();
                        laco.Stroke = new SolidColorBrush(Windows.UI.Colors.Red);
                        laco.Width = 100 / ordem + 10;
                        laco.Height = 100 / ordem + 10;

                        bloco.Children.Add(laco);
                        Canvas.SetLeft(laco, x1 + ((200 / ordem) + 10) / 2);
                        Canvas.SetTop(laco, y1 + ((200 / ordem) + 10) / 2);
                        Canvas.SetZIndex(laco, -99);
                    }
                    //Caso ele for não dirigido ou for dirigido e não apresentar ligação dupla
                    else
                    {
                        line.X1 = x1 + ((200 / ordem) + 10) / 2;
                        line.X2 = x2 + ((200 / ordem) + 10) / 2;
                        line.Y1 = y1 + ((200 / ordem) + 10) / 2;
                        line.Y2 = y2 + ((200 / ordem) + 10) / 2;
                    }

                    //Adiciona a aresta no canvas
                    bloco.Children.Add(line);
                    Canvas.SetZIndex(line, -99);
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

            BlocoDistribuicaoAleatoria.IsHitTestVisible = false;
            BlocoDistribuicaoAleatoria.Opacity = 0;
            BlocoDistribuicaoAleatoria.Children.Clear();
        }

    }
}
