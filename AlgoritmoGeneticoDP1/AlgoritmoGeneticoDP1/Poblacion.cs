using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace AlgoritmoGeneticoDP1
{
    class Poblacion
    {
        // Fields
        private ArrayList Cromosomas = new ArrayList();
        private ArrayList CromosomasResultantes = new ArrayList();
        public static int duracionTurno;
        private int Generacion = 1;
        public static ArrayList indiceError = new ArrayList();
        public static ArrayList indiceTiempo = new ArrayList();
        private const float kDeathFitness = 0f;
        private const int kMax = 2;
        private const int kMin = 0;
        private const float kMutationFrequency = 0.1f;
        private const float kReproductionFitness = 0f;
        public static int numPuestosDeTrabajo;
        public static int numTrabajadores;
        private int PoblacionActual = 0x3e8;
        private const int poblacionInicial = 0x3e8;
        private const int poblacionLimite = 0x3e8;
        private double ratioCruce = 0.8;
        private ArrayList tablaFitness = new ArrayList();
        private double totalFitness;
        public static ArrayList vacantes = new ArrayList();

        // Methods
        public Poblacion(int nwr, int nwp, int td, ArrayList mTP, ArrayList ei, ArrayList ti)
        {
            numTrabajadores = nwr;
            numPuestosDeTrabajo = nwp;
            duracionTurno = td;
            vacantes = (ArrayList)mTP.Clone();
            indiceError = (ArrayList)ei.Clone();
            indiceTiempo = (ArrayList)ti.Clone();
            for (int i = 0; i < 0x3e8; i++)
            {
                Cromosoma cromosoma = new Cromosoma((long)(numPuestosDeTrabajo * numTrabajadores), 0, 2);
                cromosoma.CalcularFitness();
                this.Cromosomas.Add(cromosoma);
            }
            this.RankPopulation();
        }

        public void Cruce(ArrayList cromosomas)
        {
            int num = 0;
            this.CromosomasResultantes.Clear();
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            for (int i = 0; i < cromosomas.Count; i++)
            {
                if ((Cromosoma.TheSeed.Next(100) % 2) > 0)
                {
                    list.Add(cromosomas[i]);
                }
                else
                {
                    list2.Add(cromosomas[i]);
                }
            }
            if (list.Count <= list2.Count)
            {
                while (list2.Count > list.Count)
                {
                    list.Add(list2[list2.Count - 1]);
                    list2.RemoveAt(list2.Count - 1);
                }
                if (list.Count > list2.Count)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
            else
            {
                while (list.Count > list2.Count)
                {
                    list2.Add(list[list.Count - 1]);
                    list.RemoveAt(list.Count - 1);
                }
                if (list2.Count > list.Count)
                {
                    list2.RemoveAt(list2.Count - 1);
                }
            }
            for (int j = 0; j < list2.Count; j++)
            {
                Cromosoma cromosoma3;
                Cromosoma cromosoma4;
                int num4 = this.RouletteSelection();
                int num5 = this.RouletteSelection();
                Cromosoma cromosoma = (Cromosoma)list2[num4];
                Cromosoma cromosoma2 = (Cromosoma)list[num5];
                if (Cromosoma.TheSeed.NextDouble() < this.ratioCruce)
                {
                    cromosoma.Cruzar_uniforme(ref cromosoma2, out cromosoma3, out cromosoma4);
                    if (!cromosoma3.esValido())
                    {
                        num++;
                        cromosoma.CopiarCromosoma(out cromosoma3);
                    }
                    if (!cromosoma4.esValido())
                    {
                        num++;
                        cromosoma2.CopiarCromosoma(out cromosoma4);
                    }
                }
                else
                {
                    cromosoma.CopiarCromosoma(out cromosoma3);
                    cromosoma2.CopiarCromosoma(out cromosoma4);
                }
                cromosoma3.Mutar();
                cromosoma4.Mutar();
                if (!cromosoma3.esValido())
                {
                    cromosoma.CopiarCromosoma(out cromosoma3);
                }
                if (!cromosoma4.esValido())
                {
                    cromosoma2.CopiarCromosoma(out cromosoma4);
                }
                this.CromosomasResultantes.Add(cromosoma3);
                this.CromosomasResultantes.Add(cromosoma4);
            }
            Console.WriteLine("Cruces no validos: " + num);
        }

        public void ImprimirGeneracion()
        {
            Console.WriteLine("Generacion {0}\n", this.Generacion);
            for (int i = 0; i < this.PoblacionActual; i++)
            {
                Console.WriteLine(((Cromosoma)this.Cromosomas[i]).ToString());
            }
            Console.WriteLine("Presione Enter para continuar...\n");
            Console.ReadLine();
        }

        public Cromosoma obtenerMejorCromosoma() =>
            ((Cromosoma)this.Cromosomas[this.PoblacionActual - 1]);

        private void RankPopulation()
        {
            this.totalFitness = 0.0;
            for (int i = 0; i < this.PoblacionActual; i++)
            {
                Cromosoma cromosoma = (Cromosoma)this.Cromosomas[i];
                this.totalFitness += cromosoma.FitnessActual;
            }
            this.Cromosomas.Sort(new ComparadorCromosoma());
            double num = 0.0;
            this.tablaFitness.Clear();
            for (int j = 0; j < this.PoblacionActual; j++)
            {
                num += ((Cromosoma)this.Cromosomas[j]).FitnessActual;
                this.tablaFitness.Add(num);
            }
        }

        private int RouletteSelection()
        {
            double num = Cromosoma.TheSeed.NextDouble() * this.totalFitness;
            int num2 = -1;
            int num4 = 0;
            int num5 = (this.PoblacionActual / 2) - 1;
            int num3 = (num5 - num4) / 2;
            while ((num2 == -1) && (num4 <= num5))
            {
                if (num < ((double)this.tablaFitness[num3]))
                {
                    num5 = num3;
                }
                else if (num > ((double)this.tablaFitness[num3]))
                {
                    num4 = num3;
                }
                num3 = (num4 + num5) / 2;
                if ((num5 - num4) == 1)
                {
                    num2 = num5;
                }
            }
            return num2;
        }

        public void SiguienteGeneracion()
        {
            this.Generacion++;
            this.CromosomasResultantes.Clear();
            this.Cruce(this.Cromosomas);
            this.Cromosomas = (ArrayList)this.CromosomasResultantes.Clone();
            for (int i = 0; i < this.Cromosomas.Count; i++)
            {
                ((Cromosoma)this.Cromosomas[i]).CalcularFitness();
            }
            if (this.Cromosomas.Count > 0x3e8)
            {
                this.Cromosomas.RemoveRange(0x3e8, this.Cromosomas.Count - 0x3e8);
            }
            this.PoblacionActual = this.Cromosomas.Count;
            this.RankPopulation();
        }

    }
}
