
using System;

namespace QMazeExample
{ 
    public class Cesta : EnvItem
    {
        public static int Tag = 1;

        public Cesta()
        {
            this.name = "+";
            this.id = Tag;
        }

        public override string ToString(bool light=false) 
        {
            if (!light)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;        
            }

            return base.ToString();
        }
    }
}