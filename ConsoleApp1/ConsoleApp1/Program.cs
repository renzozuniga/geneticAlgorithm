using System;
using System.Collections;

namespace ConsoleApp1 
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class Class1
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        [STAThread]
        static void Main(string[] args)
        {
            int numWorkers = 3;
            int turnDuration = 8 * 60; // 8 horas (expresado en minutos)

            ArrayList errorIndex = new ArrayList(); // R: indice de rotura
            ArrayList timeIndex = new ArrayList(); // T: tiempo que se demora el trabajador en un puesto de trabajo

            Console.WriteLine("Ingrese el número de trabajadores");
            int numWorkplaces = Console.Read();

            Console.WriteLine("Ingrese los indices de rotura de los trabajadores ("+numWorkers*numWorkplaces+")");
            int r, t;

            for (int i = 0; i< numWorkers * numWorkplaces; i++)
            {
                r = Console.Read();
                errorIndex.Add(r);
            }

            Console.WriteLine("Ingrese los indices de tiempo de los trabajadores (" + numWorkers * numWorkplaces + ")");

            for (int i = 0; i < numWorkers * numWorkplaces; i++)
            {
                t = Console.Read();
                timeIndex.Add(t);
            }

            Population TestPopulation = new Population(numWorkers, numWorkplaces, turnDuration, errorIndex, timeIndex);
            TestPopulation.WriteNextGeneration();

            for (int i = 0; i < 1000; i++)
            {
                TestPopulation.NextGeneration();
                TestPopulation.WriteNextGeneration();
            }

            Console.ReadLine();
        }
    }
}
