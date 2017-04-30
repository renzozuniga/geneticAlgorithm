using System;
using System.Collections;

namespace AlgoritmoGenetico
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

        public static void readDataInput(ref int numPuestosdeTrabajo, ref int numTrabajadores, ArrayList indicesError, ArrayList indicesTiempo, ArrayList vacantes)
        {
            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader("prueba.csv");

            //Lectura de numero de trabajadores
            string line = file.ReadLine();
            string[] values = line.Split(',');
            numTrabajadores = Convert.ToInt32(values[1]);

            //Lectura de numero de puestos de trabajo
            line = file.ReadLine();
            values = line.Split(',');
            numPuestosdeTrabajo = Convert.ToInt32(values[1]);

            //Lectura de vacantes
            line = file.ReadLine();
            values = line.Split(',');
            for(int i = 0; i < numPuestosdeTrabajo; i++)
            {
                vacantes.Add(Convert.ToInt32(values[i + 1]));
            }

            line = file.ReadLine();  //cabecera indice Error

            //Lectura de indices de error de trabajadores
            for (int i=0; i<numTrabajadores; i++)
            {
                line = file.ReadLine();
                values = line.Split(',');
                for(int j = 0; j < numPuestosdeTrabajo; j++)
                {
                    indicesError.Add(Convert.ToDouble(values[j+1]));
                }
            }

            line = file.ReadLine();  //cabecera indice Tiempos

            //Lectura de indices de tiempos de trabajadores
            for (int i = 0; i < numTrabajadores; i++)
            {
                line = file.ReadLine();
                values = line.Split(',');
                for (int j = 0; j < numPuestosdeTrabajo; j++)
                {
                    indicesTiempo.Add(Convert.ToInt32(values[j+1]));
                }
            }
            file.Close();
        }

        [STAThread]
        static void Main(string[] args)
        {
            int numPuestosdeTrabajo = 0;
            int duracionTurno = 8 * 60; // 8 horas (expresado en minutos)
            int numTrabajadores = 0;

            ArrayList indicesError = new ArrayList(); // R: indice de rotura
            ArrayList indicesTiempo = new ArrayList(); // T: tiempo que se demora el trabajador en un puesto de trabajo
            ArrayList vacantes = new ArrayList(); // Vacantes por puesto de trabajo

            readDataInput(ref numPuestosdeTrabajo, ref numTrabajadores, indicesError, indicesTiempo, vacantes);

            Poblacion TestPopulation = new Poblacion(numTrabajadores, numPuestosdeTrabajo, duracionTurno, vacantes, indicesError, indicesTiempo);
            Cromosoma mejorCromosoma = TestPopulation.obtenerMejorCromosoma();
            Cromosoma ultimoCromosoma = new Cromosoma();

            int generacion = 1;
            int repetido = 0;
            int i = 0;

            Console.WriteLine("Generación " + generacion);
            mejorCromosoma.mostrarCromosoma();

            while( i < 1000 && repetido < 5)
            {

                TestPopulation.SiguienteGeneracion();
                Cromosoma nuevoCromosoma = TestPopulation.obtenerMejorCromosoma();
                if (nuevoCromosoma.FitnessActual > mejorCromosoma.FitnessActual)
                {
                    mejorCromosoma = nuevoCromosoma;
                }

                if (Math.Truncate(nuevoCromosoma.FitnessActual) == Math.Truncate(ultimoCromosoma.FitnessActual))
                {
                    repetido++;
                } else
                {
                    repetido = 0;
                }
                i++;
                generacion++;
                ultimoCromosoma = nuevoCromosoma;
                nuevoCromosoma.mostrarCromosoma();
                Console.WriteLine("Generación " + generacion);
            }
            mejorCromosoma.mostrarCromosoma();
            mejorCromosoma.mostrarAsignaciones();
            Console.WriteLine(mejorCromosoma.esValido());

            Console.ReadLine();
        }
    }
}
