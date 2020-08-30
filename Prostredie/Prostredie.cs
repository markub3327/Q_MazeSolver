
using System;

namespace QMazeExample
{ 
    public class Prostredie
    {
        public static readonly int[] startPositionX = { 0, 9, 0 };  // Zacina v bode [0,1]
        public static readonly int[] startPositionY = { 1, 1, 5 };   // Zacina v bode [0,1]

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

            /*if (prostredie[4][5].id == Cesta.Tag)
            {
                x = 5;
                y = 4;
            }
            else if (prostredie[2][9].id == Cesta.Tag)
            {
                x = 9;
                y = 2;
            }
            else*/
                do {
                    x = r.Next(1, Width-1);
                    y = r.Next(1, Height-1);
                } while (prostredie[y][x].id != Cesta.Tag);

            // prepis novou polozkou sveta
            prostredie[y][x] = item;
        }

        public float Hodnotenie(int x, int y)
        {
            if (prostredie[y][x].id == Priepast.Tag)
                return -0.75f;    // Smrt
            else if (prostredie[y][x].id == Jablko.Tag)
                return +0.10f;     // Jablcko (odmena)
            else if (prostredie[y][x].id == Mina.Tag)
                return -0.15f;     // Mina (trest)
            else if (prostredie[y][x].id == Vychod.Tag)
                return +1.0f;    // Dalsi level      
            else
                return -0.04f;   // Najkratsia cesta k vychodu
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
                    if (agentX == x && agentY == y)
                    {
                        // Vykresli agenta
                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;                        
                        Console.Write(" A ");
                        Console.ResetColor();
                        continue;                        
                    }
                                                 
                    Console.Write("|");
                    Console.Write($" {prostredie[y][x].ToString()} ");
                    Console.ResetColor();                         
                }   
                Console.WriteLine("|");
                Console.WriteLine(new String('-', prostredie[y].Length * 4 + 1));
            }
        }
    }
}
