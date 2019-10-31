

namespace QMazeExample
{ 
    public class EnvItem
    {
        public string name { get; set; }
        public int id { get; set; }

        public override string ToString()
        {
            return this.name;
        }
    }
}