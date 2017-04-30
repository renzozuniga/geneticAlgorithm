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
            for(int i=0; i<numPuestosDeTrabajo; i++)
            {
                int value = TheSeed.Next(1, 5);
                vacantes.Add(value); 
                numVacantes += (int)value;
            }

            int numTrabajadores = numVacantes + TheSeed.Next(0, 3);

            //before your loop
            var csv = new StringBuilder();

            //in your loop
            var first = "Número de Trabajadores";
            var second = numTrabajadores;
            //Suggestion made by KyleMit
            var newLine = string.Format("{0},{1}", first, second);
            csv.AppendLine(newLine);

            first = "Número de Puestos de Trabajo";
            second = numPuestosDeTrabajo;
            //Suggestion made by KyleMit
            newLine = string.Format("{0},{1}", first, second);
            csv.AppendLine(newLine);

            string cadena = "Vacantes,";
            for(int i=0; i < numPuestosDeTrabajo; i++)
            {
                cadena += vacantes[i] + ",";
            }
            csv.AppendLine(cadena);

            string cabeceraPuestos = "Indice Error,";
            for(int i=0; i < numPuestosDeTrabajo; i++)
            {
                cabeceraPuestos += "Puesto " + (i + 1) + ",";
            }
            csv.AppendLine(cabeceraPuestos);

            for (int i = 0; i < numTrabajadores; i++){
                string indErrorTrab = "Trabajador " + (i+1) + ",";
                for (int j=0; j < numPuestosDeTrabajo; j++)
                {
                    double valor = (double)TheSeed.NextDouble();
                    indErrorTrab += valor.ToString("0.0") + ",";
                }
                csv.AppendLine(indErrorTrab);
            }
            cabeceraPuestos = "Indice Tiempo,";
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
                    int valor = TheSeed.Next(30,100);
                    indTiempoTrab += valor.ToString() + ",";
                }
                csv.AppendLine(indTiempoTrab);
            }
            //after your loop
            File.WriteAllText("./prueba.csv", csv.ToString());

        }

        

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Exportar();
        }


    }
}
