
using System;

namespace QMazeExample
{ 
    public class Mina : EnvItem
    {
        public static int Tag = 3;

        public Mina()
        {
            this.name = "M";
            this.id = Tag; 
        }

        public override string ToString(bool light=false) 
        {
            if (!light)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            return base.ToString();
        }
    }
}