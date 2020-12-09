
using System;

namespace QMazeExample
{ 
    public class Jablko : EnvItem
    {
        public static int Tag = 2;

        public Jablko()
        {
            this.name = "*";
            this.id = Tag;
        }

        public override string ToString(bool light=false) 
        {
            if (!light)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;                
            }
            return base.ToString();
        }
    }
}