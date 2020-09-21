using System;
using System.Collections.Generic;
using System.Threading;

namespace QMazeExample
{
    class Program
    {
        private static System.IO.StreamWriter log_file = new System.IO.StreamWriter(@"statistics.txt");

        public static void Main()
        {
            Prostredie env1 = new Prostredie(new int[][] 
            { 
                new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new int[] { 0, 1, 0, 0, 0, 1, 0, 1, 0, 1 },
                new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0 },
                new int[] { 1, 0, 0, 1, 0, 1, 0, 1, 0, 1 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new int[] { 0, 1, 0, 1, 0, 1, 0, 0, 1, 0 },
                new int[] { 0, 1, 1, 1, 1, 1, 0, 0, 1, 0 },
                new int[] { 0, 1, 0, 1, 0, 1, 1, 0, 1, 0 },
                new int[] { 0, 1, 1, 1, 0, 1, 0, 0, 4, 0 }
            });

            Agent a1 = new Agent();

            // Faze ucenia
            run(env1, a1, 1000000, training: true);

            // Faze testovania
            run(env1, a1, 1000, training: false);

            log_file.Close();
        }

        private static void run(Prostredie env, Agent a, int episodes=1000, int steps=100, bool training=true)
        {
            var epsilon = training == true ? 1.0f : 0.0f;
            float score;
            int is_end;

            // Trening agenta
            for (int episode = 0, step; episode < episodes; episode++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                a.reset(env, testing: !training);

                is_end = 0;
                score = 0;

                for (step = 0; step < steps; step++)
                {
                    //if (training == false)
                    //{
                    //    Console.Clear();
                    //    env.Vypis(a.currentPositionX, a.currentPositionY);
                    //}

                    var odmena = a.AktualizujAgenta(env, training, epsilon);
                    score += odmena;

                    //Console.WriteLine($"\nTime epoch: {time}/{time_max}, runningTime: {runningTime}");
                    //Console.WriteLine($"Pocet naucenych stavov: {a1.PocetUlozenychStavov}\n");

                    // ukonci hru ak nasiel ciel
                    if (env.prostredie[a.currentPositionY][a.currentPositionX].id == Vychod.Tag)
                    {
                        is_end = 100;
                        break;
                    }

                    //Thread.Sleep(10);   // ±100 FPS
                }

                watch.Stop();

                if (epsilon >= 0.01f)
                    epsilon *= 0.999995f;
                
                if ((episode % 100000) == 0)
                {
                    Console.WriteLine($"\nepsilon: {epsilon}, epoch: {episode}/{episodes}");
                    Console.WriteLine($"Pocet naucenych stavov: {a.PocetUlozenychStavov}\n");
                    Console.WriteLine($"apples: {a.apples}/{a.apple_count}, mines: {a.mines}/{a.mine_count}");
                }

                // log only testing phase
                if (training == false)
                {
                    log_file.WriteLine($"{episode};{score};{step};{watch.Elapsed.TotalMilliseconds * 1000};{(a.apples/(float)a.apple_count)*100.0f};{(a.mines/(float)a.mine_count)*100.0f};{is_end}");
                }
            }
        }
    }        
}