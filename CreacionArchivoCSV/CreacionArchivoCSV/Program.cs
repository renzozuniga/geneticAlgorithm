using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreacionArchivoCSV
{ 
    class Program
    {
        protected void Exportar()
        {
            ArrayList vacantes = new ArrayList();
            Random TheSeed = new Random((int)DateTime.Now.Ticks);
            int numPuestosDeTrabajo = 7;
            int numVacantes = 0;
            double alfa = 0.3;
            int tiempoTotalTurno = 500;
            for(int i=0; i<numPuestosDeTrabajo; i++)
            {
                int value = TheSeed.Next(1, 5);
                vacantes.Add(value); 
                numVacantes += (int)value;
            }

            int numTrabajadores = numVacantes + TheSeed.Next(0, 15);

            //before your loop
            var csv = new StringBuilder();

            string cabeceraPrincipal = "Trabajadores,Procesos,Alfa,Tiempo Total del Turno";
            csv.AppendLine(cabeceraPrincipal);
            
            var primero = numTrabajadores;
            var segundo = numPuestosDeTrabajo;
            var tercero = alfa;
            var cuarto = tiempoTotalTurno;
            var newLine = string.Format("{0},{1},{2},{3}", primero, segundo, tercero, cuarto);
            csv.AppendLine(newLine);

            csv.AppendLine("");

            string cabeceraPuestos = "Rotura,";
            for (int i = 0; i < numPuestosDeTrabajo; i++)
            {
                cabeceraPuestos += "Puesto " + (i + 1) + ",";
            }
            csv.AppendLine(cabeceraPuestos);

            for (int i = 0; i < numTrabajadores; i++)
            {
                string indErrorTrab = "Trabajador " + (i + 1) + ",";
                for (int j = 0; j < numPuestosDeTrabajo; j++)
                {
                    double valor = TheSeed.NextDouble();
                    indErrorTrab += valor.ToString("0.00") + ",";
                }
                csv.AppendLine(indErrorTrab);
            }

            csv.AppendLine("");

            cabeceraPuestos = "Tiempo,";
            for (int i = 0; i < numPuestosDeTrabajo; i++)
            {
                cabeceraPuestos += "Puesto " + (i + 1) + ",";
            }
            csv.AppendLine(cabeceraPuestos);

            for (int i = 0; i < numTrabajadores; i++)
            {
                string indTiempoTrab = "Trabajador " + (i + 1) + ",";
                for (int j = 0; j < numPuestosDeTrabajo; j++)
                {
                    int valor = TheSeed.Next(30, 100);
                    indTiempoTrab += valor.ToString() + ",";
                }
                csv.AppendLine(indTiempoTrab);
            }

            csv.AppendLine("");

            string cadena = "Vacantes,";
            for(int i=0; i < numPuestosDeTrabajo; i++)
            {
                cadena += vacantes[i] + ",";
            }
            csv.AppendLine(cadena);

            File.WriteAllText("./data_input.csv", csv.ToString(), Encoding.UTF8);

        }

        

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Exportar();
        }


    }
}
