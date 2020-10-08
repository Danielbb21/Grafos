using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteGrafos
{
    public class Vertice
    {
        public List<Aresta> Adjacencia { get; set; }
        public int Nametag { get; set; }
        public bool IsChecked { get; set; }

        public Vertice() 
        {
            this.IsChecked = false;
        }

		// Pode colocar mais informações caso a gente queria exibir no uwp
	}
}
