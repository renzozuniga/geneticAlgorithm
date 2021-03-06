﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoGeneticoDP1
{
    class Trabajador
    {
        public int id = -1;
        public string nombre;
        public ArrayList indicesRotura = new ArrayList(); // entre 0 y 1
        public ArrayList indicesTiempo = new ArrayList(); //en minutos

        public Trabajador(int id,string nombre)
        {
            this.id = id;
            this.nombre = nombre;
        }

        public double CalcularIndiceProceso(Proceso proc)
        {
            return (1 + (double)indicesRotura[proc.id]) * (int)indicesTiempo[proc.id];
        }

        public void Imprimir()
        {
            Console.WriteLine("TRABAJADOR: {0}", id);

            Console.Write("Roturas: ");
            for (int i = 0; i < indicesRotura.Count; ++i)
            {
                Console.Write("  {0}", indicesRotura[i]);
            }
            Console.WriteLine();

            Console.Write("Tiempos: ");
            for (int i = 0; i < indicesTiempo.Count; ++i)
            {
                Console.Write("  {0}", indicesTiempo[i]);
            }
            Console.WriteLine();
        }
    }
}
