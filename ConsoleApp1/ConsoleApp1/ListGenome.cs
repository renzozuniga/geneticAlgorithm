using System;
using System.Collections;

namespace ConsoleApp1
{
    /// <summary>
    /// Summary description for ListGenome.
    /// </summary>
    public class ListGenome : Genome
    {
        ArrayList TheArray = new ArrayList();
        public static Random TheSeed = new Random((int)DateTime.Now.Ticks);
        int TheMin = 0;
        int TheMax = 2;

        public override int CompareTo(object a)
        {
            ListGenome Gene1 = this;
            ListGenome Gene2 = (ListGenome)a;
            return Math.Sign(Gene2.CurrentFitness - Gene1.CurrentFitness);
        }


        public override void SetCrossoverPoint(int crossoverPoint)
        {
            CrossoverPoint = crossoverPoint;
        }

        public ListGenome()
        {

        }


        public ListGenome(long length, object min, object max)
        {
            Length = length;
            TheMin = (int)min;
            TheMax = (int)max;

            for (int i = 0; i < Population.numWorkers; i++)
            {
                int ind = TheSeed.Next(0, Population.numWorkplaces);
                for (int j = 0; j < Population.numWorkplaces; j++)
                {
                    if (j == ind)
                    {
                        int nextValue = (int)GenerateGeneValue(min, max);
                        TheArray.Add(nextValue);

                    } else
                    {
                        TheArray.Add(0);
                    }
                }
            }
        }

        public override void Initialize()
        {

        }

        public override bool CanDie(float fitness)
        {
            if (CurrentFitness <= (int)(fitness * 100.0f))
            {
                return true;
            }

            return false;
        }


        public override bool CanReproduce(float fitness)
        {
            if (ListGenome.TheSeed.Next(100) >= (int)(fitness * 100.0f))
            {
                return true;
            }

            return false;
        }

        public override object GenerateGeneValue(object min, object max)
        {
            return TheSeed.Next((int)min, (int)max);
        }

        public override void Mutate()
        {
            MutationIndex = TheSeed.Next((int)Length);
            int val = (int)GenerateGeneValue(TheMin, TheMax);
            TheArray[MutationIndex] = val;

        }

        // This fitness function calculates the production from the current genome
        private float CalculateProduction()
        {
            int Xij, Rij, Tij, ind;
            CurrentFitness = 0;
            for (int i = 0; i < Population.numWorkplaces; i++)
            {
                for (int j = 0; j < Population.numWorkers; j++)
                {
                    ind = j * Population.numWorkplaces + i;
                    Xij = (int)TheArray[ind];
                    Rij = (int)Population.errorIndex[ind];
                    Tij = (int)Population.timeIndex[ind];

                    int duration = Population.turnDuration;

                    CurrentFitness += (float)Math.Truncate((double)(duration / ((1 + Rij) * Tij * Xij)));
                }
            }

            return CurrentFitness;
        }

        public override float CalculateFitness()
        {
            CurrentFitness = CalculateProduction();
            return CurrentFitness;
        }

        public override string ToString()
        {
            string strResult = "";
            for (int i = 0; i < Length; i++)
            {
                strResult = strResult + ((int)TheArray[i]).ToString() + " ";
            }

            strResult += "-->" + CurrentFitness.ToString();

            return strResult;
        }

        public override void CopyGeneInfo(Genome dest)
        {
            ListGenome theGene = (ListGenome)dest;
            theGene.Length = Length;
            theGene.TheMin = TheMin;
            theGene.TheMax = TheMax;
        }


        public override Genome Crossover(Genome g)
        {
            ListGenome aGene1 = new ListGenome();
            ListGenome aGene2 = new ListGenome();
            g.CopyGeneInfo(aGene1);
            g.CopyGeneInfo(aGene2);


            ListGenome CrossingGene = (ListGenome)g;
            for (int i = 0; i < CrossoverPoint; i++)
            {
                aGene1.TheArray.Add(CrossingGene.TheArray[i]);
                aGene2.TheArray.Add(TheArray[i]);
            }

            for (int j = CrossoverPoint; j < Length; j++)
            {
                aGene1.TheArray.Add(TheArray[j]);
                aGene2.TheArray.Add(CrossingGene.TheArray[j]);
            }

            // 50/50 chance of returning gene1 or gene2
            ListGenome aGene = null;
            if (TheSeed.Next(2) == 1)
            {
                aGene = aGene1;
            }
            else
            {
                aGene = aGene2;
            }

            return aGene;
        }

        public override void initializeArray(Genome array)
        {
            for(int i=0; i < array.Length; i++)
            {
                ((ListGenome)array).TheArray.Add(0);
            }
        }

        public override void Crossover2(ref Genome genome2, out Genome child1, out Genome child2)
        {
            int pos = (int)(TheSeed.NextDouble() * (double)Length);
            child1 = new ListGenome();
            child2 = new ListGenome();
            
            genome2.CopyGeneInfo(child1);
            genome2.CopyGeneInfo(child2);

            initializeArray(child1);
            initializeArray(child2);

            for (int i = 0; i < Length; i++)
            {
                if (i < pos)
                {
                    ((ListGenome)child1).TheArray.Add(TheArray[i]);
                    ((ListGenome)child2).TheArray.Add(((ListGenome)genome2).TheArray[i]);
                }
                else
                {
                    ((ListGenome)child1).TheArray.Add(((ListGenome)genome2).TheArray[i]);
                    ((ListGenome)child2).TheArray.Add(TheArray[i]);
                }
            }
        }

    }
}
