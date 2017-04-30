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
        private const int kMax = 2;
        private const int kMin = 0;
        public static int numPuestosDeTrabajo;
        public static int numTrabajadores;

        private int PoblacionActual = poblacionInicial;
        private const int poblacionInicial = 1000;
        private const int poblacionLimite = 1000;

        private const double frecuenciaCruce = 0.8;
        private const double frecuenciaMutacion = 0.2;

        private ArrayList tablaFitness = new ArrayList();
        private double totalFitness;

        public static ArrayList trabajadores = new ArrayList();
        public static ArrayList procesos = new ArrayList();

        //Este constructor, genera la población inicial
        public Poblacion(ArrayList trab, ArrayList proc, int td)
        {
            //Se inicializan las variables principales de Población
            numTrabajadores = trab.Count;
            numPuestosDeTrabajo = proc.Count;
            duracionTurno = td;
            procesos = proc;
            trabajadores = trab;

            for (int i = 0; i < poblacionInicial; i++)
            {
                //Aquí se generan los cromosomas de la poblacion inicial
                Cromosoma cromosoma = new Cromosoma(numPuestosDeTrabajo * numTrabajadores, kMin, kMax);
                cromosoma.CalcularFitness();
                Cromosomas.Add(cromosoma);
            }
            //Se proceden a ordenar los cromosomas de menor a mayor fitness
            RankPopulation();
        }

        private void repartirCromosomas(ArrayList CromosomasTotales, ArrayList CromosomasMadre, ArrayList CromosomasPadre)
        {
            for (int i = 0; i < CromosomasTotales.Count; i++)
            {
                if ((Cromosoma.TheSeed.Next(100) % 2) > 0)
                {
                    CromosomasMadre.Add(CromosomasTotales[i]);
                }
                else
                {
                    CromosomasPadre.Add(CromosomasTotales[i]);
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
        }

        //Esta función acciona el operador genético de cruce y de mutación
        public void Cruce(ArrayList cromosomas)
        {
            CromosomasResultantes.Clear();
            ArrayList CromosomasMadre = new ArrayList();
            ArrayList CromosomasPadre = new ArrayList();

            //Se procede a repartir equitativamente los cromosomas para generar los cruces posteriores
            repartirCromosomas(cromosomas, CromosomasMadre, CromosomasPadre);

            //Se proceden a realizar los cruces entre padre y madre escogiendo estos por el método de la ruleta
            for (int j = 0; j < CromosomasPadre.Count; j++)
            {
                Cromosoma hijo1;
                Cromosoma hijo2;
                int ind1 = RouletteSelection();   //Indica el indice del cromosoma padre
                int ind2 = RouletteSelection();   //Indica el indice del cromosoma madre
                Cromosoma padre = (Cromosoma)CromosomasPadre[ind1];
                Cromosoma madre = (Cromosoma)CromosomasMadre[ind2];

                //Dependiendo de la frecuencia de cruce, puede que se haga cruce o no
                if (Cromosoma.TheSeed.NextDouble() < frecuenciaCruce)
                {
                    //Se procede a accionar el operador genético de cruce de tipo uniforme
                    padre.Cruce_uniforme(ref madre, out hijo1, out hijo2);

                    //Se verifica que los cromosomas después del cruce sigan siendo validos en estructura
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

                // Dependiendo de la frecuencia de mutación, puede que se mute o no
                if (Cromosoma.TheSeed.NextDouble() < frecuenciaMutacion)
                {
                    // Se procede a accionar el operador genético de mutación
                    hijo1.Mutar_intercambio();
                    hijo2.Mutar_intercambio();

                    // Se verifica que los cromosomas después de la mutación sigan siendo validos en estructura
                    if (!hijo1.esValido())
                    {
                        padre.CopiarCromosoma(out hijo1);
                    }
                    if (!hijo2.esValido())
                    {
                        madre.CopiarCromosoma(out hijo2);
                    }
                }

                // Se agrupan los hijos para generar la siguiente generación
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
