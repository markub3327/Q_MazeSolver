using System;
using System.Collections.Generic;
using System.Threading;

namespace QMazeExample
{
    class Program
    {
        public static readonly int time_max       = 1000000;

        public static void Main()
        {
            Agent a1 = new Agent();
            System.IO.StreamWriter file_train = new System.IO.StreamWriter(@"score_train.txt");
            System.IO.StreamWriter file_test = new System.IO.StreamWriter(@"score_test.txt");

            Prostredie env1 = new Prostredie(new int[][] 
            { 
                new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
                new int[] { 0, 1, 0, 0, 1, 1, 0, 1, 0, 0 },
                new int[] { 0, 2, 0, 0, 0, 1, 2, 1, 0, 0 },
                new int[] { 0, 0, 0, 1, 0, 3, 0, 1, 0, 0 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new int[] { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0 },
                new int[] { 0, 3, 1, 1, 1, 1, 0, 0, 1, 0 },
                new int[] { 0, 1, 0, 2, 0, 1, 0, 0, 1, 0 },
                new int[] { 0, 1, 0, 0, 0, 1, 0, 0, 4, 0 },
            });

            var epsilon = 1.0f;

            // Trening agenta
            for (int time = 0; time < time_max; time++)
            {
                a1.reset(env1);

                for (int i = 0; i < 4; i++)
                    env1.GenerateItem(new Jablko());
        
                for (int i = 0; i < 3; i++)
                    env1.GenerateItem(new Mina());

                float score = 0;
                for (int runningTime = 0; runningTime < 100; runningTime++)
                {
                    //Console.Clear();

                    //env1.Vypis(a1.currentPositionX, a1.currentPositionY);

                    var odmena = a1.AktualizujAgenta(env1, true, epsilon);
                    score += odmena;

                    //Console.WriteLine($"\nTime epoch: {time}/{time_max}, runningTime: {runningTime}");
                    //Console.WriteLine($"Pocet naucenych stavov: {a1.PocetUlozenychStavov}\n");

                    // ukonci hru ak nasiel ciel
                    if (env1.prostredie[a1.currentPositionY][a1.currentPositionX].id == Vychod.Tag)
                        break;
                    else if (env1.prostredie[a1.currentPositionY][a1.currentPositionX].id == Priepast.Tag)
                        break;

                    //Thread.Sleep(10);   // ±100 FPS
                }

                if (epsilon >= 0.01f)
                    epsilon *= 0.9999f;

                if (time % 1000 == 0)
                {
                    Console.WriteLine($"\nepsilon: {epsilon}, epoch: {time}/{time_max}");
                    Console.WriteLine($"Pocet naucenych stavov: {a1.PocetUlozenychStavov}\n");

                    file_train.WriteLine($"{score};{a1.PocetUlozenychStavov}");
                }

                // Vymaz vsetky jablka a miny
                env1.NahradObjekty(Jablko.Tag, new Cesta());
                env1.NahradObjekty(Mina.Tag, new Cesta());
            }

            file_train.WriteLine($"\n");

            // Trening agenta
            for (int time = 0; time < 1000; time++)
            {
                a1.reset(env1);

                for (int i = 0; i < 4; i++)
                    env1.GenerateItem(new Jablko());
        
                for (int i = 0; i < 3; i++)
                    env1.GenerateItem(new Mina());

                float score = 0;
                for (int runningTime = 0; runningTime < 100; runningTime++)
                {
                    Console.Clear();

                    Console.WriteLine("Testovanie agenta\n");

                    env1.Vypis(a1.currentPositionX, a1.currentPositionY);

                    var odmena = a1.AktualizujAgenta(env1, false, 0.0);
                    score += odmena;

                    Console.WriteLine($"reward: {odmena}, score: {score}");

                    // ukonci hru ak nasiel ciel
                    if (env1.prostredie[a1.currentPositionY][a1.currentPositionX].id == Vychod.Tag)
                        break;
                    else if (env1.prostredie[a1.currentPositionY][a1.currentPositionX].id == Priepast.Tag)
                        break;

                    Thread.Sleep(100);
                }

                env1.Vypis(a1.currentPositionX, a1.currentPositionY);

                // Vymaz vsetky jablka a miny
                env1.NahradObjekty(Jablko.Tag, new Cesta());
                env1.NahradObjekty(Mina.Tag, new Cesta());

                //Console.WriteLine($"Time epoch: {time}\n");

                Thread.Sleep(1500);

                file_test.WriteLine($"{score}");
            }

            file_train.Close();
            file_test.Close();
        }
    }        
}
