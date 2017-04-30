using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoGeneticoDP1
{
    class Proceso
    {
        public int id;
        public int vacantes;

        public Proceso(int i, int vac)
        {
            id = i;
            vacantes = vac;
        }

       /* public void asignarTrabajador(Trabajador trab)
        {
            asignacionTrabajadores[trab.id] = 1;
            ++trabajadoresAsignados;
        }*/

        public void Imprimir()
        {
            Console.WriteLine("PROCESO: {0}", id);
            Console.WriteLine("VACANTES: {0}", vacantes);
        }
    }
}
