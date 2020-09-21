
using System;
using System.Collections.Generic;

namespace QMazeExample
{
    public class Agent
    {
        public int currentPositionX;  // Sucasna poloha agenta
        public int currentPositionY;  // Sucasna poloha agenta
        public readonly int pocetAkcii = 4;
        public int PocetUlozenychStavov
        {
            get
            {
                return qBrain.Qtable.Count;
            }
        }
        public enum EAkcie : int
        {
            Hore,
            Dole,
            Vpravo,
            Vlavo
        }
        Random r = new Random((int)DateTime.Now.Ticks);

        public int apples = 0;
        public int apple_count = 0;

        public int mines = 0;
        public int mine_count = 0;

        AI.QLearning.Qlearning qBrain = new AI.QLearning.Qlearning();

        public AI.QLearning.QState stav = null;

        public float AktualizujAgenta(Prostredie env, bool ucenie, double eps)
        {
            bool isValid;
            int akcia;
            float odmena;

            // Vyber akciu
            var akcia_max = qBrain.NajdiMaxAkciu(stav);
            
            /****************************************************/
            /*                 Agent vykona akciu               */
            /****************************************************/
            // Ak existuje vedomost
            if (akcia_max != null)
            {         
                // explore
                if (ucenie && r.NextDouble() < eps)
                   isValid = sample(out akcia);
                else
                {
                    akcia = akcia_max.Value;
                    isValid = Pohyb((EAkcie)akcia, 10, 10);
                }
                //Console.WriteLine($"Qvalue[{akcia.Value}]: {qBrain.Qtable[stav][akcia.Value]}");
            }
            // Ak neexistuje este vedomost
            else
            {   
                isValid = sample(out akcia);
                // ... vytvor zaznam o najdenom stave
                qBrain.Qtable.Add(stav, new float[] {0f,0f,0f,0f});
            }            
            //Console.WriteLine($"Stav: {stav}");
            //Console.WriteLine($"Akcia: {((EAkcie)akcia).ToString()}, {akcia}");
            //Console.WriteLine();

            if (isValid)
                odmena = env.Hodnotenie(currentPositionX, currentPositionY);
            else
                odmena = -1.0f;

            /****************************************************/
            /*                      Feedback                    */
            /****************************************************/
            var novyStav = new AI.QLearning.QState 
            { 
                PositionX = currentPositionX,
                PositionY = currentPositionY,
                stateRadar = Radar(env)
            };

            if (ucenie)
            {
                var buducaAkcia = qBrain.NajdiMaxAkciu(novyStav);
                var buducaQhodnota = 0f;

                //Console.WriteLine($"Odmena: {odmena}");
                //Console.WriteLine($"novyStav: {novyStav}");

                // Aktualizuj Qtable hodnotu pre [s;a]
                if (buducaAkcia != null)
                {           
                    buducaQhodnota = qBrain.Qtable[novyStav][buducaAkcia.Value];
                    //Console.WriteLine($"buducaQvalue[{buducaAkcia}]: {buducaQhodnota}");
                }

                qBrain.Aktualizuj(stav, akcia, odmena, buducaQhodnota);             
            }

            this.stav = novyStav;
            
            // Agent zobral jablko
            if (env.prostredie[currentPositionY][currentPositionX].id == Jablko.Tag)
            {
                env.prostredie[currentPositionY][currentPositionX] = new Cesta();
                this.apples += 1;
            }

            // Agent aktivoval minu
            if (env.prostredie[currentPositionY][currentPositionX].id == Mina.Tag)
            {
                env.prostredie[currentPositionY][currentPositionX] = new Cesta();
                this.mines += 1;
            }

            return odmena;
        }

        private int[] Radar(Prostredie env)
        {
            int[] scan = new int[8];

            // Hore
            if (!JeMimoAreny(currentPositionX, currentPositionY-1, 10, 10))
                scan[0] = env.prostredie[currentPositionY-1][currentPositionX].id;

            // Vpravo-Hore
            if (!JeMimoAreny(currentPositionX+1, currentPositionY-1, 10, 10))
                scan[1] = env.prostredie[currentPositionY-1][currentPositionX+1].id;

            // Vpravo
            if (!JeMimoAreny(currentPositionX+1, currentPositionY, 10, 10))
                scan[2] = env.prostredie[currentPositionY][currentPositionX+1].id;

            // Vpravo-Dole
            if (!JeMimoAreny(currentPositionX+1, currentPositionY+1, 10, 10))
                scan[3] = env.prostredie[currentPositionY+1][currentPositionX+1].id;

            // Dole
            if (!JeMimoAreny(currentPositionX, currentPositionY+1, 10, 10))
                scan[4] = env.prostredie[currentPositionY+1][currentPositionX].id;

            // Vlavo-Dole
            if (!JeMimoAreny(currentPositionX-1, currentPositionY+1, 10, 10))
                scan[5] = env.prostredie[currentPositionY+1][currentPositionX-1].id;

            // Vlavo
            if (!JeMimoAreny(currentPositionX-1, currentPositionY, 10, 10))
                scan[6] = env.prostredie[currentPositionY][currentPositionX-1].id;

            // Vlavo-Hore
            if (!JeMimoAreny(currentPositionX-1, currentPositionY-1, 10, 10))
                scan[7] = env.prostredie[currentPositionY-1][currentPositionX-1].id;

            return scan;
        }

        private bool Pohyb(EAkcie akcia, int maxW, int maxH)
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

                return false;
            }

            return true;
        }

        private bool JeMimoAreny(int x, int y, int sirka, int vyska)
        {
            if (x >= 0 && x < sirka && y >= 0 && y < vyska)
                return false;

            return true;
        }

        public void reset(Prostredie env, bool testing=false)
        {
            // Test agenta
            if (testing)
            {
                var idx = r.Next(0, 3);
                this.currentPositionX = Prostredie.startPositionX_testing[idx]; 
                this.currentPositionY = Prostredie.startPositionY_testing[idx];
            }
            else
            {
                var idx = r.Next(0, 3);
                this.currentPositionX = Prostredie.startPositionX_training[idx]; 
                this.currentPositionY = Prostredie.startPositionY_training[idx];                
            }

            // Vymaz vsetky jablka a miny
            env.NahradObjekty(Jablko.Tag, new Cesta());
            env.NahradObjekty(Mina.Tag, new Cesta());

            apple_count = r.Next(2,5) + 1;
            for (int i = 0; i < apple_count; i++)
                env.GenerateItem(new Jablko());

            mine_count = r.Next(0,3) + 1;
            for (int i = 0; i < mine_count; i++)
                env.GenerateItem(new Mina());

            stav = new AI.QLearning.QState 
            { 
                PositionX = currentPositionX,
                PositionY = currentPositionY,
                stateRadar = Radar(env)
            };

            this.apples = 0;
            this.mines = 0;
        }

        public bool sample(out int akcia)
        {
            akcia = r.Next(0, 4);
            return Pohyb((EAkcie)akcia, 10, 10);
        }
    }
}