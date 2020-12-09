

namespace QMazeExample
{ 
    public abstract class EnvItem
    {
        public string name { get; set; }
        public int id { get; set; }

        public override string ToString()
        {
            return this.name;
        }

        public virtual string ToString(bool light)
        {
            return this.name;
        }
    }
}