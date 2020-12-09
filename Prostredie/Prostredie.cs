
using System;

namespace QMazeExample
{ 
    public class Prostredie
    {
        // trenovacie body
        public static readonly int[] startPositionX_training = { 0, 9, 0 };
        public static readonly int[] startPositionY_training = { 1, 1, 5 };

        // testovacie body na overenie generalizacie agenta
        //public static readonly int[] startPositionX_testing = { 9, 6, 3 };
        //public static readonly int[] startPositionY_testing = { 4, 8, 0 };

        public int Height { get { return prostredie.Length; } }
        public int Width { get { return prostredie[0].Length; } }
        public EnvItem[][] prostredie { get; private set; }
        Random r = new Random((int)DateTime.Now.Ticks);

        public Prostredie(int[][] prostredie)
        {
            this.prostredie = new EnvItem[prostredie.Length][];
            for (int y = 0; y < prostredie.Length; y++)
            {
                this.prostredie[y] = new EnvItem[prostredie[y].Length];
                for (int x = 0; x < prostredie[y].Length; x++)
                {
                    if (prostredie[y][x] == Jablko.Tag)
                        this.prostredie[y][x] = new Jablko();
                    else if (prostredie[y][x] == Mina.Tag)
                        this.prostredie[y][x] = new Mina();
                    else if (prostredie[y][x] == Cesta.Tag)
                        this.prostredie[y][x] = new Cesta();
                    else if (prostredie[y][x] == Priepast.Tag)
                        this.prostredie[y][x] = new Priepast();
                    else if (prostredie[y][x] == Vychod.Tag)
                        this.prostredie[y][x] = new Vychod();                    
                }
            }
        }

        public void GenerateItem(EnvItem item)
        {
            int x, y;

            do {
                x = r.Next(1, Width-1);
                y = r.Next(1, Height-1);
            } while (prostredie[y][x].id != Cesta.Tag || (x == 8 && y >= 5) || (y == 1) || (y == 5));

            // prepis novou polozkou sveta
            prostredie[y][x] = item;
        }

        public float[] rewards = new float[] { -0.55f, +0.80f, -0.85f, -0.15f };
        private float[] rewards_best;

        // Author: Arduino.cc (https://www.arduino.cc/reference/en/language/functions/math/map/)
        private float map(float x, float in_min, float in_max, float out_min, float out_max) 
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public void saveRewards()
        {
            this.rewards_best = new float[this.rewards.Length];

            for (int i = 0; i < this.rewards.Length; i++)
            {
                this.rewards_best[i] = this.rewards[i];
            }
        }

        public void randomizeRewards()
        {
            // deepcopy
            if (this.rewards_best != null)
            {
                for (int i = 0; i < this.rewards.Length; i++)
                {
                    this.rewards[i] = this.rewards_best[i];
                }
            }

            // mutate
            var idx = r.Next(0, this.rewards.Length);
            switch (idx)
            {
                        case 0:
                            this.rewards[idx] = map((float)r.NextDouble(), 0f, 1f, -1f, -0.5f);
                            break;
                        case 2:
                            this.rewards[idx] = map((float)r.NextDouble(), 0f, 1f, -1f, 0f);
                            break;
                        case 3:
                            this.rewards[idx] = map((float)r.NextDouble(), 0f, 1f, -1f, 0f);
                            break;
                        default:
                            this.rewards[idx] = (float)r.NextDouble();
                            break;
            }
        }

        public float Hodnotenie(int x, int y)
        {
            if (prostredie[y][x].id == Priepast.Tag)
                return this.rewards[0];   // Smrt
            else if (prostredie[y][x].id == Jablko.Tag)
                return this.rewards[1];   // Jablcko (odmena)
            else if (prostredie[y][x].id == Mina.Tag)
                return this.rewards[2];   // Mina (trest)
            else if (prostredie[y][x].id == Vychod.Tag)
                return +1.00f;            // nasiel ciel
            else
                return this.rewards[3];   // Najkratsia cesta k vychodu
        }

        public void NahradObjekty(int tag, EnvItem item)
        {
            for (int y = 0; y < prostredie.Length; y++)
                for (int x = 0; x < prostredie[y].Length; x++)
                    if (prostredie[y][x].id == tag)
                        prostredie[y][x] = item;
        }
    
        public void Vypis(int agentX, int agentY)
        {
            Console.WriteLine(new String('-', prostredie[0].Length * 4 + 1));
            for (int y = 0; y < prostredie.Length; y++)
            {
                for (int x = 0; x < prostredie[y].Length; x++)
                {
                    Console.Write("|");
                    
                    if (agentX == x && agentY == y)
                    {
                        // Vykresli agenta
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;                        
                        Console.Write(" A ");
                    }
                    else if (Math.Abs(agentX - x) <= 1 && Math.Abs(agentY - y) <= 1)         
                    {              
                        Console.Write($" {prostredie[y][x].ToString(light: true)} ");
                    }
                    else
                    {
                        Console.Write($" {prostredie[y][x].ToString(light: false)} ");
                    }
                    Console.ResetColor();
                }   
                Console.WriteLine("|");
                Console.WriteLine(new String('-', prostredie[y].Length * 4 + 1));
            }
        }

        public void VypisParam()
        {
            for (int i = 0; i < this.rewards_best.Length; i++)
                Console.Write($"r_best[{i}]: {this.rewards_best[i]}\t");
            Console.WriteLine();

            for (int i = 0; i < this.rewards.Length; i++)
                Console.Write($"r[{i}]: {this.rewards[i]}\t");
            Console.WriteLine();
        }
    }
}
