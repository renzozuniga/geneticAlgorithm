using System;
using System.Collections;

namespace ConsoleApp1
{
    /// <summary>
    /// Summary description for Population.
    /// </summary>
    public class Population
    {

        public static int numWorkplaces;
        public static int numWorkers;
        public static int turnDuration; // 8 horas (expresado en minutos)
        public static ArrayList errorIndex = new ArrayList(); // R: indice de rotura
        public static ArrayList timeIndex = new ArrayList(); // T: tiempo que se demora el trabajador en un puesto de trabajo

        int kCrossover;
        const int kInitialPopulation = 1000;
        const int kPopulationLimit = 1000;
        const int kMin = 0;
        const int kMax = 2;
        const float kMutationFrequency = 0.10f;
        const float kDeathFitness = 0.00f;
        const float kReproductionFitness = 0.0f;

        private double totalFitness;
        private ArrayList fitnessTable = new ArrayList();

        ArrayList Genomes = new ArrayList();
        ArrayList GenomeReproducers = new ArrayList();
        ArrayList GenomeResults = new ArrayList();
        ArrayList GenomeFamily = new ArrayList();

        int CurrentPopulation = kInitialPopulation;
        int Generation = 1;

        public Population(int nwr, int nwp, int td, ArrayList ei, ArrayList ti)
        {
            numWorkers = nwr;
            numWorkplaces = nwp;
            turnDuration = td;
            errorIndex = (ArrayList)ei.Clone();
            timeIndex = (ArrayList)ti.Clone();

            kCrossover = numWorkplaces / 2;

            for (int i = 0; i < kInitialPopulation; i++)
            {
                ListGenome aGenome = new ListGenome(numWorkplaces * numWorkers, kMin, kMax);
                aGenome.SetCrossoverPoint(kCrossover* numWorkers);
                aGenome.CalculateFitness();
                Genomes.Add(aGenome);
            }

            RankPopulation();

        }

        private void Mutate(Genome aGene)
        {
            if (ListGenome.TheSeed.Next(100) < (int)(kMutationFrequency * 100.0))
            {
                aGene.Mutate();
            }
        }

        public void NextGeneration()
        {
            // increment the generation;
            Generation++;


            // check who can die
            for (int i = 0; i < Genomes.Count; i++)
            {
                if (((Genome)Genomes[i]).CanDie(kDeathFitness))
                {
                    Genomes.RemoveAt(i);
                    i--;
                }
            }


            // determine who can reproduce
            GenomeReproducers.Clear();
            GenomeResults.Clear();
            

            for (int i = 0; i < Genomes.Count; i++)
            {
                if (((Genome)Genomes[i]).CanReproduce(kReproductionFitness))
                {
                    GenomeReproducers.Add(Genomes[i]);
                }
            }

            // do the crossover of the genes and add them to the population
            //CurrentPopulation = GenomeReproducers.Count; 
            DoCrossover(GenomeReproducers);

            Genomes = (ArrayList)GenomeResults.Clone();

            // calculate fitness of all the genes
            for (int i = 0; i < Genomes.Count; i++)
            {
                ((Genome)Genomes[i]).CalculateFitness();
            }


            //			Genomes.Sort();

            // kill all the genes above the population limit
            if (Genomes.Count > kPopulationLimit)
                Genomes.RemoveRange(kPopulationLimit, Genomes.Count - kPopulationLimit);

            CurrentPopulation = Genomes.Count;
            RankPopulation();

        }

        public void CalculateFitnessForAll(ArrayList genes)
        {
            foreach (ListGenome lg in genes)
            {
                lg.CalculateFitness();
            }
        }

        private double crossoverRate = 0.80;

        public void initializeArray(ArrayList array)
        {
            for(int i=0; i<CurrentPopulation; i++)
            {
                array.Add(0);
            }
        }

        public void DoCrossover(ArrayList genes)
        {
            GenomeResults.Clear();
            ArrayList GeneMoms = new ArrayList();
            ArrayList GeneDads = new ArrayList();

            for (int i = 0; i < genes.Count; i++)
            {
                // randomly pick the moms and dad's
                if (ListGenome.TheSeed.Next(100) % 2 > 0)
                {
                    GeneMoms.Add(genes[i]);
                }
                else
                {
                    GeneDads.Add(genes[i]);
                }
            }

            //  now make them equal
            if (GeneMoms.Count > GeneDads.Count)
            {
                while (GeneMoms.Count > GeneDads.Count)
                {
                    GeneDads.Add(GeneMoms[GeneMoms.Count - 1]);
                    GeneMoms.RemoveAt(GeneMoms.Count - 1);
                }

                if (GeneDads.Count > GeneMoms.Count)
                {
                    GeneDads.RemoveAt(GeneDads.Count - 1); // make sure they are equal
                }

            }
            else
            {
                while (GeneDads.Count > GeneMoms.Count)
                {
                    GeneMoms.Add(GeneDads[GeneDads.Count - 1]);
                    GeneDads.RemoveAt(GeneDads.Count - 1);
                }

                if (GeneMoms.Count > GeneDads.Count)
                {
                    GeneMoms.RemoveAt(GeneMoms.Count - 1); // make sure they are equal
                }
            }

            // now cross them over and add them according to fitness
            for (int i = 0; i < GeneDads.Count; i += 1)
            {
                int pidx1 = RouletteSelection();
                int pidx2 = RouletteSelection();
                Genome parent1, parent2, child1, child2;
                parent1 = ((Genome)GeneDads[pidx1]);
                parent2 = ((Genome)GeneMoms[pidx2]);

                if (ListGenome.TheSeed.NextDouble() < crossoverRate)
                {
                    parent1.Crossover2(ref parent2, out child1, out child2);
                }
                else
                {
                    child1 = parent1;
                    child2 = parent2;
                }
                child1.Mutate();
                child2.Mutate();

                GenomeResults.Add(child1);
                GenomeResults.Add(child2);
                
            }



        }

        public void WriteNextGeneration()
        {
            // just write the top 20
            Console.WriteLine("Generation {0}\n", Generation);
            for (int i = 0; i < CurrentPopulation; i++)
            {
                Console.WriteLine(((Genome)Genomes[i]).ToString());
            }

            Console.WriteLine("Hit the enter key to continue...\n");
            Console.ReadLine();
        }

        private int RouletteSelection()
        {
            double randomFitness = ListGenome.TheSeed.NextDouble() * totalFitness;
            int idx = -1;
            int mid;
            int first = 0;
            int last = CurrentPopulation/2 - 1;
            mid = (last - first) / 2;

            //  ArrayList's BinarySearch is for exact values only
            //  so do this by hand.
            while (idx == -1 && first <= last)
            {
                if (randomFitness < (double)fitnessTable[mid])
                {
                    last = mid;
                }
                else if (randomFitness > (double)fitnessTable[mid])
                {
                    first = mid;
                }
                mid = (first + last) / 2;
                //  lies between i and i+1
                if ((last - first) == 1)
                    idx = last;
            }
            return idx;
        }

        private void RankPopulation()
        {
            totalFitness = 0;
            for (int i = 0; i < CurrentPopulation; i++)
            {
                Genome g = ((Genome)Genomes[i]);
                totalFitness += g.CurrentFitness;
            }
            Genomes.Sort(new GenomeComparer());

            //  now sorted in order of fitness.
            double fitness = 0.0;
            fitnessTable.Clear();
            for (int i = 0; i < CurrentPopulation; i++)
            {
                fitness += ((Genome)Genomes[i]).CurrentFitness;
                fitnessTable.Add((double)fitness);
            }
        }
    }
}
