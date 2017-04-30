using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace AlgoritmoGeneticoDP1
{
    class Cromosoma
    {
        public float FitnessActual;
        public long Length;
        public ArrayList TheArray;
        private int TheMax;
        private int TheMin;
        public static Random TheSeed = new Random((int)DateTime.Now.Ticks);

        public Cromosoma()
        {
            FitnessActual = 0.0f;
            TheArray = new ArrayList();
            TheMin = 0;
            TheMax = 2;
        }

        //Genera un cromosoma de la población, teniendo en cuenta ciertas restricciones
        public Cromosoma(long length, object min, object max)
        {
            FitnessActual = 0.0f;
            TheArray = new ArrayList();
            Length = length;
            TheMin = (int)min;
            TheMax = (int)max;

            inicializarCromosoma();
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                int vacantes = ((Proceso)Poblacion.procesos[i]).vacantes;
                for (int j = 0; j < vacantes; j++)
                {
                    //Aquí se restringe que un trabajador solo puede estar asignado a un puesto de trabajo
                    //y que el numero de trabajadores asignados a un puesto de trabajo no supere la cantidad
                    //máxima de trabajadores en un puesto de trabajo
                    int trabajador = TheSeed.Next(0, Poblacion.numTrabajadores);
                    if (!trabajadorDisponible(trabajador))
                    {
                        j--;
                    }
                    else
                    {
                        int ind = (trabajador * Poblacion.numPuestosDeTrabajo) + i;
                        TheArray[ind] = 1;
                    }
                }
            }
        }

        //Devuelve el fitness de un cromosoma
        public float CalcularFitness()
        {
            FitnessActual = CalcularProduccion();
            return FitnessActual;
        }

        //Se calcula la función objetivo para un determinado cromosoma 
        private float CalcularProduccion()
        {
            int Xij, ind;
            FitnessActual = 0.0f;

            Trabajador trabajador;
            Proceso proceso;

            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                proceso = (Proceso)Poblacion.procesos[i];
                for (int j = 0; j < Poblacion.numTrabajadores; j++)
                {
                    trabajador = (Trabajador)Poblacion.trabajadores[j];
                    ind = (j * Poblacion.numPuestosDeTrabajo) + i;
                    Xij = (int)this.TheArray[ind];
                    int duracionTurno = Poblacion.duracionTurno;
                    if (Xij > 0)
                    {
                        FitnessActual += (float)Math.Truncate(duracionTurno / (trabajador.CalcularIndiceProceso(proceso) * Xij));
                    }
                }
            }
            return FitnessActual;
        }

        //Se copia la información de un cromosoma a otro
        public void CopiarInformacionCromosoma(Cromosoma dest)
        {
            Cromosoma cromosoma = dest;
            cromosoma.Length = Length;
            cromosoma.TheMin = TheMin;
            cromosoma.TheMax = TheMax;
        }

        //Se copia la información y contenido de un cromosoma a otro
        public void CopiarCromosoma(out Cromosoma dest)
        {
            dest = new Cromosoma();
            dest.Length = Length;
            dest.TheMin = TheMin;
            dest.TheMax = TheMax;
            dest.FitnessActual = FitnessActual;
            for (int i = 0; i < Length; i++)
            {
                dest.TheArray.Add(TheArray[i]);
            }
        }

        //Operador Genético de cruce de tipo uniforme
        public void Cruzar_uniforme(ref Cromosoma cromosoma2, out Cromosoma hijo1, out Cromosoma hijo2)
        {
            hijo1 = new Cromosoma();
            hijo2 = new Cromosoma();
            cromosoma2.CopiarInformacionCromosoma(hijo1);
            cromosoma2.CopiarInformacionCromosoma(hijo2);
            int cont = 0;
            for (int i = 0; i < Poblacion.numTrabajadores; i++)
            {
                for (int j = 0; j < Poblacion.numPuestosDeTrabajo; j++)
                {
                    if ((i % 2) == 0)
                    {
                        hijo1.TheArray.Add(TheArray[cont]);
                        hijo2.TheArray.Add(cromosoma2.TheArray[cont]);
                    }
                    else
                    {
                        hijo2.TheArray.Add(TheArray[cont]);
                        hijo1.TheArray.Add(cromosoma2.TheArray[cont]);
                    }
                    cont++;
                }
            }
        }

        //Operador Genético de cruce de tipo one-point
        public void Cruzar_unpunto(ref Cromosoma cromosoma2, out Cromosoma hijo1, out Cromosoma hijo2)
        {
            int trabajador1 = TheSeed.Next(0, Poblacion.numTrabajadores);
            int trabajador2 = TheSeed.Next(0, Poblacion.numTrabajadores);
            hijo1 = new Cromosoma();
            hijo2 = new Cromosoma();
            CopiarCromosoma(out hijo1);
            cromosoma2.CopiarCromosoma(out hijo2);
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                int ind1 = (trabajador1 * Poblacion.numPuestosDeTrabajo) + i;
                int ind2 = (trabajador2 * Poblacion.numPuestosDeTrabajo) + i;
                int aux = (int)hijo1.TheArray[ind1];
                hijo1.TheArray[ind1] = hijo2.TheArray[ind2];
                hijo2.TheArray[ind2] = aux;
            }
        }

        //Verifica que un cromosoma tenga una estructura valida
        public bool esValido()
        {
            //Arreglo auxiliar de cantidades acumuladas de trabajadores asignados en cada puesto de trabajo
            ArrayList asignaciones = new ArrayList();
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                asignaciones.Add(0);
            }
            int cont = 0;
            for (int i = 0; i < Poblacion.numTrabajadores; i++)
            {
                int cantidadAsignaciones = 0;
                for (int j = 0; j < Poblacion.numPuestosDeTrabajo; j++)
                {
                    cantidadAsignaciones += (int)TheArray[cont];
                    if (((int)TheArray[cont]) == 1)
                    {
                        asignaciones[j] = ((int)asignaciones[j]) + 1;
                    }
                    //Verificación de cantidad acumulada de trabajadores no sobrepase el máximo de trabajadores en un puesto de trabajo
                    if ((int)asignaciones[j] > ((Proceso)Poblacion.procesos[j]).vacantes)
                    {
                        return false;
                    }

                    //Verificación de un trabajador solo puede estar asignado a un puesto de trabajo
                    if (cantidadAsignaciones > 1)
                    {
                        return false;
                    }
                    cont++;
                }

                //Esto indica que un trabajador debe estar siempre asignado a un puesto de trabajo
                /*if (cantidadAsignaciones != 1)
                {
                    return false;
                }*/
                //Hemos comentado esto porque asumimos que el numero de trabajadores es mayor o igual que el numero de vacantes por puestos de trabajo,
                //puede que un trabajador no esté asignado a un puesto de trabajo
            }
            return true;
        }

        private void inicializarCromosoma()
        {
            for (int i = 0; i < Length; i++)
            {
                TheArray.Add(0);
            }
        }

        public void mostrarAsignaciones()
        {
            Console.WriteLine("Número de trabajadores: " + Poblacion.numTrabajadores);
            Console.WriteLine("Cantidad de vacantes por puesto: ");
            for (int i = 0; i < Poblacion.numPuestosDeTrabajo; i++)
            {
                Console.Write(((Proceso)(Poblacion.procesos[i])).nombre + ": ");
                Console.WriteLine(((Proceso)Poblacion.procesos[i]).vacantes);
            }
            for (int j = 0; j < Poblacion.numTrabajadores; j++)
            {
                Console.Write(((Trabajador)(Poblacion.trabajadores[j])).nombre + ": ");
                int suma = 0;
                for (int k = 0; k < Poblacion.numPuestosDeTrabajo; k++)
                {
                    int indice = (j * Poblacion.numPuestosDeTrabajo) + k;
                    suma += (int)TheArray[indice];
                    if (((int)TheArray[indice]) == 1)
                    {
                        Console.WriteLine("Asignado a " + ((Proceso)(Poblacion.procesos[k])).nombre);
                    }
                }
                if (suma == 0)
                {
                    Console.WriteLine("No asignado a ningún puesto de trabajo");
                }
            }
        }

        public void mostrarCromosoma()
        {
            Console.WriteLine(ToString());
        }

        //Operador genético de mutación 
        public void Mutar()
        {
            //Indica el indice de un trabajador elegido aleatoriamente
            int indiceTrabajador = TheSeed.Next(Poblacion.numTrabajadores); 

            //Indica el número de puestos de trabajo
            int numPuestosDeTrabajo = Poblacion.numPuestosDeTrabajo;

            //Indica el indice de un puesto de trabajo del trabajador elegido
            int indice = 0;

            //Indica el indice de un puesto de trabajo del trabajador elegido a mutar
            int indiceMutacion = (numPuestosDeTrabajo * indiceTrabajador) + TheSeed.Next(Poblacion.numPuestosDeTrabajo);

            //Se procede a realizar la mutación cambiando solo una casilla en el rango de un trabajador
            for (int i = 0; i < numPuestosDeTrabajo; i++)
            {
                indice = (numPuestosDeTrabajo * indiceTrabajador) + i;
                if (((int)TheArray[indice]) == 1)
                {
                    TheArray[indice] = 0;
                    break;
                }
            }
            TheArray[indiceMutacion] = 1;
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Length; i++)
            {
                str = str + ((int)TheArray[i]).ToString() + " ";
            }
            return (str + "-->" + FitnessActual.ToString());
        }

        private bool trabajadorDisponible(int trabajador)
        {
            int indice = trabajador * Poblacion.numPuestosDeTrabajo;
            int suma = 0;
            for (int i = indice; i < (indice + Poblacion.numPuestosDeTrabajo); i++)
            {
                suma += (int)TheArray[i];
            }
            if (suma > 0)
            {
                return false;
            }
            return true;
        }

    }
}
