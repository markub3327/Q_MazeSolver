
using System;

namespace QMazeExample
{ 
    public class Vychod : EnvItem
    {
        public static int Tag = 4;

        public Vychod()
        {
            this.name = "E";
            this.id = Tag;
        }

        public override string ToString() 
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            return base.ToString();
        }
    }
}