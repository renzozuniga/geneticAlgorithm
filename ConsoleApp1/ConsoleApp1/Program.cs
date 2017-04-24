using System;

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
        [STAThread]
        static void Main(string[] args)
        {
            int n = 3;
            Population TestPopulation = new Population(n);
            TestPopulation.WriteNextGeneration();

            for (int i = 0; i < 1000; i++)
            {
                TestPopulation.NextGeneration();
                TestPopulation.WriteNextGeneration();
            }

            Console.ReadLine();
            //
            // TODO: Add code to start application here
            //
        }
    }
}
