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
            ArrayList asignaciones = new ArrayList();
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                asignaciones.Add(0);
            }
            int cont = 0;
            int puesto = 0;
            for (int j = 0; j < Poblacion.numTrabajadores; j++)
            {
                int cantidadAsignaciones = 0;
                for (int k = 0; k < Poblacion.numPuestosDeTrabajo; k++)
                {
                    cantidadAsignaciones += (int)this.TheArray[cont];
                    if (((int)this.TheArray[cont]) == 1)
                    {
                        puesto = cont % Poblacion.numPuestosDeTrabajo;
                        asignaciones[puesto] = ((int)asignaciones[puesto]) + 1;
                    }
                    if (((int)asignaciones[puesto]) > ((int)Poblacion.vacantes[puesto]))
                    {
                        return false;
                    }
                    if (cantidadAsignaciones > 1)
                    {
                        return false;
                    }
                    cont++;
                }
                if (cantidadAsignaciones != 1)
                {
                    return false;
                }
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
                    Tij = (int)Poblacion.indiceTiempo[ind];

                    int duracionTurno = Poblacion.duracionTurno;
                    if (Xij != 0)
                        FitnessActual += (float)Math.Truncate((double)(((double)duracionTurno) / ((1 + Rij) * Tij * Xij)));
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

        public void Cruzar_uniforme(ref Cromosoma cromosoma2, out Cromosoma hijo1, out Cromosoma hijo2)
        {
            hijo1 = new Cromosoma();
            hijo2 = new Cromosoma();
            cromosoma2.CopiarInformacionCromosoma(hijo1);
            cromosoma2.CopiarInformacionCromosoma(hijo2);
            int cont = 0;
            for (int i = 0; i < Poblacion.numTrabajadores; i++)
            {
                for (int j = 0; j < Poblacion.numPuestosDeTrabajo; j++)
                {
                    if ((i % 2) == 0)
                    {
                        hijo1.TheArray.Add(this.TheArray[cont]);
                        hijo2.TheArray.Add(cromosoma2.TheArray[cont]);
                    }
                    else
                    {
                        hijo2.TheArray.Add(this.TheArray[cont]);
                        hijo1.TheArray.Add(cromosoma2.TheArray[cont]);
                    }
                    cont++;
                }
            }
        }

        public void Cruzar_unpunto(ref Cromosoma cromosoma2, out Cromosoma hijo1, out Cromosoma hijo2)
        {
            int trabajador1 = TheSeed.Next(0, Poblacion.numTrabajadores);
            int trabajador2 = TheSeed.Next(0, Poblacion.numTrabajadores);
            hijo1 = new Cromosoma();
            hijo2 = new Cromosoma();
            this.CopiarCromosoma(out hijo1);
            cromosoma2.CopiarCromosoma(out hijo2);
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                int ind1 = (trabajador1 * Poblacion.numPuestosDeTrabajo) + i;
                int ind2 = (trabajador2 * Poblacion.numPuestosDeTrabajo) + i;
                int aux = (int)hijo1.TheArray[ind1];
                hijo1.TheArray[ind1] = hijo2.TheArray[ind2];
                hijo2.TheArray[ind2] = aux;
            }
        }

        public void mostrarAsignaciones()
        {
            Console.WriteLine("N\x00famero de trabajadores: " + Poblacion.numTrabajadores);
            Console.WriteLine("Cantidad de vacantes por puesto: ");
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                Console.Write("Puesto " + (i + 1) + ": ");
                Console.WriteLine(Poblacion.vacantes[i]);
            }
            for (int j = 0; j < Poblacion.numTrabajadores; j++)
            {
                Console.Write("Trabajador " + (j + 1) + ": ");
                int suma = 0;
                for (int k = 0; k < Poblacion.numPuestosDeTrabajo; k++)
                {
                    int indice = (j * Poblacion.numPuestosDeTrabajo) + k;
                    suma += (int)this.TheArray[indice];
                    if (((int)this.TheArray[indice]) == 1)
                    {
                        Console.WriteLine("Asignado a Puesto " + (k + 1));
                    }
                }
                if (suma == 0)
                {
                    Console.WriteLine("No asignado a ning\x00fan puesto de trabajo");
                }
            }
        }

        public void mostrarCromosoma()
        {
            Console.WriteLine(this.ToString());
        }

    }
}