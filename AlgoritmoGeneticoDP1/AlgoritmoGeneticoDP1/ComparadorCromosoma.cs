using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace AlgoritmoGeneticoDP1
{
    class ComparadorCromosoma : IComparer 
    {
        public ComparadorCromosoma()
        {
        }
        // Methods
        public int Compare(object x, object y)
        {
            if (!(x is Cromosoma) || !(y is Cromosoma))
            {
                throw new ArgumentException("No es de la clase Cromosoma");
            }
            if (((Cromosoma)x).FitnessActual > ((Cromosoma)y).FitnessActual)
            {
                return 1;
            }
            if (((Cromosoma)x).FitnessActual == ((Cromosoma)y).FitnessActual)
            {
                return 0;
            }
            return -1;
        }

    }
}
