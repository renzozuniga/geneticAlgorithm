using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace AlgoritmoGeneticoDP1
{
    class Cromosoma
    {
        // Fields
        public float FitnessActual;
        public long Length;
        public int MutationIndex;
        public ArrayList TheArray;
        private int TheMax;
        private int TheMin;
        public static Random TheSeed = new Random((int)DateTime.Now.Ticks);

        // Methods
        public Cromosoma()
        {
            this.FitnessActual = 0f;
            this.TheArray = new ArrayList();
            this.TheMin = 0;
            this.TheMax = 2;
        }

        public Cromosoma(long length, object min, object max)
        {
            this.FitnessActual = 0f;
            this.TheArray = new ArrayList();
            this.TheMin = 0;
            this.TheMax = 2;
            this.Length = length;
            this.TheMin = (int)min;
            this.TheMax = (int)max;
            this.inicializarCromosoma();
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                int num2 = (int)Poblacion.vacantes[i];
                for (int j = 0; j < num2; j++)
                {
                    int trabajador = TheSeed.Next(0, Poblacion.numTrabajadores);
                    if (!this.trabajadorDisponible(trabajador))
                    {
                        j--;
                    }
                    else
                    {
                        int num5 = (trabajador * Poblacion.numPuestosDeTrabajo) + i;
                        this.TheArray[num5] = 1;
                    }
                }
            }
        }

        public float CalcularFitness()
        {
            this.FitnessActual = this.CalcularProduccion();
            return this.FitnessActual;
        }

        private float CalcularProduccion()
        {
            this.FitnessActual = 0f;
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                for (int j = 0; j < Poblacion.numTrabajadores; j++)
                {
                    int num2 = (j * Poblacion.numPuestosDeTrabajo) + i;
                    int num = (int)this.TheArray[num2];
                    double num3 = (double)Poblacion.indiceError[num2];
                    double num4 = (int)Poblacion.indiceTiempo[num2];
                    int duracionTurno = Poblacion.duracionTurno;
                    if (num > 0)
                    {
                        this.FitnessActual += (float)Math.Truncate((double)(((double)duracionTurno) / (((1.0 + num3) * num4) * num)));
                    }
                }
            }
            return this.FitnessActual;
        }

        public void CopiarCromosoma(out Cromosoma dest)
        {
            dest = new Cromosoma();
            dest.Length = this.Length;
            dest.TheMin = this.TheMin;
            dest.TheMax = this.TheMax;
            dest.FitnessActual = this.FitnessActual;
            for (int i = 0; i < this.Length; i++)
            {
                dest.TheArray.Add(this.TheArray[i]);
            }
        }

        public void CopiarInformacionCromosoma(Cromosoma dest)
        {
            Cromosoma cromosoma = dest;
            cromosoma.Length = this.Length;
            cromosoma.TheMin = this.TheMin;
            cromosoma.TheMax = this.TheMax;
        }

        public void Cruzar_uniforme(ref Cromosoma cromosoma2, out Cromosoma hijo1, out Cromosoma hijo2)
        {
            hijo1 = new Cromosoma();
            hijo2 = new Cromosoma();
            cromosoma2.CopiarInformacionCromosoma(hijo1);
            cromosoma2.CopiarInformacionCromosoma(hijo2);
            int num = 0;
            for (int i = 0; i < Poblacion.numTrabajadores; i++)
            {
                for (int j = 0; j < Poblacion.numPuestosDeTrabajo; j++)
                {
                    if ((i % 2) == 0)
                    {
                        hijo1.TheArray.Add(this.TheArray[num]);
                        hijo2.TheArray.Add(cromosoma2.TheArray[num]);
                    }
                    else
                    {
                        hijo2.TheArray.Add(this.TheArray[num]);
                        hijo1.TheArray.Add(cromosoma2.TheArray[num]);
                    }
                    num++;
                }
            }
        }

        public void Cruzar_unpunto(ref Cromosoma cromosoma2, out Cromosoma hijo1, out Cromosoma hijo2)
        {
            int num = TheSeed.Next(0, Poblacion.numTrabajadores);
            int num2 = TheSeed.Next(0, Poblacion.numTrabajadores);
            hijo1 = new Cromosoma();
            hijo2 = new Cromosoma();
            this.CopiarCromosoma(out hijo1);
            cromosoma2.CopiarCromosoma(out hijo2);
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                int num5 = (num * Poblacion.numPuestosDeTrabajo) + i;
                int num6 = (num2 * Poblacion.numPuestosDeTrabajo) + i;
                int num3 = (int)hijo1.TheArray[num5];
                hijo1.TheArray[num5] = hijo2.TheArray[num6];
                hijo2.TheArray[num6] = num3;
            }
        }

        public bool esValido()
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                list.Add(0);
            }
            int num = 0;
            int num2 = 0;
            for (int j = 0; j < Poblacion.numTrabajadores; j++)
            {
                int num5 = 0;
                for (int k = 0; k < Poblacion.numPuestosDeTrabajo; k++)
                {
                    num5 += (int)this.TheArray[num];
                    if (((int)this.TheArray[num]) == 1)
                    {
                        num2 = num % Poblacion.numPuestosDeTrabajo;
                        list[num2] = ((int)list[num2]) + 1;
                    }
                    if (((int)list[num2]) > ((int)Poblacion.vacantes[num2]))
                    {
                        return false;
                    }
                    if (num5 > 1)
                    {
                        return false;
                    }
                    num++;
                }
                if (num5 != 1)
                {
                    return false;
                }
            }
            return true;
        }

        private void inicializarCromosoma()
        {
            for (int i = 0; i < this.Length; i++)
            {
                this.TheArray.Add(0);
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
                int num3 = 0;
                for (int k = 0; k < Poblacion.numPuestosDeTrabajo; k++)
                {
                    int num5 = (j * Poblacion.numPuestosDeTrabajo) + k;
                    num3 += (int)this.TheArray[num5];
                    if (((int)this.TheArray[num5]) == 1)
                    {
                        Console.WriteLine("Asignado a Puesto " + (k + 1));
                    }
                }
                if (num3 == 0)
                {
                    Console.WriteLine("No asignado a ning\x00fan puesto de trabajo");
                }
            }
        }

        public void mostrarCromosoma()
        {
            Console.WriteLine(this.ToString());
        }

        public void Mutar()
        {
            int num = TheSeed.Next(Poblacion.numTrabajadores);
            int numPuestosDeTrabajo = Poblacion.numPuestosDeTrabajo;
            int num3 = 0;
            int num4 = (numPuestosDeTrabajo * num) + TheSeed.Next(Poblacion.numPuestosDeTrabajo);
            for (int i = 0; i < numPuestosDeTrabajo; i++)
            {
                num3 = (numPuestosDeTrabajo * num) + i;
                if (((int)this.TheArray[num3]) == 1)
                {
                    this.TheArray[num3] = 0;
                    break;
                }
            }
            this.TheArray[num4] = 1;
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < this.Length; i++)
            {
                str = str + ((int)this.TheArray[i]).ToString() + " ";
            }
            return (str + "-->" + this.FitnessActual.ToString());
        }

        private bool trabajadorDisponible(int trabajador)
        {
            int num = trabajador * Poblacion.numPuestosDeTrabajo;
            int num2 = 0;
            for (int i = num; i < (num + Poblacion.numPuestosDeTrabajo); i++)
            {
                num2 += (int)this.TheArray[i];
            }
            if (num2 > 0)
            {
                return false;
            }
            return true;
        }

    }
}
