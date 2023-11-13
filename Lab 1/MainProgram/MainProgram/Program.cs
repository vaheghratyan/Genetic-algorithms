using System;
using System.Collections.Generic;
using System.Linq;

namespace MainProgram
{
    class Program
    {
        public int countOfIndividualsInGeneration; //Количество особей в поколении
        public int countOfVertex; //Количество вершин 
        public int countOfEdges = 0; //Количество ребер
        public string[] edges; //Ребра
        public float[,] adjacencyMatrix; //Матрица смежности
        public int[,] solution;    //2-номер особи, 3 - используемое ребро (1 - используется, 0 - не используется)
        public int numberOfCurrentGeneration; //Номер текущего поколения
        float[] currentGeneration; //Текущее поколение
        float[] sortedCurrentGeneration; //Отсортированное текущее поколение
        public int[,] individuals; //Особи
        public float graphWeight = 0; //Вес графа
        int[] vertexLabels; //Метки вершин
        float[,] matrixForCyclesSearch; //Матрица для поиска циклов
        float previousOF; //Предыдущее значение оценочной функции
        int countOfTimesWithoutChangingOF; //Количество раз без изменения оценочной функции
        Random rand = new Random();
        static void Main(string[] args)
        {
            Program program = new Program();


            Console.WriteLine("Введите количество вершин");
            program.countOfVertex = Convert.ToInt32(Console.ReadLine());
            program.adjacencyMatrix = new float[program.countOfVertex, program.countOfVertex];

            Console.WriteLine("Введите матрицу смежности графа");
            for (int i = 0; i < program.countOfVertex; i++)                      //Создание матрицы смежности графа, расчет его веса и количества ребер
            {
                for (int j = 0; j < program.countOfVertex; j++)
                {
                    if (i < j)
                    {
                        Console.WriteLine("Введите значение матрицы " + i + " " + j);
                        program.adjacencyMatrix[i, j] = (float)Convert.ToDouble(Console.ReadLine());
                        program.adjacencyMatrix[j, i] = program.adjacencyMatrix[i, j];

                        program.graphWeight = program.graphWeight + program.adjacencyMatrix[i, j];
                        if (program.adjacencyMatrix[i, j] != 0)
                        {
                            program.countOfEdges++;
                        }
                    }
                }
            }
            program.countOfIndividualsInGeneration = (int)Math.Sqrt(program.countOfEdges) * 2;
            program.currentGeneration = new float[program.countOfIndividualsInGeneration];
            program.sortedCurrentGeneration = new float[program.countOfIndividualsInGeneration];
            program.solution = new int[program.countOfIndividualsInGeneration, program.countOfEdges];
            program.edges = new string[program.countOfEdges];
            program.individuals = new int[program.countOfIndividualsInGeneration, program.countOfEdges];
            program.vertexLabels = new int[program.countOfVertex];
            int count = 0;
            for (int i = 0; i < program.countOfVertex; i++)               //Создание массива ребер
            {
                for (int j = 0; j < program.countOfVertex; j++)
                {
                    if (i < j)
                    {
                        if (program.adjacencyMatrix[i, j] != 0)
                        {
                            program.edges[count] = i.ToString() + "&" + j.ToString();
                            count++;
                        }
                    }
                }
            }

            for (int i = 0; i < program.countOfVertex; i++)           //Вывод матрицы смежности
            {
                for (int j = 0; j < program.countOfVertex; j++)
                {
                    Console.Write(program.adjacencyMatrix[i, j] + " ");
                }
                Console.Write("\n");
            }

            program.previousOF = 100;
            program.countOfTimesWithoutChangingOF = 0;
            int exit = (int)((program.countOfEdges * 2) / (Math.Sqrt(program.countOfEdges)));
            program.creatingAPopulation();
            for (int counter = 0; counter < 9999; counter++)
            {
                for (int i = 0; i < program.countOfIndividualsInGeneration; i++)
                {
                    program.currentGeneration[i] = program.evaluationFunction(i);
                    program.sortedCurrentGeneration[i] = program.currentGeneration[i];
                }
                Array.Sort(program.sortedCurrentGeneration);
                for (int i = 0; i < program.countOfIndividualsInGeneration; i++)
                {
                    for (int j = 0; j < program.countOfIndividualsInGeneration; j++)
                    {
                        if (program.currentGeneration[i] == program.sortedCurrentGeneration[j])
                        {
                            for (int k = 0; k < program.countOfEdges; k++)
                            {
                                program.individuals[j, k] = program.solution[i, k];
                            }
                        }
                    }
                }

                for (int i = 0; i < program.countOfIndividualsInGeneration; i++)
                {
                    for (int k = 0; k < program.countOfEdges; k++)
                    {
                        program.solution[i, k] = program.individuals[i, k];
                    }
                }

                int[] newIndividual = new int[program.countOfEdges];
                for (int i = 0; i < (program.countOfIndividualsInGeneration * 0.5); i++)
                {
                    newIndividual = program.crossing();
                    for (int k = 0; k < program.countOfEdges; k++)
                    {
                        program.individuals[program.countOfIndividualsInGeneration - 1 - i, k] = newIndividual[k];
                    }
                }
                for (int i = 0; i < (program.countOfIndividualsInGeneration * 0.3); i++)
                {
                    newIndividual = program.mutation();
                    for (int k = 0; k < program.countOfEdges; k++)
                    {
                        program.individuals[i + Convert.ToInt32(program.countOfIndividualsInGeneration * 0.2), k] = newIndividual[k];                 //Две лучшие особи оставляем без изменений
                    }
                }
                for (int i = 0; i < (program.countOfIndividualsInGeneration * 0.2); i++)
                {
                    for (int k = 0; k < program.countOfEdges; k++)
                    {
                        program.individuals[i, k] = program.solution[i, k];
                    }
                }

                program.numberOfCurrentGeneration++;

                for (int i = 0; i < program.countOfIndividualsInGeneration; i++)
                {
                    for (int k = 0; k < program.countOfEdges; k++)
                    {
                        program.solution[i, k] = program.individuals[i, k];
                    }
                }
                Console.WriteLine("Поколение " + program.numberOfCurrentGeneration + ": " + program.sortedCurrentGeneration[0]);
                Console.WriteLine();

                if (program.previousOF == program.sortedCurrentGeneration[0])
                {
                    program.countOfTimesWithoutChangingOF++;
                }
                else
                {
                    program.countOfTimesWithoutChangingOF = 0;
                }
                program.previousOF = program.sortedCurrentGeneration[0];

                if (program.countOfTimesWithoutChangingOF == exit)
                {
                    int[] genome = new int[program.countOfEdges];
                    for (int i = 0; i < program.countOfEdges; i++)
                    {
                        genome[i] = program.solution[0, i];
                    }
                    Console.WriteLine("Матрица смежности остова:");
                    program.convertFromGenomeToAdjacencyMatrix(genome);
                    Console.ReadLine();
                    break;
                }
            }

            Console.ReadLine();
        }

        public void creatingAPopulation() //Создание популяции
        {
            List<int> numbersOfVertex = new List<int>();
            int numberOfEdge;
            int vertex1;
            int vertex2;
            for (int count = 0; count < countOfIndividualsInGeneration; count++)
            {
                for (int i = 0; i < countOfVertex; i++)
                {
                    numbersOfVertex.Add(i);
                }
                while (numbersOfVertex.Count > 0)
                {
                    numberOfEdge = rand.Next(countOfEdges);
                    vertex1 = Convert.ToInt32(edges[numberOfEdge].Substring(0, edges[numberOfEdge].IndexOf("&")));
                    vertex2 = Convert.ToInt32(edges[numberOfEdge].Substring(edges[numberOfEdge].IndexOf("&") + 1));
                    if ((numbersOfVertex.Contains(vertex1) == true) || (numbersOfVertex.Contains(vertex2) == true))
                    {
                        if (((numbersOfVertex.Contains(vertex1) == true) && (numbersOfVertex.Contains(vertex2) == true)) && (numbersOfVertex.Count != countOfVertex))
                        {
                            continue;
                        }
                        numbersOfVertex.Remove(vertex1);
                        numbersOfVertex.Remove(vertex2);
                        solution[count, numberOfEdge] = 1;
                    }
                }
            }
        }

        public float evaluationFunction(int numberOfIndividual)                 //Вычисление оценочной функции (чем меньше, тем лучше)
        {
            float ostovWeight = 0;
            int vertex1;
            int vertex2;
            for (int i = 0; i < countOfEdges; i++)
            {
                if (solution[numberOfIndividual, i] != 0)
                {
                    vertex1 = Convert.ToInt32(edges[i].Substring(0, edges[i].IndexOf("&")));
                    vertex2 = Convert.ToInt32(edges[i].Substring(edges[i].IndexOf("&") + 1));
                    ostovWeight = ostovWeight + adjacencyMatrix[vertex1, vertex2];
                }

            }

            return (ostovWeight / graphWeight) * 100;
        }

        public int selection() //Селекция
        {
            float roulette = 0;
            float result = 0;
            for (int i = 0; i < countOfIndividualsInGeneration; i++)
            {
                roulette = roulette + sortedCurrentGeneration[i];
            }
            result = rand.Next((int)roulette);
            result = (int)(roulette - result);
            roulette = 0;
            for (int i = 0; i < countOfIndividualsInGeneration; i++)
            {
                roulette = roulette + sortedCurrentGeneration[i];
                if (roulette >= result)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool checkingValidityOfSolution(int[] genome) //Проверка допустимости решения
        {
            int vertex1;
            int vertex2;
            int counter = 0;
            matrixForCyclesSearch = new float[countOfVertex, countOfVertex];
            float[,] buffer = new float[countOfVertex, countOfVertex];
            for (int i = 0; i < countOfEdges; i++)
            {
                if (genome[i] == 1)
                {
                    counter++;
                }
            }
            if (counter != countOfVertex - 1)
            {
                return false;
            }
            for (int i = 0; i < countOfVertex; i++)
            {
                for (int j = 0; j < countOfVertex; j++)
                {
                    matrixForCyclesSearch[i, j] = 0;
                    buffer[i, j] = 0;
                }
            }
            for (int i = 0; i < countOfEdges; i++)
            {
                if (genome[i] == 1)
                {
                    vertex1 = Convert.ToInt32(edges[i].Substring(0, edges[i].IndexOf("&")));
                    vertex2 = Convert.ToInt32(edges[i].Substring(edges[i].IndexOf("&") + 1));
                    buffer[vertex1, vertex2] = adjacencyMatrix[vertex1, vertex2];
                    buffer[vertex2, vertex1] = adjacencyMatrix[vertex2, vertex1];
                }
            }
            for (int i = 0; i < countOfVertex; i++)
            {
                for (int k = 0; k < countOfVertex; k++)
                {
                    for (int j = 0; j < countOfVertex; j++)
                    {
                        matrixForCyclesSearch[k, j] = buffer[k, j];
                    }
                }
                for (int m = 0; m < countOfVertex; m++)
                {
                    vertexLabels[m] = 0;
                }
                if (searchCyclesInGraph(i) == true)
                {
                    return false;
                }
            }
            return true;
        }

        public bool searchCyclesInGraph(int v) //Поиск циклов в графе
        {
            if (vertexLabels[v] == 3)
            {
                return true;
            }
            else
            {
                vertexLabels[v] = 1;
                for (int i = 0; i < countOfVertex; i++)
                {
                    if ((matrixForCyclesSearch[v, i] > 0) && (vertexLabels[i] == 1))
                    {
                        vertexLabels[i] = 3;
                        return true;
                    }
                    else if (matrixForCyclesSearch[v, i] > 0)
                    {
                        matrixForCyclesSearch[i, v] = 0;
                        matrixForCyclesSearch[v, i] = 0;
                        searchCyclesInGraph(i);
                    }
                }
                vertexLabels[v] = 2;
                for (int i = 0; i < countOfVertex; i++)
                {
                    if (vertexLabels[i] == 3)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public int[] crossing() //Скрещивание
        {

            int[] genomeOfIndividual1 = new int[countOfEdges];
            int[] genomeOfIndividual2 = new int[countOfEdges];
            int[] childGenome = new int[countOfEdges];

            do
            {
                int individual1 = selection();
                int individual2 = selection();
                while (individual1 == individual2)
                {
                    individual2 = selection();
                }

                for (int i = 0; i < countOfIndividualsInGeneration; i++)             //Извлечение геномов селектированных особей
                {
                    if (sortedCurrentGeneration[individual1] == currentGeneration[i])
                    {
                        for (int k = 0; k < countOfEdges; k++)
                        {
                            genomeOfIndividual1[k] = solution[i, k];
                        }
                    }
                    if (sortedCurrentGeneration[individual2] == currentGeneration[i])
                    {
                        for (int k = 0; k < countOfEdges; k++)
                        {
                            genomeOfIndividual2[k] = solution[i, k];
                        }
                    }
                }


                int boundary1 = rand.Next(countOfEdges - 1);
                int boundary2 = rand.Next(1, countOfEdges);
                while (boundary1 >= boundary2)
                {
                    boundary2 = rand.Next(1, countOfEdges);
                }
                int[] mask = new int[boundary2 - boundary1];
                for (int i = 0; i < (boundary2 - boundary1); i++)
                {
                    mask[i] = rand.Next(1, 3);
                }
                for (int i = 0; i < boundary1; i++)
                {
                    childGenome[i] = genomeOfIndividual1[i];
                }
                int count = boundary2 - boundary1 - 1;
                for (int i = boundary2 - 1; i > boundary1 + 1; i--)
                {
                    if (mask[count] == 1)
                    {
                        childGenome[i] = genomeOfIndividual1[i];
                    }
                    else
                    {
                        childGenome[i] = genomeOfIndividual2[i];
                    }
                    count--;
                }
                for (int i = boundary2; i < childGenome.Length; i++)
                {
                    childGenome[i] = genomeOfIndividual2[i];
                }

                if ((Enumerable.SequenceEqual(childGenome, genomeOfIndividual1)) || (Enumerable.SequenceEqual(childGenome, genomeOfIndividual2)))
                {
                    continue;
                }

            } while (checkingValidityOfSolution(childGenome) == false);
            return childGenome;
        }

        public int[] mutation() //Мутация
        {
            int individual = selection();
            int[] individualGenome = new int[countOfEdges];
            int[] mutatedIndividualGenome = new int[countOfEdges];
            for (int i = 0; i < countOfIndividualsInGeneration; i++)             //Извлечение генома селектированной особи
            {
                if (sortedCurrentGeneration[individual] == currentGeneration[i])
                {
                    for (int k = 0; k < countOfEdges; k++)
                    {
                        individualGenome[k] = solution[i, k];
                        mutatedIndividualGenome[k] = individualGenome[k];
                    }
                }
            }

            do
            {
                int boundary1 = rand.Next(countOfEdges - 2);
                int boundary2 = rand.Next(2, countOfEdges);
                while ((boundary2 - boundary1) < 2)
                {
                    boundary2 = rand.Next(2, countOfEdges);
                }

                int checkingDifferenceOfElements = 0;
                for (int i = boundary1; i < boundary2; i++)
                {
                    checkingDifferenceOfElements = checkingDifferenceOfElements + individualGenome[i];
                }
                if ((checkingDifferenceOfElements == 0) || (checkingDifferenceOfElements == (boundary2 - boundary1)))
                {
                    continue;
                }

                for (int i = boundary2 - 1; i >= boundary1; i--)
                {
                    int j = rand.Next(boundary1, i + 1);

                    int tmp = mutatedIndividualGenome[j];
                    mutatedIndividualGenome[j] = mutatedIndividualGenome[i];
                    mutatedIndividualGenome[i] = tmp;
                }
                bool isMixingSuccessfull = false;
                for (int i = 0; i < countOfEdges; i++)
                {
                    if (individualGenome[i] != mutatedIndividualGenome[i])
                    {
                        isMixingSuccessfull = true;
                    }
                }
                if (isMixingSuccessfull == false)
                {
                    continue;
                }

            } while (checkingValidityOfSolution(mutatedIndividualGenome) == false);
            return mutatedIndividualGenome;


        }

        public void convertFromGenomeToAdjacencyMatrix(int[] genome) //Преобразование генома в матрицу смежности
        {
            float[,] tempMatrix = new float[countOfVertex, countOfVertex];
            for (int i = 0; i < countOfEdges; i++)
            {
                if (genome[i] == 1)
                {
                    int vertex1 = Convert.ToInt32(edges[i].Substring(0, edges[i].IndexOf("&")));
                    int vertex2 = Convert.ToInt32(edges[i].Substring(edges[i].IndexOf("&") + 1));

                    tempMatrix[vertex1, vertex2] = adjacencyMatrix[vertex1, vertex2];
                    tempMatrix[vertex2, vertex1] = adjacencyMatrix[vertex2, vertex1];
                }
            }
            for (int i = 0; i < countOfVertex; i++)
            {
                for (int j = 0; j < countOfVertex; j++)
                {
                    Console.Write(tempMatrix[i, j] + " ");
                }
                Console.Write("\n");
            }
        }

    }
}
