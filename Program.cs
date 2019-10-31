using System;
using System.Collections.Generic;
using System.Threading;

namespace QMazeExample
{
    class Program
    {
        public static readonly int startPositionX = 0;  // Zacina v bode [0,1]
        public static readonly int startPositionY = 1;  // Zacina v bode [0,1]

        public static void Main()
        {
            Agent a1 = new Agent();

            Prostredie env1 = new Prostredie(new int[][] 
            { 
                new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
                new int[] { 0, 1, 0, 0, 1, 1, 0, 0, 0, 1 },
                new int[] { 0, 2, 0, 0, 0, 1, 2, 0, 0, 1 },
                new int[] { 0, 0, 0, 1, 0, 3, 0, 0, 0, 1 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new int[] { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0 },
                new int[] { 0, 3, 1, 1, 1, 1, 0, 0, 1, 0 },
                new int[] { 0, 1, 0, 2, 0, 1, 0, 0, 1, 0 },
                new int[] { 0, 1, 0, 0, 0, 1, 0, 0, 4, 0 },
            });

            // Trening agenta
            for (int time = 0; time < 10000; time++)
            {
                // Test agenta
                a1.currentPositionX = startPositionX; 
                a1.currentPositionY = startPositionY;

                int runningTime = 0;
                while (a1.currentPositionY != 9 || a1.currentPositionX != 8)
                {
                    // Mnou dany cas obnovovania jablcok a min
                    if ((runningTime % 200) == 0)   // 200 * 15ms = ±3s
                    {
                        if (env1.pocetJablk < 4)
                            env1.GenerateItem(new Jablko());
        
                        if (env1.pocetMin < 3)
                            env1.GenerateItem(new Mina());
                    }

                    Console.Clear();

                    env1.Vypis(a1.currentPositionX, a1.currentPositionY);

                    a1.AktualizujAgenta(env1, true);

                    Console.WriteLine($"\nTime epoch: {time}/500, runningTime: {runningTime}");
                    Console.WriteLine($"Pocet naucenych stavov: {a1.PocetUlozenychStavov}\n");

                    runningTime++;

                    Thread.Sleep(15);
                }
            }

            for (;;)
            {
                // Test agenta
                a1.currentPositionX = startPositionX; 
                a1.currentPositionY = startPositionY;

                int runningTime = 0;
                while (a1.currentPositionY != 9 || a1.currentPositionX != 8)
                {
                    // Mnou dany cas obnovovania jablcok a min
                    if ((runningTime % 30) == 0)   // 30 * 100ms = ±3s
                    {
                        if (env1.pocetJablk < 4)
                            env1.GenerateItem(new Jablko());
        
                        if (env1.pocetMin < 3)
                            env1.GenerateItem(new Mina());
                    }

                    Console.Clear();

                    Console.WriteLine("Testovanie agenta\n");

                    env1.Vypis(a1.currentPositionX, a1.currentPositionY);

                    a1.AktualizujAgenta(env1, false);

                    runningTime++;

                    Thread.Sleep(100);
                }

                env1.Vypis(a1.currentPositionX, a1.currentPositionY);

                //Console.WriteLine($"Time epoch: {time}\n");

                Thread.Sleep(1500);
            }
        }
    }        
}
