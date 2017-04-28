using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoGenetico
{
    /// <summary>
	/// Compares genomes by fitness
	/// </summary>
	public sealed class ComparadorCromosoma : IComparer
    {
        public ComparadorCromosoma()
        {
        }
        public int Compare(object x, object y)
        {
            if (!(x is Cromosoma) || !(y is Cromosoma))
                throw new ArgumentException("No es de la clase Cromosoma");

            if (((Cromosoma)x).FitnessActual > ((Cromosoma)y).FitnessActual)
                return 1;
            else if (((Cromosoma)x).FitnessActual == ((Cromosoma)y).FitnessActual)
                return 0;
            else
                return -1;

        }
    }
}
