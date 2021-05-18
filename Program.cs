using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace QMazeExample
{
    class Program
    {
        private static StreamWriter log_file = new StreamWriter(@"statistics.txt");
        
        // Parametre prostredia
        private static int episodes_train = 5000;
        private static int episodes_test = 20;
        private static int max_steps = 100;
        private static float epsilon_decay = 0.999f;
        
        // Definicia bludiska
        private static Prostredie env1 = new Prostredie(new int[][] 
        { 
            new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
            new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            new int[] { 0, 1, 0, 0, 0, 1, 0, 1, 0, 1 },
            new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 1, 0, 0, 1, 0, 1, 0, 1, 0, 1 },
            new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            new int[] { 0, 1, 0, 1, 0, 1, 0, 0, 1, 0 },
            new int[] { 0, 1, 1, 1, 1, 1, 0, 0, 1, 0 },
            new int[] { 0, 1, 0, 1, 0, 1, 1, 1, 1, 0 },
            new int[] { 0, 1, 1, 1, 0, 1, 0, 0, 4, 0 }
        });

        public static void Main()
        {
            Statistics curr_stat = null;
            Statistics best_stat = null;

            Agent a1 = new Agent();

            // Algoritmus Hill climbing
            /*for (int iter = 0; iter < 10000;)
            {                
                // sprav n merani pre jednu konfiguraciu
                curr_stat = null;
                for (int repeat = 0; repeat < 15; repeat++)
                {
                    // vymaz agentovu vedomost
                    a1.clearMem();

                    // Faza ucenia
                    run(env1, a1, 5000, training: true);

                    // Faza testovania
                    var stat = run(env1, a1, 20, training: false);

                    if (curr_stat == null)
                    {
                        curr_stat = stat;
                    }
                    else
                    {
                        curr_stat.append(stat);
                    }
                }
                //Console.WriteLine($"{curr_stat.apples.Count};{curr_stat.mines.Count};{curr_stat.ends.Count}");

                if (best_stat != null)
                {
                    var apple_t = curr_stat.apples_mean;
                    var apple_b = best_stat.apples_mean; 
                    
                    var mine_t = curr_stat.mines_mean; 
                    var mine_b = best_stat.mines_mean; 
                    
                    var end_t = curr_stat.ends_mean; 
                    var end_b = best_stat.ends_mean;

                    // vypis najdenych parametrov a statistiky
                    if (iter % 1 == 0)
                    {
                        Console.WriteLine($"Time: {iter}");
                        Console.WriteLine($"curr_apples: {apple_t}%\tcurr_mines: {mine_t}%\tcurr_ends: {end_t}%");
                        Console.WriteLine($"best_apples: {apple_b}%\tbest_mines: {mine_b}%\tbest_ends: {end_b}%");
                        env1.VypisParam();
                        Console.WriteLine();
                    }
                    log_file.WriteLine($"{iter};{apple_b};{mine_b};{end_b}");

                    // ak nasiel lepsie riesenie uloz ho
                    if (((mine_t - mine_b) < -1f && (apple_t - apple_b) >= -5f && (end_t - end_b) >= -5f) 
                        || ((apple_t - apple_b) > 1f && (mine_t - mine_b) <= 5f && (end_t - end_b) >= -5f) 
                        || ((end_t - end_b) > 1f && (mine_t - mine_b) <= 5f && (apple_t - apple_b) >= -5f))
                    {
                        // uloz si zlepseny stav
                        best_stat = curr_stat;

                        // uloz najlepsie parametre
                        env1.saveRewards();
                    }
                    // ak sa stagnuje alebo sa zhorsil v jednej z kategorii zapocitaj mu iteraciu 
                    //   (v pripade spravnych krokov ma nekonecne vela casu)
                    else if (mine_t >= mine_b || apple_t <= apple_b || end_t <= end_b)
                    {
                        iter++;
                    }
                }
                else
                {
                    // uloz si prvy stav
                    best_stat = curr_stat;

                    // uloz najlepsie parametre
                    env1.saveRewards();
                
                    iter++;
                }

                // vygeneruj novy nahodny parameter
                env1.randomizeRewards();                
            }

            env1.VypisParam();
            Console.WriteLine();*/

            // vymaz agentovu vedomost
            a1.clearMem();

            // Faza ucenia
            run(env1, a1, episodes_train, max_steps, training: true);

            // Faza testovania
            run(env1, a1, episodes_test, max_steps, training: false);

            log_file.Close();
        }

        private static Statistics run(Prostredie env, Agent a, int episodes, int steps, bool training=true)
        {
            Statistics stat = new Statistics();
            var epsilon = training == true ? 1.0f : 0.0f;
            float score;
            int is_end;

            // Trening agenta
            for (int episode = 0, step; episode < episodes; episode++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                a.reset(env, episode, training);

                is_end = 0;
                score = 0;

                for (step = 0; step < steps; step++)
                {
                    if (training == false)
                    {
                        Console.Clear();
                        env.Vypis(a.currentPos.x, a.currentPos.y);
                        Thread.Sleep(200);
                    }

                    var isValid = a.AktualizujAgenta(env, training, epsilon, out float odmena);
                    score += odmena;

                    // ukonci hru ak nasiel ciel
                    if (env.prostredie[a.currentPos.y][a.currentPos.x].id == Vychod.Tag)
                    {
                        is_end = 100;
                        break;
                    }
                    else if (isValid == false)
                    {
                        break;
                    }
                }

                watch.Stop();

                if (epsilon >= 0.01f)
                    epsilon *= epsilon_decay;
                
                /*if ((episode % 1000) == 0)
                {
                    Console.WriteLine($"\nepsilon: {epsilon}, epoch: {episode}/{episodes}");
                    Console.WriteLine($"Pocet naucenych stavov: {a.PocetUlozenychStavov}\n");
                    Console.WriteLine($"apples: {a.apples}/{a.apple_count}, mines: {a.mines}/{a.mine_count}");
                }*/

                // log only testing phase
                if (training == false)
                {
                    var apple = (a.apples/(float)a.apple_count) * 100.0f;
                    var mine = (a.mines/(float)a.mine_count) * 100.0f;

                    log_file.WriteLine($"{episode};{score};{step};{watch.Elapsed.TotalMilliseconds * 1000};{apple};{mine};{is_end}");
                    //Console.WriteLine($"{apple};{mine};{is_end}");

                    stat.append(apple, mine, is_end);
                }
            }

            return stat;
        }
    }

    class Statistics
    {
        public List<float> apples = new List<float>();
        public List<float> mines = new List<float>();
        public List<float> ends = new List<float>();

        public float apples_mean { get { return MathF.Round(this.mean(this.apples)); } }
        public float mines_mean { get { return MathF.Round(this.mean(this.mines)); } }
        public float ends_mean { get { return MathF.Round(this.mean(this.ends)); } }

        public void append(float apple, float mine, float end)
        {
            this.apples.Add(apple);
            this.mines.Add(mine);
            this.ends.Add(end);
        }

        public void append(Statistics stat)
        {
            foreach (var a in stat.apples)
            {
                this.apples.Add(a);
            }

            foreach (var m in stat.mines)
            {
                this.mines.Add(m);
            }

            foreach (var e in stat.ends)
            {
                this.ends.Add(e);
            }
        }

        private float mean(List<float> list)
        {
            float avg = 0f;

            foreach (var val in list)
            {
                avg += val;
            }
            avg /= (float)list.Count;

            return avg;
        }
    }        
}
