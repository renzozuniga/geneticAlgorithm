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
        public static void readArrays(System.IO.StreamReader file, int numWorkplaces, int numWorkers, ArrayList array)
        {
            String line;
            for (int i = 0; i < numWorkers; i++)
            {
                line = file.ReadLine();
                String[] data = line.Split(',');
                for (int j = 0; j < numWorkplaces; j++)
                {
                    array.Add(Convert.ToDouble(data[j]));
                }
            }
        }

        public static void readDataInput(int numWorkplaces, ref int numWorkers, ArrayList errorIndex, ArrayList timeIndex)
        {
            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader("data_input.txt");
            numWorkers = Convert.ToInt32(file.ReadLine());
            readArrays(file, numWorkplaces, numWorkers, errorIndex);
            readArrays(file, numWorkplaces, numWorkers, timeIndex);
            file.Close();
        }

        [STAThread]
        static void Main(string[] args)
        {
            int numWorkplaces = 7;
            int turnDuration = 8 * 60; // 8 horas (expresado en minutos)
            int numWorkers=0;
            ArrayList errorIndex = new ArrayList(); // R: indice de rotura
            ArrayList timeIndex = new ArrayList(); // T: tiempo que se demora el trabajador en un puesto de trabajo

            readDataInput(numWorkplaces,ref numWorkers,errorIndex,timeIndex);

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
