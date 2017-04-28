using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoGenetico
{
    class Cromosoma
    {
        public long Length;
        public int MutationIndex;
        public float FitnessActual = 0.0f;

        public ArrayList TheArray = new ArrayList();
        public static Random TheSeed = new Random((int)DateTime.Now.Ticks);
        int TheMin = 0;
        int TheMax = 2;

        public Cromosoma()
        {

        }

        public Cromosoma(long length, object min, object max)
        {
            Length = length;
            TheMin = (int)min;
            TheMax = (int)max;

            for (int i = 0; i < Poblacion.numTrabajadores; i++)
            {
                int ind = TheSeed.Next(0, Poblacion.numPuestosDeTrabajo);
                for (int j = 0; j < Poblacion.numPuestosDeTrabajo; j++)
                {
                    if (j == ind)
                    {
                        TheArray.Add(1);

                    }
                    else
                    {
                        TheArray.Add(0);
                    }
                }
            }
        }

        public bool esValido()
        {
            int cont = 0;
            for (int i = 0; i < Poblacion.numTrabajadores; i++)
            {
                int cantidadAsignaciones = 0;  // Cantidad de puestos de trabajo en el que esta cada trabajador.
                for (int j = 0; j < Poblacion.numPuestosDeTrabajo; j++)
                {
                    cantidadAsignaciones += (int)TheArray[cont];
                    if (cantidadAsignaciones > 1)   //Restricción: Un trabajador solo puede estar en un puesto de trabajo.
                        return false;
                    cont++;
                }

                if (cantidadAsignaciones != 1) return false;

            }
            return true;
        }

        public object GenerateGeneValue(object min, object max)
        {
            return TheSeed.Next((int)min, (int)max);
        }

        public void Mutar()
        {
            int indiceTrabajador = TheSeed.Next(Poblacion.numTrabajadores);
            int puestosDeTrabajo = Poblacion.numPuestosDeTrabajo;
            int indice = 0;

            int indiceMutacion = puestosDeTrabajo * indiceTrabajador + TheSeed.Next(Poblacion.numPuestosDeTrabajo);

            for (int i = 0; i < puestosDeTrabajo; i++)
            {
                indice = puestosDeTrabajo * indiceTrabajador + i;
                if ((int)TheArray[indice] == 1)
                {
                    TheArray[indice] = 0;
                    break;
                }
            }

            TheArray[indiceMutacion] = 1;

        }

        // This fitness function calculates the production from the current genome
        private float CalcularProduccion()
        {
            int Xij, ind;
            double Rij, Tij;
            FitnessActual = 0;
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                for (int j = 0; j < Poblacion.numTrabajadores; j++)
                {
                    ind = j * Poblacion.numPuestosDeTrabajo + i;
                    Xij = (int)TheArray[ind];
                    Rij = (double)Poblacion.indiceError[ind];
                    Tij = (double)Poblacion.indiceTiempo[ind];

                    int duracion = Poblacion.duracionTurno;
                    if (Xij != 0)
                        FitnessActual += (float)Math.Truncate((double)(duracion / ((1 + Rij) * Tij * Xij)));
                }
            }

            return FitnessActual;
        }

        public float CalcularFitness()
        {
            FitnessActual = CalcularProduccion();
            return FitnessActual;
        }

        override public string ToString()
        {
            string strResult = "";
            for (int i = 0; i < Length; i++)
            {
                strResult = strResult + ((int)TheArray[i]).ToString() + " ";
            }

            strResult += "-->" + FitnessActual.ToString();

            return strResult;
        }

        public void CopiarInformacionCromosoma(Cromosoma dest)
        {
            Cromosoma cromosoma = dest;
            cromosoma.Length = Length;
            cromosoma.TheMin = TheMin;
            cromosoma.TheMax = TheMax;
        }

        public void CopiarCromosoma(out Cromosoma dest)
        {
            dest = new Cromosoma();
            dest.Length = Length;
            dest.TheMin = TheMin;
            dest.TheMax = TheMax;
            dest.FitnessActual = FitnessActual;
            for (int i = 0; i < Length; i++)
            {
                dest.TheArray.Add(TheArray[i]);
            }
        }

        public void Cruzar(ref Cromosoma cromosoma2, out Cromosoma hijo1, out Cromosoma hijo2)
        {
            int pos = (int)(TheSeed.NextDouble() * (double)Length);
            hijo1 = new Cromosoma();
            hijo2 = new Cromosoma();

            cromosoma2.CopiarInformacionCromosoma(hijo1);
            cromosoma2.CopiarInformacionCromosoma(hijo2);

            for (int i = 0; i < Length; i++)
            {
                if (i < pos)
                { 
                    hijo1.TheArray.Add(TheArray[i]);
                    hijo2.TheArray.Add(cromosoma2.TheArray[i]);
                }
                else
                {
                    hijo1.TheArray.Add(cromosoma2.TheArray[i]);
                    hijo2.TheArray.Add(TheArray[i]);
                }
            }
        }
    }
}
