﻿using System;
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

        public static void leerDataEntrada(ArrayList trabajadores, ArrayList procesos, ref int duracionTurno)
        {
            int numPuestosdeTrabajo = 0;    //Indica el numero de puestos de trabajo
            int numTrabajadores = 0;        //Indica el numero de trabajadores en un dia de trabajo
            StreamReader file = new StreamReader("data_input.csv");

            //Leer cabeceras principales
            file.ReadLine();

            //Lectura de numero de trabajadores
            //Lectura de numero de puestos de trabajo
            //Lectura de alfa
            //Lectura de duracion de turno 
            string[] line = file.ReadLine().Split(',');
            numTrabajadores = Convert.ToInt32(line[0]);
            numPuestosdeTrabajo = Convert.ToInt32(line[1]);
            double alfa = Convert.ToDouble(line[2]);   //No se usa alfa en algoritmo genético
            duracionTurno = Convert.ToInt32(line[3]);

            file.ReadLine();

            // Creando los trabajadores
            for (int i = 0; i < numTrabajadores; i++)
            {
                Trabajador trabajador = new Trabajador(i,"Trabajador "+(i+1));
                trabajadores.Add(trabajador);
            }

            //Lectura de cabecera indices de rotura
            string cabeceraIndices = file.ReadLine();
            //Lectura indices de rotura de cada trabajador
            for (int i = 0; i < numTrabajadores; i++)
            {
                line = file.ReadLine().Split(',');
                for (int j = 0; j < numPuestosdeTrabajo; j++)
                {
                    double rotura = Convert.ToDouble(line[j + 1]);
                    ((Trabajador)trabajadores[i]).indicesRotura.Add(rotura);
                }
            }

            file.ReadLine();

            //Lectura de cabecera indices de tiempo
            cabeceraIndices = file.ReadLine();
            //Lectura indices de tiempo de cada trabajador
            for (int i = 0; i < numTrabajadores; i++)
            {
                line = file.ReadLine().Split(',');
                for (int j = 0; j < numPuestosdeTrabajo; j++)
                {
                    int tiempo = Convert.ToInt32(line[j + 1]);
                    ((Trabajador)trabajadores[i]).indicesTiempo.Add(tiempo);
                }
            }

            file.ReadLine();

            //lectura de vacantes
            line = file.ReadLine().Split(',');
            for (int i = 0; i < numPuestosdeTrabajo; i++)
            {
                //Indica las vacantes maximas de trabajadores en cada puesto de trabajo
                int vacantes = Convert.ToInt32(line[i + 1]);
                Proceso proceso = new Proceso(i, vacantes,"Proceso "+(i+1));
                procesos.Add(proceso);
            }
            file.Close();
        }

        [STAThread]
        private static void Main(string[] args)
        {
            int duracionTurno = 0;        //Indica la duracion total de un dia de trabajo en minutos
            ArrayList trabajadores = new ArrayList();   
            ArrayList procesos = new ArrayList();

            //Se procede a leer la data inicial desde un archivo .csv
            leerDataEntrada(trabajadores, procesos, ref duracionTurno);
            //Se genera la población inicial 
            Poblacion poblacion = new Poblacion(trabajadores, procesos, duracionTurno);

            Cromosoma mejorCromosoma = poblacion.obtenerMejorCromosoma();   //Se obtiene el mejor cromosoma de la poblacion inicial
            Cromosoma ultimoCromosoma = new Cromosoma();

            int generacion = 1;                 //Indica el numero de la generacion
            int repetido = 0;                   //Indica la cantidad de veces que el fitness del mejor cromosoma no cambia
            int i = 0;

            Console.WriteLine("Generación " + generacion);
            mejorCromosoma.mostrarCromosoma();

            while ((i < 1000) && (repetido < 5))        //Condicion de parada del algoritmo genetico, ya sea 1000 generaciones o 
            {                                           //que el mejor cromosoma sea el mismo por 5 generaciones seguidas

                poblacion.SiguienteGeneracion();        //Se procede a generar la siguiente generacion 

                //Se obtiene el mejor cromosoma de la actual generacion
                Cromosoma nuevoCromosoma = poblacion.obtenerMejorCromosoma();   

                //Se compara si el mejor cromosoma de la generacion actual es mejor que el mejor cromosoma historico
                if (nuevoCromosoma.FitnessActual > mejorCromosoma.FitnessActual)
                {
                    mejorCromosoma = nuevoCromosoma;
                }

                if (Math.Truncate(nuevoCromosoma.FitnessActual) == Math.Truncate(ultimoCromosoma.FitnessActual))
                {
                    //Si el mejor cromosoma de la generacion actual es igual al mejor cromosoma de la generacion anterior
                    repetido++;
                }
                else
                {
                    //Si el mejor cromosoma de la generacion actual no es igual al mejor cromosoma de la generacion anterior
                    repetido = 0;
                }
                i++;
                generacion++;

                //Se guarda el mejor cromosoma actual para tenerlo en cuenta en la siguiente generacion
                ultimoCromosoma = nuevoCromosoma;  
                nuevoCromosoma.mostrarCromosoma();
                Console.WriteLine("Generación " + generacion);
            }
            mejorCromosoma.mostrarCromosoma();
            mejorCromosoma.mostrarAsignaciones();   //Se muestran las asignaciones correspondientes
            Console.WriteLine(mejorCromosoma.esValido());
            Console.ReadLine();
        }

        

    }
}