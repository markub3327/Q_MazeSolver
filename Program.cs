using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace AI
{
    class QState
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int[] stateRadar { get; set; }

        public override string ToString()
        {
            return $"statePosition: X:{PositionX}, Y:{PositionY}\n" +
                   $"stateRadar: {stateRadar[0]},{stateRadar[1]},{stateRadar[2]},{stateRadar[3]},{stateRadar[4]},{stateRadar[5]},{stateRadar[6]},{stateRadar[7]}";
        }
    }

    class QStateEqualityComparer : IEqualityComparer<QState>
    {
        public bool Equals([AllowNull] QState x, [AllowNull] QState y)
        {
            // Poloha agentov sa nezhoduje
            if ((x.PositionX != y.PositionX) || (x.PositionY != y.PositionY))
                return false;

            // Prehladaj pole radaru pri nezhode vrat false z funkcieß
            for (int i = 0; i < 8; i++)
            {
                if (x.stateRadar[i] != y.stateRadar[i])
                    return false;                    
            }

            // ziadne predchadzajuce return nezafungovalo nasiel zhodu
            return true;
        }

        public int GetHashCode([DisallowNull] QState obj)
        {
            return ((obj.ToString()).GetHashCode());
        }
    }

    class Program
    {
        static bool error = false;

        static void ResetEnv(Prostredie env)
        {
            env.prostredie = new int[][] 
            { 
                new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
                new int[] { 0, 1, 0, 0, 1, 1, 0, 0, 0, 1 },
                new int[] { 0, 2, 0, 0, 0, 1, 2, 0, 0, 1 },
                new int[] { 0, 0, 0, 1, 0, 4, 0, 0, 0, 1 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new int[] { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0 },
                new int[] { 0, 4, 1, 1, 1, 1, 0, 0, 1, 0 },
                new int[] { 0, 1, 0, 2, 0, 1, 0, 0, 1, 0 },
                new int[] { 0, 1, 0, 0, 0, 1, 0, 0, 3, 0 },
            };
        }

        public static void Main()
        {
            Agent a1 = new Agent();

            Prostredie env1 = new Prostredie { agent = a1 };

            ResetEnv(env1);

            // Trening agenta
            for (int time = 0; time < 500; time++)
            {
                // Test agenta
                a1.currentPositionX = Agent.startPositionX; 
                a1.currentPositionY = Agent.startPositionY;

                int runningTime = 0;
                while (a1.currentPositionY != 9 || a1.currentPositionX != 8)
                {
                    // Mnou dany cas obnovovania jablcok
                    if ((runningTime % 200) == 0)   // 200 * 15ms = ±3s
                        ResetEnv(env1);

                    Console.Clear();

                    env1.Vypis();

                    a1.AktualizujAgenta(env1, true);

                    Console.WriteLine($"\nTime epoch: {time}/500, runningTime: {runningTime}");
                    Console.WriteLine($"Pocet naucenych stavov: {a1.PocetUlozenychStavov}\n");

                    runningTime++;

                    Thread.Sleep(15);
                }
            }

            for (int time = 0; /*time < 10*/; time++)
            {
                // Test agenta
                a1.currentPositionX = Agent.startPositionX; 
                a1.currentPositionY = Agent.startPositionY;

                ResetEnv(env1);

                while (a1.currentPositionY != 9 || a1.currentPositionX != 8)
                {
                    Console.Clear();

                    Console.WriteLine("Testovanie agenta\n");

                    env1.Vypis();

                    a1.AktualizujAgenta(env1, false);

                    Thread.Sleep(250);
                }

                env1.Vypis();

                Console.WriteLine($"Time epoch: {time}\n");

                Thread.Sleep(500);
            }
        }
    }

    class Prostredie
    {
        public enum EObjekty : int
        {
            PRIEPAST,
            CESTA,
            JABLKO,
            VYCHOD,
            MINA
        }

        public int[][] prostredie;

        public Agent agent = null;

        public void Vypis()
        {
            Console.WriteLine(new String('-', prostredie[0].Length * 4 + 1));
            for (int y = 0; y < prostredie.Length; y++)
            {
                for (int x = 0; x < prostredie[y].Length; x++)
                {
                    if (agent != null && agent.currentPositionX == x && agent.currentPositionY == y)
                    {
                        if (prostredie[y][x] == (int)EObjekty.JABLKO)
                            prostredie[y][x] = (int)EObjekty.CESTA;

                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;                        
                        Console.Write(" A ");
                        Console.ResetColor();                        
                        continue;                        
                    }

                    switch (prostredie[y][x])
                    {
                        case (int)EObjekty.PRIEPAST:
                            Console.Write("|   ");
                            break;
                        case (int)EObjekty.CESTA:
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(" + ");
                            Console.ResetColor();
                            break;
                        case (int)EObjekty.JABLKO:
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(" * ");
                            Console.ResetColor();
                            break;
                        case (int)EObjekty.MINA:
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(" M ");
                            Console.ResetColor();
                            break;
                        case (int)EObjekty.VYCHOD:
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(" E ");
                            Console.ResetColor();
                            break;
                    }
                }   
                Console.WriteLine("|");
                Console.WriteLine(new String('-', prostredie[y].Length * 4 + 1));
            }
        }
    }

    class Agent
    {
        public static readonly int startPositionX = 0;  // Zacina v bode [0,1]
        public static readonly int startPositionY = 1;  // Zacina v bode [0,1]

        public int currentPositionX = startPositionX;  // Sucasna poloha agenta
        public int currentPositionY = startPositionY;  // Sucasna poloha agenta

        private Dictionary<QState, float[]> Qtable = new Dictionary<QState, float[]>(128, new QStateEqualityComparer());           // [stav;akcia], pociatocna kapacita 128 zaznamov

        public readonly int pocetAkcii = 4;

        public int PocetUlozenychStavov
        {
            get
            {
                return Qtable.Count;
            }
        }

        public enum EAkcie : int
        {
            Hore,
            Dole,
            Vpravo,
            Vlavo
        }

        Random r = new Random();


        public void AktualizujAgenta(Prostredie env, bool ucenie)
        {
            const float learning_rate = 0.5f;
            const float gamma = 0.5f;

            var stav = UpravaDatSenzorov(env);
            var akcia = NajdiMaxAkciu(stav);

            /****************************************************/
            /*                 Agent vykona akciu               */
            /****************************************************/
            // Ak existuje vedomost
            if (akcia != null)
            {         
                // explore
                if (ucenie && r.NextDouble() < 0.2f)
                {
                    akcia = r.Next(0, 4);
                }   
                Pohyb((EAkcie)akcia.Value, 10, 10);                

                Console.WriteLine($"Qvalue[{akcia.Value}]: {Qtable[stav][akcia.Value]}");
            }
            // Ak neexistuje este vedomost
            else
            {   
                akcia = r.Next(0, 4);
                Pohyb((EAkcie)akcia.Value, 10, 10);

                // ... vytvor zaznam o najdenom stave
                Qtable.Add(stav, new float[] {0f,0f,0f,0f});
            }            
            Console.WriteLine($"Stav: {stav}");
            Console.WriteLine($"Akcia: {((EAkcie)akcia).ToString()}, {akcia}");
            Console.WriteLine();

            if (ucenie)
            {
                /****************************************************/
                /*                      Feedback                    */
                /****************************************************/
                var novyStav = UpravaDatSenzorov(env);
                var buducaAkcia = NajdiMaxAkciu(novyStav);
                var odmena = Hodnotenie(env);
                var buducaQhodnota = 0f;

                Console.WriteLine($"Odmena: {odmena}");
                Console.WriteLine($"novyStav: {novyStav}");

                // Aktualizuj Qtable hodnotu pre [s;a]
                if (buducaAkcia != null)
                {           
                    buducaQhodnota = Qtable[novyStav][buducaAkcia.Value];
                    Console.WriteLine($"buducaQvalue[{buducaAkcia}]: {buducaQhodnota}");
                }
                Qtable[stav][akcia.Value] = (1f - learning_rate) * Qtable[stav][akcia.Value] + learning_rate * (odmena + gamma * buducaQhodnota);
            }
        }


        private float Hodnotenie(Prostredie env)
        {
            switch (env.prostredie[currentPositionY][currentPositionX])
            {
                case (int)Prostredie.EObjekty.PRIEPAST:
                    return -100f;    // Smrt
                case (int)Prostredie.EObjekty.CESTA:
                    return -0.01f;   // Najkratsia cesta k vychodu
                case (int)Prostredie.EObjekty.JABLKO:
                    return +10f;     // Jablcko (odmena)
                case (int)Prostredie.EObjekty.VYCHOD:                
                    return +100f;    // Dalsi level
                case (int)Prostredie.EObjekty.MINA:
                    return -10f;    // Mina (trest)
                default:
                    return 0f;       // Error code :D
            }
        }

        private QState UpravaDatSenzorov(Prostredie env)
        {
            return new QState
            {
                // simuluje absolutnu polohu agenta vo svete (ako GPS), jedinecny identifikator polohy
                PositionX = currentPositionX,
                PositionY = currentPositionY,
                stateRadar = Radar(env)
            };
        }

        private int[] Radar(Prostredie env)
        {
            int[] scan = new int[8];

            // Hore
            if (!JeMimoAreny(currentPositionX, currentPositionY-1, 10, 10))
                scan[0] = env.prostredie[currentPositionY-1][currentPositionX];

            // Vpravo-Hore
            if (!JeMimoAreny(currentPositionX+1, currentPositionY-1, 10, 10))
                scan[1] = env.prostredie[currentPositionY-1][currentPositionX+1];

            // Vpravo
            if (!JeMimoAreny(currentPositionX+1, currentPositionY, 10, 10))
                scan[2] = env.prostredie[currentPositionY][currentPositionX+1];

            // Vpravo-Dole
            if (!JeMimoAreny(currentPositionX+1, currentPositionY+1, 10, 10))
                scan[3] = env.prostredie[currentPositionY+1][currentPositionX+1];

            // Dole
            if (!JeMimoAreny(currentPositionX, currentPositionY+1, 10, 10))
                scan[4] = env.prostredie[currentPositionY+1][currentPositionX];

            // Vlavo-Dole
            if (!JeMimoAreny(currentPositionX-1, currentPositionY+1, 10, 10))
                scan[5] = env.prostredie[currentPositionY+1][currentPositionX-1];

            // Vlavo
            if (!JeMimoAreny(currentPositionX-1, currentPositionY, 10, 10))
                scan[6] = env.prostredie[currentPositionY][currentPositionX-1];

            // Vlavo-Hore
            if (!JeMimoAreny(currentPositionX-1, currentPositionY-1, 10, 10))
                scan[7] = env.prostredie[currentPositionY-1][currentPositionX-1];

            return scan;
        }

        private int? NajdiMaxAkciu(QState state)
        {
            // Ak existuje vedomost
            if (Qtable.TryGetValue(state, out float[] akcia))
            {
                // Najdi najvyhodnejsiu akciu
                int akciaMax = 0;
                for (int i = 1; i < akcia.Length; i++)
                {
                    if (akcia[i] > akcia[akciaMax])
                        akciaMax = i;
                }
                return akciaMax;
            }
            return null;
        }

        private void Pohyb(EAkcie akcia, int maxW, int maxH)
        {
            var oldX = currentPositionX;
            var oldY = currentPositionY;

            switch (akcia)
            {
                case EAkcie.Hore:
                    //currentPosition[0] += 0;
                    currentPositionY += 1;
                    break;
                case EAkcie.Dole:
                    //currentPosition[0] += 0;
                    currentPositionY -= 1;
                    break;
                case EAkcie.Vlavo:
                    currentPositionX -= 1;
                    //currentPosition[1] += 0;
                    break;
                case EAkcie.Vpravo:
                    currentPositionX += 1;
                    //currentPosition[1] += 0;
                    break;
            }

            if (JeMimoAreny(currentPositionX, currentPositionY, 10, 10))
            {
                currentPositionX = oldX;
                currentPositionY = oldY;                
            }
        }

        private bool JeMimoAreny(int x, int y, int sirka, int vyska)
        {
            if (x >= 0 && x < sirka && y >= 0 && y < vyska)
                return false;

            return true;
        }
    }
}