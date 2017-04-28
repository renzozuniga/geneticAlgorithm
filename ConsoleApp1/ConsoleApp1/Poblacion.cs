using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoGenetico
{
    class Poblacion
    {
        public static int numPuestosDeTrabajo;
        public static int numTrabajadores;
        public static int duracionTurno; // 8 horas (expresado en minutos)
        public static ArrayList indiceError = new ArrayList(); // R: indice de rotura
        public static ArrayList indiceTiempo = new ArrayList(); // T: tiempo que se demora el trabajador en un puesto de trabajo

        const int poblacionInicial = 1000;
        const int poblacionLimite = 1000;
        const int kMin = 0;
        const int kMax = 2;
        const float kMutationFrequency = 0.10f;
        const float kDeathFitness = 0.00f;
        const float kReproductionFitness = 0.0f;
        private double ratioCruce = 0.80;


        private double totalFitness;
        private ArrayList tablaFitness = new ArrayList();

        ArrayList Cromosomas = new ArrayList();
        ArrayList CromosomasResultantes = new ArrayList();

        int PoblacionActual = poblacionInicial;
        int Generacion = 1;

        public Poblacion(int nwr, int nwp, int td, ArrayList ei, ArrayList ti)
        {
            numTrabajadores = nwr;    //número de trabajadores
            numPuestosDeTrabajo = nwp;    //número de puestos de trabajo
            duracionTurno = td;  //tiempo de duración del turno
            indiceError = (ArrayList)ei.Clone();
            indiceTiempo = (ArrayList)ti.Clone();

            for (int i = 0; i < poblacionInicial; i++)
            {
                Cromosoma cromosoma = new Cromosoma(numPuestosDeTrabajo * numTrabajadores, kMin, kMax);
                cromosoma.CalcularFitness();
                Cromosomas.Add(cromosoma);

            }

            RankPopulation();

        }

        public void SiguienteGeneracion()
        {
            // incrementar la generacion;
            Generacion++;
            CromosomasResultantes.Clear();

            // cruzar los cromosomas
            Cruce(Cromosomas);
            Cromosomas = (ArrayList)CromosomasResultantes.Clone();

            // calcular el fitness de cada cromosoma
            for (int i = 0; i < Cromosomas.Count; i++)
            {
                ((Cromosoma)Cromosomas[i]).CalcularFitness();
            }

            // kill all the genes above the population limit
            if (Cromosomas.Count > poblacionLimite)
                Cromosomas.RemoveRange(poblacionLimite, Cromosomas.Count - poblacionLimite);

            PoblacionActual = Cromosomas.Count;
            RankPopulation();

        }

        public void Cruce(ArrayList cromosomas)
        {
            CromosomasResultantes.Clear();
            ArrayList CromosomasMadre = new ArrayList();
            ArrayList CromosomasPadre = new ArrayList();

            for (int i = 0; i < cromosomas.Count; i++)
            {
                // randomly pick the moms and dad's
                if (Cromosoma.TheSeed.Next(100) % 2 > 0)
                {
                    CromosomasMadre.Add(cromosomas[i]);
                }
                else
                {
                    CromosomasPadre.Add(cromosomas[i]);
                }
            }

            //  repartir equitativamente padres y madres
            if (CromosomasMadre.Count > CromosomasPadre.Count)
            {
                while (CromosomasMadre.Count > CromosomasPadre.Count)
                {
                    CromosomasPadre.Add(CromosomasMadre[CromosomasMadre.Count - 1]);
                    CromosomasMadre.RemoveAt(CromosomasMadre.Count - 1);
                }

                if (CromosomasPadre.Count > CromosomasMadre.Count)
                {
                    CromosomasPadre.RemoveAt(CromosomasPadre.Count - 1); // asegurarse que sean iguales
                }

            }
            else
            {
                while (CromosomasPadre.Count > CromosomasMadre.Count)
                {
                    CromosomasMadre.Add(CromosomasPadre[CromosomasPadre.Count - 1]);
                    CromosomasPadre.RemoveAt(CromosomasPadre.Count - 1);
                }

                if (CromosomasMadre.Count > CromosomasPadre.Count)
                {
                    CromosomasMadre.RemoveAt(CromosomasMadre.Count - 1); // asegurarse que sean iguales
                }
            }

            // now cross them over and add them according to fitness
            for (int i = 0; i < CromosomasPadre.Count; i += 1)
            {
                Cromosoma padre, madre, hijo1, hijo2;
                int ind1 = RouletteSelection();
                int ind2 = RouletteSelection();
                padre = ((Cromosoma)CromosomasPadre[ind1]);
                madre = ((Cromosoma)CromosomasMadre[ind2]);

                if (Cromosoma.TheSeed.NextDouble() < ratioCruce)
                {
                    padre.Cruzar(ref madre, out hijo1, out hijo2);
                    if (!hijo1.esValido())
                    {
                        padre.CopiarCromosoma(out hijo1);
                    }
                    if (!hijo2.esValido())
                    {
                        madre.CopiarCromosoma(out hijo2);
                    }
                }
                else
                {
                    padre.CopiarCromosoma(out hijo1);
                    madre.CopiarCromosoma(out hijo2);
                }

                hijo1.Mutar();
                hijo2.Mutar();

                CromosomasResultantes.Add(hijo1);
                CromosomasResultantes.Add(hijo2);

            }

        }

        public void ImprimirGeneracion()
        {
            Console.WriteLine("Generacion {0}\n", Generacion);
            for (int i = 0; i < PoblacionActual; i++)
            {
                Console.WriteLine(((Cromosoma)Cromosomas[i]).ToString());
            }

            Console.WriteLine("Presione Enter para continuar...\n");
            Console.ReadLine();
        }

        private int RouletteSelection()
        {
            double randomFitness = Cromosoma.TheSeed.NextDouble() * totalFitness;
            int ind = -1;
            int medio;
            int primero = 0;
            int ultimo = PoblacionActual / 2 - 1;
            medio = (ultimo - primero) / 2;

            //  ArrayList's BinarySearch is for exact values only
            //  so do this by hand.
            while (ind == -1 && primero <= ultimo)
            {
                if (randomFitness < (double)tablaFitness[medio])
                {
                    ultimo = medio;
                }
                else if (randomFitness > (double)tablaFitness[medio])
                {
                    primero = medio;
                }
                medio = (primero + ultimo) / 2;
                //  lies between i and i+1
                if ((ultimo - primero) == 1)
                    ind = ultimo;
            }
            return ind;
        }

        private void RankPopulation()
        {
            totalFitness = 0;
            for (int i = 0; i < PoblacionActual; i++)
            {
                Cromosoma c = ((Cromosoma)Cromosomas[i]);
                totalFitness += c.FitnessActual;
            }
            Cromosomas.Sort(new ComparadorCromosoma());

            //  ordenar en orden de fitness.
            double fitness = 0.0;
            tablaFitness.Clear();
            for (int i = 0; i < PoblacionActual; i++)
            {
                fitness += ((Cromosoma)Cromosomas[i]).FitnessActual;
                tablaFitness.Add((double)fitness);
            }
        }
    }
}
