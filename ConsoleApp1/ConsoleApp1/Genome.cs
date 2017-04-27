using System;
using System.Collections;

namespace ConsoleApp1

{
    /// <summary>
    /// Summary description for Genome.
    /// </summary>
    public abstract class Genome : IComparable
    {
        public long Length;
        public int CrossoverPoint;
        public int MutationIndex;
        public float CurrentFitness = 0.0f;

        abstract public void Initialize();
        abstract public void Mutate();
        abstract public Genome Crossover(Genome g);
        abstract public object GenerateGeneValue(object min, object max);
        abstract public void SetCrossoverPoint(int crossoverPoint);
        abstract public float CalculateFitness();
        abstract public bool CanReproduce(float fitness);
        abstract public bool CanDie(float fitness);
        abstract public string ToString();
        abstract public void initializeArray(Genome g);
        abstract public void CopyGeneInfo(Genome g);
        abstract public void CopyGene(Genome g);
        abstract public void Crossover2(ref Genome genome2, out Genome child1, out Genome child2);


        abstract public int CompareTo(object a);

    }
}
