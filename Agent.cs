
using System;
using System.IO;
using System.Collections.Generic;

namespace QMazeExample
{
    public class Agent
    {
        public Vector2 currentPos;
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

        private List<Vector2[]> testing_applesPos;
        private List<Vector2[]> testing_minesPos;
        private List<Vector2> testing_startsPos;

        public Agent()
        {
            testing_applesPos = this.readCollectiblesFile(@"log_apples.txt");
            testing_minesPos = this.readCollectiblesFile(@"log_mines.txt");
            testing_startsPos = this.readStartFile(@"log_start_test.txt");
        }

        public void clearMem()
        {
            this.qBrain.Qtable.Clear();
        }

        public bool AktualizujAgenta(Prostredie env, bool ucenie, double eps, out float odmena)
        {
            bool isValid;
            int akcia;

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
            }
            // Ak neexistuje este vedomost
            else
            {   
                isValid = sample(out akcia);
                // ... vytvor zaznam o najdenom stave
                qBrain.Qtable.Add(stav, new float[] {0f,0f,0f,0f});
            }            

            if (isValid)
                odmena = env.Hodnotenie(currentPos.x, currentPos.y);
            else
                odmena = -1.0f;

            /****************************************************/
            /*                      Feedback                    */
            /****************************************************/
            var novyStav = new AI.QLearning.QState 
            { 
                PositionX = currentPos.x,
                PositionY = currentPos.y,
                stateRadar = Radar(env)
            };

            if (ucenie)
            {
                var buducaAkcia = qBrain.NajdiMaxAkciu(novyStav);
                var buducaQhodnota = 0f;

                // Aktualizuj Qtable hodnotu pre [s;a]
                if (buducaAkcia != null)
                {           
                    buducaQhodnota = qBrain.Qtable[novyStav][buducaAkcia.Value];
                }

                qBrain.Aktualizuj(stav, akcia, odmena, buducaQhodnota);             
            }

            this.stav = novyStav;
            
            // Agent zobral jablko
            if (env.prostredie[currentPos.y][currentPos.x].id == Jablko.Tag)
            {
                env.prostredie[currentPos.y][currentPos.x] = new Cesta();
                this.apples += 1;
            }

            // Agent aktivoval minu
            if (env.prostredie[currentPos.y][currentPos.x].id == Mina.Tag)
            {
                env.prostredie[currentPos.y][currentPos.x] = new Cesta();
                this.mines += 1;
            }

            return isValid;
        }

        private int[] Radar(Prostredie env)
        {
            int[] scan = new int[8];

            // Hore
            if (!JeMimoAreny(currentPos.x, currentPos.y-1, 10, 10))
                scan[0] = env.prostredie[currentPos.y-1][currentPos.x].id;

            // Vpravo-Hore
            if (!JeMimoAreny(currentPos.x+1, currentPos.y-1, 10, 10))
                scan[1] = env.prostredie[currentPos.y-1][currentPos.x+1].id;

            // Vpravo
            if (!JeMimoAreny(currentPos.x+1, currentPos.y, 10, 10))
                scan[2] = env.prostredie[currentPos.y][currentPos.x+1].id;

            // Vpravo-Dole
            if (!JeMimoAreny(currentPos.x+1, currentPos.y+1, 10, 10))
                scan[3] = env.prostredie[currentPos.y+1][currentPos.x+1].id;

            // Dole
            if (!JeMimoAreny(currentPos.x, currentPos.y+1, 10, 10))
                scan[4] = env.prostredie[currentPos.y+1][currentPos.x].id;

            // Vlavo-Dole
            if (!JeMimoAreny(currentPos.x-1, currentPos.y+1, 10, 10))
                scan[5] = env.prostredie[currentPos.y+1][currentPos.x-1].id;

            // Vlavo
            if (!JeMimoAreny(currentPos.x-1, currentPos.y, 10, 10))
                scan[6] = env.prostredie[currentPos.y][currentPos.x-1].id;

            // Vlavo-Hore
            if (!JeMimoAreny(currentPos.x-1, currentPos.y-1, 10, 10))
                scan[7] = env.prostredie[currentPos.y-1][currentPos.x-1].id;

            return scan;
        }

        private bool Pohyb(EAkcie akcia, int maxW, int maxH)
        {
            var oldX = currentPos.x;
            var oldY = currentPos.y;

            switch (akcia)
            {
                case EAkcie.Hore:
                    //currentPosition[0] += 0;
                    currentPos.y += 1;
                    break;
                case EAkcie.Dole:
                    //currentPosition[0] += 0;
                    currentPos.y -= 1;
                    break;
                case EAkcie.Vlavo:
                    currentPos.x -= 1;
                    //currentPosition[1] += 0;
                    break;
                case EAkcie.Vpravo:
                    currentPos.x += 1;
                    //currentPosition[1] += 0;
                    break;
            }

            if (JeMimoAreny(currentPos.x, currentPos.y, 10, 10))
            {
                currentPos.x = oldX;
                currentPos.y = oldY;   

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

        private List<Vector2[]> readCollectiblesFile(string path)
        {
            StreamReader log_apples = new StreamReader(path);
            List<Vector2[]> list = new List<Vector2[]>();
            string line;
            int i = 0;

            while((line = log_apples.ReadLine()) != null) 
            {
                var str = line.Split('|');

                list.Add(new Vector2[str.Length - 1]);

                for (int j = 0; j < str.Length - 1; j++)
                {    
                    var str2 = str[j].Split(';');
                    list[i][j] = new Vector2(int.Parse(str2[0]), int.Parse(str2[1]));
                }

                i += 1;
            }
            //Console.WriteLine($"{list.Count}");
            //Console.WriteLine($"{list[3].Length}");

            return list;
        }

        private List<Vector2> readStartFile(string path)
        {
            StreamReader log_starts = new StreamReader(path);
            List<Vector2> list = new List<Vector2>();
            string line;

            while((line = log_starts.ReadLine()) != null) 
            {
                var str = line.Split(';');
                list.Add(new Vector2(int.Parse(str[0]), int.Parse(str[1])));
            }
            //Console.WriteLine($"{list.Count}");

            return list;
        }

        public void reset(Prostredie env, int t = 0, bool training = true)
        {
            // Vymaz vsetky jablka a miny
            env.NahradObjekty(Jablko.Tag, new Cesta());
            env.NahradObjekty(Mina.Tag, new Cesta());

            if (training == true)
            {
                var idx = r.Next(0, 3);
                this.currentPos = new Vector2(Prostredie.startPositionX_training[idx], Prostredie.startPositionY_training[idx]);                

                apple_count = r.Next(2, 5) + 1;
                for (int i = 0; i < apple_count; i++)
                    env.GenerateItem(new Jablko());

                mine_count = r.Next(0, 3) + 1;
                for (int i = 0; i < mine_count; i++)
                    env.GenerateItem(new Mina());
            }
            else
            {
                this.currentPos = new Vector2(this.testing_startsPos[t].x, this.testing_startsPos[t].y);

                for (int i = 0; i < this.testing_applesPos[t].Length; i++)
                    env.prostredie[this.testing_applesPos[t][i].y][this.testing_applesPos[t][i].x] = new Jablko();
    
                for (int i = 0; i < this.testing_minesPos[t].Length; i++)
                    env.prostredie[this.testing_minesPos[t][i].y][this.testing_minesPos[t][i].x] = new Mina();
            }

            stav = new AI.QLearning.QState 
            { 
                PositionX = currentPos.x,
                PositionY = currentPos.y,
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
