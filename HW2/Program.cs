using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW2
{
    public class Task
    {
        public int id;         // identificador para esta tarea
        public int duration;   // duracion en segundos
        public int priority;   // prioridad de la tarea

        public Task(int id, int duration, int priority)
        {
            this.id = id;
            this.duration = duration;
            this.priority = priority;
        }
        
    }

    // Esta clase define una tarea agendada.  Tu solucion debe devover una lista de
    // objetos de esta clase
    public class ScheduledTask
    {
        public Task task;       // referencia al objeto tarea agendado
        public int start_time;  // hora de inicio para esa tarea

        public ScheduledTask(Task task, int start_time)
        {
            this.task = task;
            this.start_time = start_time;
        }
    }


    // Esta clase abstracta es para facilitar mi trabajo de corregir este homework.
    // Tu solucion debe sobreescribir el metodo Solve en la clase MySoltution
    public abstract class Solution
    {
        // Este es el metodo que debes implementar
        // Elige 'K' de las tareas en 'tasks' y asignales una hora de inicio a cada
        // tarea, de manera que minimice la penalidad descrita en el enunciado.
        // Devuelve el listado L de las K tareas agendadas, donde L[i].start_time
        // es la hora de inicio de la tarea L[i].task
        public abstract List<ScheduledTask> Solve(Task[] tasks, int K);
    }


    // Esta clase es donde debe implementar tu solucion basada en Brute Force
    public class MyBruteForceSolution : Solution
    {
        // TODO: Agrega los atributos y metodos que necesites
        List<Task[]> _tasksList = new List<Task[]>();
        List<ScheduledTask> _scheduledTasks = new List<ScheduledTask>();


        public void GenerateTaskPermutation(List<Task[]> permutation, int permuteIndex, int k, Task[] lastPermutation, Task[] tasks)
        {
            if (permuteIndex >= k)
            {
                permutation.Add(lastPermutation.ToArray());
                //TODO: Create method calculating penalty And remove task array with bigger costs
                if (permutation.Count == 2)
                {
                    CalculatePenalty(permutation);
                }
            }
            else
            {
                for (int i = permuteIndex; i < tasks.Length; i++)
                {
                    lastPermutation[permuteIndex] = tasks[i];
                    Swap(ref tasks[i], ref tasks[permuteIndex]);
                    GenerateTaskPermutation(permutation, permuteIndex+1, k, lastPermutation, tasks);
                    Swap(ref tasks[i], ref  tasks[permuteIndex]);
                }
            }
            
        }
        //TODO: Finish Penalty Calculation and remove the Task with the biggest penalty
        private void CalculatePenalty(List<Task[]> permutation)
        {
            long penalty1 = 0;
            long penalty2 = 0;

            foreach (Task tasArray1 in permutation[0])
            {
                long taskResult = tasArray1.priority * tasArray1.duration;
                penalty1 += taskResult;
            }

            foreach (Task tasArray2 in permutation[1])
            {
                long taskResult = tasArray2.priority * tasArray2.duration;
                penalty2 += taskResult;
            }

            if (penalty1 > penalty2)
            {
                permutation.RemoveAt(0);
            }

            if (penalty2 > penalty1)
            {
                permutation.RemoveAt(1);
            }

            if (penalty2 == penalty1)
            {
                permutation.RemoveAt(1);
            }
        }

        private static void Swap<T>(ref T v1, ref T v2)
        {
            T old = v1;
            v1 = v2;
            v2 = old;
        }


        public override List<ScheduledTask> Solve(Task[] tasks, int K)
        {
            // TODO: Implementar algoritmo con Brute Force (recursivo) para resolver
            //       el problema descrito en el enunciado
            // NOTA: tu algoritmo debe usar Brute Force (no Greedy) para elegir las
            //       K tareas a ejecutar
            // Complejidad esperada: exponencial en N
            // Valor: 9 puntos
            
            //All BruteForce is going to be moved to GenerateTaskPermutation given that is easier to handle pruning
            //this way (at least for me), by generating certain amount of permutations, comparing them
            //then discarding the permutations with the biggest costs, leaving in the end one answer
            Task[] tempTasks = new Task[K];
            GenerateTaskPermutation(_tasksList, 0, K, tempTasks, tasks);
            int counter = 0;
            int startTimeValue = 0;
            while (counter < _tasksList[0].Length)
            {
                _scheduledTasks.Add(new ScheduledTask(_tasksList[0][counter], startTimeValue));
                startTimeValue += _tasksList[0][counter].duration;
                counter++;
            }

            return _scheduledTasks;
        }

        private void CalculateFinalPenalty(List<Task[]> tasksList)
        {
            long penalty = 0;
            foreach (Task[] arrayTask in tasksList)
            {
                foreach (Task  item in arrayTask)
                {
                    long tempResult = item.priority * item.duration;
                    penalty += tempResult;
                    Console.WriteLine(item.id);
                }
            }

            Console.WriteLine(penalty);
        }
    }


    // Esta clase es donde debe implementar tu solucion basada en Greedy
    public class MyGreedySolution : Solution
    {
        // TODO: Agrega los atributos y metodos que necesites
        List<KeyValuePair<Task, float>> _selectedTasks = new List<KeyValuePair<Task, float>>();
        List<ScheduledTask> _finalScheduledTasks = new List<ScheduledTask>();

        public override List<ScheduledTask> Solve(Task[] tasks, int K)
        {
            // TODO: Implementar algoritmo Greedy para resolver el problema descrito
            //       en el enunciado.  La idea es aplicar dos veces greedy:
            // 1) Para seleccionar las K tareas: elige las tareas con menor producto
            //    priority * duration
            // 2) Para ordenar esas K tareas por la razon (priority / duration) de
            //    manera descendente, tal como explicamos en clase
            // Complejidad esperada: O(N log N)
            // Valor: 6 puntos
            List<Task> tasksToSelect = new List<Task>(tasks);
            SelectSmallerTasks(tasksToSelect, K);
            _selectedTasks.Sort((a, b) => b.Value.CompareTo(a.Value));

            int counter = 0;
            int startTimeValue = 0;
            while (counter < _selectedTasks.Count)
            {
                _finalScheduledTasks.Add(new ScheduledTask(_selectedTasks[counter].Key, startTimeValue));
                startTimeValue += _selectedTasks[counter].Key.duration;
                counter++;
            }
            
            return _finalScheduledTasks;
        }

        private void SelectSmallerTasks(List<Task> tasks, int k)
        {
            int counter = 0;
            while (counter < k)
            {
                Task tempTask = tasks[0];
                float tempReason = (float)tempTask.priority / (float)tempTask.duration;

                for (int i = 1; i < tasks.Count; i++)
                {
                    if ((tasks[i].priority * tasks[i].duration) < (tempTask.priority * tempTask.duration))
                    {
                        tempTask = tasks[i];
                        float priority = tasks[i].priority;
                        float duration = tasks[i].duration;
                        tempReason = priority / duration;

                    }
                    
                }

                foreach (var item in _selectedTasks)
                {
                    if (item.Value.Equals(tempReason))
                    {
                        tempReason += (float)0.001;
                        break;
                    }
                }

                _selectedTasks.Add(new KeyValuePair<Task, float>(tempTask,tempReason));
                tasks.Remove(tempTask);
                counter++;
            }

        }
    }


    public class HW2s
    {
        public static void Main(string[] args)
        {
            Task[] tasks = {
            new Task( 1, 200, 10),
            new Task( 2, 120, 9),
            new Task( 3, 240, 12),
            new Task( 4, 150, 12),
            new Task( 5, 125, 20),
            new Task( 6, 225, 18),
            new Task( 7, 200, 15),
            new Task( 8, 190, 16),
            new Task( 9, 180, 14),
            new Task(10, 150, 21),
            new Task(11, 100,  7)
        };
            int K = 7;

            {
                Console.WriteLine("-------");
                Console.WriteLine("Greedy:");
                Console.WriteLine("-------");
                Solution sol = new MyGreedySolution();
                List<ScheduledTask> result = sol.Solve((Task[])tasks.Clone(), K);
                long penalty = 0;
                foreach (ScheduledTask s in result)
                {
                    int end_time = s.start_time + s.task.duration;
                    penalty += s.task.priority * end_time;
                    Console.WriteLine(
                        "Task id {0,2}  priority: {1,2}  duration: {2,3}  " +
                        "start: {3,4}  end: {4,4}  penalty: {5,5}",
                        s.task.id, s.task.priority, s.task.duration,
                        s.start_time, end_time, s.task.priority * end_time
                    );
                }
                Console.WriteLine("Total penalty: {0}", penalty);
                Console.WriteLine();
            }

            {
                Console.WriteLine("------------");
                Console.WriteLine("Brute Force:");
                Console.WriteLine("------------");
                Solution sol = new MyBruteForceSolution();
                List<ScheduledTask> result = sol.Solve((Task[])tasks.Clone(), K);
                long penalty = 0;
                foreach (ScheduledTask s in result)
                {
                    int end_time = s.start_time + s.task.duration;
                    penalty += s.task.priority * end_time;
                    Console.WriteLine(
                        "Task id {0,2}  priority: {1,2}  duration: {2,3}  " +
                        "start: {3,4}  end: {4,4}  penalty: {5,5}",
                        s.task.id, s.task.priority, s.task.duration,
                        s.start_time, end_time, s.task.priority * end_time
                    );
                }
                Console.WriteLine("Total penalty: {0}", penalty);
                Console.WriteLine();
            }
        }

        /*
        Output de mi solucion:

        -------
        Greedy:
        -------
        Task id  5  priority: 20  duration: 125  start:    0  end:  125  penalty:  2500
        Task id  4  priority: 12  duration: 150  start:  125  end:  275  penalty:  3300
        Task id  9  priority: 14  duration: 180  start:  275  end:  455  penalty:  6370
        Task id  2  priority:  9  duration: 120  start:  455  end:  575  penalty:  5175
        Task id 11  priority:  7  duration: 100  start:  575  end:  675  penalty:  4725
        Task id  3  priority: 12  duration: 240  start:  675  end:  915  penalty: 10980
        Task id  1  priority: 10  duration: 200  start:  915  end: 1115  penalty: 11150
        Total penalty: 44200

        ------------
        Brute Force:
        ------------
        Task id  5  priority: 20  duration: 125  start:    0  end:  125  penalty:  2500
        Task id  4  priority: 12  duration: 150  start:  125  end:  275  penalty:  3300
        Task id  9  priority: 14  duration: 180  start:  275  end:  455  penalty:  6370
        Task id  2  priority:  9  duration: 120  start:  455  end:  575  penalty:  5175
        Task id 11  priority:  7  duration: 100  start:  575  end:  675  penalty:  4725
        Task id  1  priority: 10  duration: 200  start:  675  end:  875  penalty:  8750
        Task id  3  priority: 12  duration: 240  start:  875  end: 1115  penalty: 13380
        Total penalty: 44200

        */

    }
}
