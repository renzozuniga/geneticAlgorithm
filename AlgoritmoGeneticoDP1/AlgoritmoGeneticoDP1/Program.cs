using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace AlgoritmoGeneticoDP1
{
    class Program
    {
        // Methods
        [STAThread]
        private static void Main(string[] args)
        {
            int numPuestosdeTrabajo = 0;
            int td = 480;
            int numTrabajadores = 0;
            ArrayList indicesError = new ArrayList();
            ArrayList indicesTiempo = new ArrayList();
            ArrayList vacantes = new ArrayList();
            readDataInput(ref numPuestosdeTrabajo, ref numTrabajadores, indicesError, indicesTiempo, vacantes);
            Poblacion poblacion = new Poblacion(numTrabajadores, numPuestosdeTrabajo, td, vacantes, indicesError, indicesTiempo);
            Cromosoma cromosoma = poblacion.obtenerMejorCromosoma();
            Cromosoma cromosoma2 = new Cromosoma();
            int num4 = 1;
            int num5 = 0;
            int num6 = 0;
            Console.WriteLine("Generaci\x00f3n " + num4);
            cromosoma.mostrarCromosoma();
            while ((num6 < 0x3e8) && (num5 < 5))
            {
                poblacion.SiguienteGeneracion();
                Cromosoma cromosoma3 = poblacion.obtenerMejorCromosoma();
                if (cromosoma3.FitnessActual > cromosoma.FitnessActual)
                {
                    cromosoma = cromosoma3;
                }
                if (Math.Truncate((double)cromosoma3.FitnessActual) == Math.Truncate((double)cromosoma2.FitnessActual))
                {
                    num5++;
                }
                else
                {
                    num5 = 0;
                }
                num6++;
                num4++;
                cromosoma2 = cromosoma3;
                cromosoma3.mostrarCromosoma();
                Console.WriteLine("Generaci\x00f3n " + num4);
            }
            cromosoma.mostrarCromosoma();
            cromosoma.mostrarAsignaciones();
            Console.WriteLine(cromosoma.esValido());
            Console.ReadLine();
        }

        public static void readArrays(StreamReader file, int numWorkplaces, int numWorkers, ArrayList array)
        {
            for (int i = 0; i < numWorkers; i++)
            {
                char[] separator = new char[] { ',' };
                string[] strArray = file.ReadLine().Split(separator);
                for (int j = 0; j < numWorkplaces; j++)
                {
                    array.Add(Convert.ToDouble(strArray[j]));
                }
            }
        }

        public static void readDataInput(ref int numPuestosdeTrabajo, ref int numTrabajadores, ArrayList indicesError, ArrayList indicesTiempo, ArrayList vacantes)
        {
            StreamReader reader = new StreamReader("prueba.csv");
            char[] separator = new char[] { ',' };
            string[] strArray = reader.ReadLine().Split(separator);
            numTrabajadores = Convert.ToInt32(strArray[1]);
            char[] chArray2 = new char[] { ',' };
            strArray = reader.ReadLine().Split(chArray2);
            numPuestosdeTrabajo = Convert.ToInt32(strArray[1]);
            char[] chArray3 = new char[] { ',' };
            strArray = reader.ReadLine().Split(chArray3);
            for (int i = 0; i < numPuestosdeTrabajo; i++)
            {
                vacantes.Add(Convert.ToInt32(strArray[i + 1]));
            }
            string str = reader.ReadLine();
            for (int j = 0; j < numTrabajadores; j++)
            {
                char[] chArray4 = new char[] { ',' };
                strArray = reader.ReadLine().Split(chArray4);
                for (int m = 0; m < numPuestosdeTrabajo; m++)
                {
                    indicesError.Add(Convert.ToDouble(strArray[m + 1]));
                }
            }
            str = reader.ReadLine();
            for (int k = 0; k < numTrabajadores; k++)
            {
                char[] chArray5 = new char[] { ',' };
                strArray = reader.ReadLine().Split(chArray5);
                for (int n = 0; n < numPuestosdeTrabajo; n++)
                {
                    indicesTiempo.Add(Convert.ToInt32(strArray[n + 1]));
                }
            }
            reader.Close();
        }

    }
}
