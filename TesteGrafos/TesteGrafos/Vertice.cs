using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

namespace TesteGrafos
{
    public class Vertice
    {
        public List<Aresta> Adjacencia { get; set; }
        public int Nametag { get; set; }
        public bool IsChecked { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double angulo { get; set; }
        public Ellipse Ellipse { get; set; }

        public Vertice() 
        {
            this.IsChecked = false;
        }

        public void DefinirPosicao(int ordem, int posicao)
        {
            this.angulo = (2 * Math.PI / ordem * posicao);
            this.X = Math.Cos(angulo);
            this.Y = Math.Sin(angulo);
        }

        // Pode colocar mais informações caso a gente queria exibir no uwp
    }
}
