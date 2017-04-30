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
  
        private ArrayList Cromosomas = new ArrayList();
        private ArrayList CromosomasResultantes = new ArrayList();
        public static int duracionTurno;
        private int Generacion = 1;
        public static ArrayList indiceRotura = new ArrayList();
        public static ArrayList indiceTiempo = new ArrayList();
        private const float kDeathFitness = 0.0f;
        private const int kMax = 2;
        private const int kMin = 0;
        private const float kMutationFrequency = 0.1f;
        private const float kReproductionFitness = 0.0f;
        public static int numPuestosDeTrabajo;
        public static int numTrabajadores;
        private int PoblacionActual = poblacionInicial;
        private const int poblacionInicial = 1000;
        private const int poblacionLimite = 1000;
        private double ratioCruce = 0.8;
        private ArrayList tablaFitness = new ArrayList();
        private double totalFitness;
        public static ArrayList vacantes = new ArrayList();

        //Este constructor, genera la población inicial
        public Poblacion(int nwr, int nwp, int td, ArrayList mTP, ArrayList ei, ArrayList ti)
        {
            numTrabajadores = nwr;
            numPuestosDeTrabajo = nwp;
            duracionTurno = td;
            vacantes = (ArrayList)mTP.Clone();
            indiceRotura = (ArrayList)ei.Clone();
            indiceTiempo = (ArrayList)ti.Clone();
            for (int i = 0; i < poblacionInicial; i++)
            {
                //Aquí se generan los cromosomas de la poblacion inicial
                Cromosoma cromosoma = new Cromosoma(numPuestosDeTrabajo * numTrabajadores, 0, 2);
                cromosoma.CalcularFitness();
                Cromosomas.Add(cromosoma);
            }
            //Se proceden a ordenar los cromosomas de menor a mayor fitness
            RankPopulation();
        }

        //Esta función acciona el operador genético de cruce y de mutación
        public void Cruce(ArrayList cromosomas)
        {
            int cantidadMalos = 0;                         //Indica la cantidad de cruces malos entre cromosomas en la generacion
            CromosomasResultantes.Clear();
            ArrayList CromosomasMadre = new ArrayList();
            ArrayList CromosomasPadre = new ArrayList();

            //Se procede a repartir equitativamente los cromosomas para generar los cruces posterirores
            for (int i = 0; i < cromosomas.Count; i++)
            {
                if ((Cromosoma.TheSeed.Next(100) % 2) > 0)
                {
                    CromosomasMadre.Add(cromosomas[i]);
                }
                else
                {
                    CromosomasPadre.Add(cromosomas[i]);
                }
            }
            if (CromosomasMadre.Count <= CromosomasPadre.Count)
            {
                while (CromosomasPadre.Count > CromosomasMadre.Count)
                {
                    CromosomasMadre.Add(CromosomasPadre[CromosomasPadre.Count - 1]);
                    CromosomasPadre.RemoveAt(CromosomasPadre.Count - 1);
                }
                if (CromosomasMadre.Count > CromosomasPadre.Count)
                {
                    CromosomasMadre.RemoveAt(CromosomasMadre.Count - 1);
                }
            }
            else
            {
                while (CromosomasMadre.Count > CromosomasPadre.Count)
                {
                    CromosomasPadre.Add(CromosomasMadre[CromosomasMadre.Count - 1]);
                    CromosomasMadre.RemoveAt(CromosomasMadre.Count - 1);
                }
                if (CromosomasPadre.Count > CromosomasMadre.Count)
                {
                    CromosomasPadre.RemoveAt(CromosomasPadre.Count - 1);
                }
            }

            //Se proceden a realizar los cruces entre padre y madre escogiendo estos por el método de la ruleta
            for (int j = 0; j < CromosomasPadre.Count; j++)
            {
                Cromosoma hijo1;
                Cromosoma hijo2;
                int ind1 = RouletteSelection();   //Indica el indice del cromosoma padre
                int ind2 = RouletteSelection();   //Indica el indice del cromosoma madre
                Cromosoma padre = (Cromosoma)CromosomasPadre[ind1];
                Cromosoma madre = (Cromosoma)CromosomasMadre[ind2];

                //Dependiendo del ratioCruce, puede que se haga cruce o no
                if (Cromosoma.TheSeed.NextDouble() < this.ratioCruce)
                {
                    //Se procede a accionar el operador genético de cruce de tipo uniforme
                    padre.Cruzar_uniforme(ref madre, out hijo1, out hijo2);

                    //Se verifica que los cromosomas después del cruce sigan siendo validos en estructura
                    if (!hijo1.esValido())
                    {
                        cantidadMalos++;
                        padre.CopiarCromosoma(out hijo1);
                    }
                    if (!hijo2.esValido())
                    {
                        cantidadMalos++;
                        madre.CopiarCromosoma(out hijo2);
                    }
                }
                else
                {
                    padre.CopiarCromosoma(out hijo1);
                    madre.CopiarCromosoma(out hijo2);
                }

                //Se procede a accionar el operador genético de mutación
                hijo1.Mutar();
                hijo2.Mutar();
                //Se verifica que los cromosomas después de la mutación sigan siendo validos en estructura
                if (!hijo1.esValido())
                {
                    padre.CopiarCromosoma(out hijo1);
                }
                if (!hijo2.esValido())
                {
                    madre.CopiarCromosoma(out hijo2);
                }

                //Se agrupan los hijos para generar la siguiente generación
                CromosomasResultantes.Add(hijo1);
                CromosomasResultantes.Add(hijo2);
            }
            Console.WriteLine("Cruces no validos: " + cantidadMalos);
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

        //Devuelve el mejor cromosoma de la generación actual
        public Cromosoma obtenerMejorCromosoma()
        {
            Cromosoma valor = (Cromosoma)Cromosomas[PoblacionActual - 1];
            return valor;
        }

        //Ordena los cromosomas de la generación actual y actualiza el fitness total de dicha generación
        private void RankPopulation()
        {
            totalFitness = 0.0;
            for (int i = 0; i < PoblacionActual; i++)
            {
                Cromosoma cromosoma = (Cromosoma)Cromosomas[i];
                totalFitness += cromosoma.FitnessActual;
            }
            Cromosomas.Sort(new ComparadorCromosoma());
            double fitness = 0.0;
            tablaFitness.Clear();
            for (int j = 0; j < PoblacionActual; j++)
            {
                fitness += ((Cromosoma)Cromosomas[j]).FitnessActual;
                tablaFitness.Add(fitness);
            }
        }

        //Devuelve el indice del cromosoma seleccionado usando el método de la ruleta
        private int RouletteSelection()
        {
            double randomFitness = Cromosoma.TheSeed.NextDouble() * totalFitness;
            int ind = -1;
            int medio = 0;
            int primero = (PoblacionActual / 2) - 1;
            int ultimo = (primero - medio) / 2;
            while ((ind == -1) && (medio <= primero))
            {
                if (randomFitness < ((double)tablaFitness[ultimo]))
                {
                    primero = ultimo;
                }
                else if (randomFitness > ((double)tablaFitness[ultimo]))
                {
                    medio = ultimo;
                }
                ultimo = (medio + primero) / 2;
                if ((primero - medio) == 1)
                {
                    ind = primero;
                }
            }
            return ind;
        }

        //Se genera la siguiente generación
        public void SiguienteGeneracion()
        {
            Generacion++;
            CromosomasResultantes.Clear();

            //Se realiza el cruce de cromosomas 
            Cruce(Cromosomas);

            Cromosomas = (ArrayList)CromosomasResultantes.Clone();
            for (int i = 0; i < Cromosomas.Count; i++)
            {
                ((Cromosoma)Cromosomas[i]).CalcularFitness();
            }
            if (Cromosomas.Count > poblacionLimite)
            {
                Cromosomas.RemoveRange(poblacionLimite, Cromosomas.Count - poblacionLimite);
            }
            PoblacionActual = Cromosomas.Count;
            RankPopulation();
        }

    }
}
