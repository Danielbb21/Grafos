using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteGrafos
{
    public class Aresta
    {
        public Vertice Entrada { get; set; }
        public Vertice Saida { get; set; }
        public int Peso { get; set; }
        
        //inverte arestas 
        public Aresta ArestaAoContrario()
        {
            Aresta a = new Aresta();
            a.Entrada = this.Saida;
            a.Saida = this.Entrada;
            a.Peso = this.Peso;
            return a;
        }
    }
}
