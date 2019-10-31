

using System.Collections.Generic;

namespace AI.QLearning
{
    public class Qlearning
    {
        private const float learning_rate = 0.5f;
        private const float gamma = 0.5f;
        
        // Slovnik parov [stav;akcia], pociatocna kapacita 128 zaznamov
        public Dictionary<QState, float[]> Qtable { get; private set; } = new Dictionary<QState, float[]>(128);           

        public int? NajdiMaxAkciu(QState state)
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

        public void Aktualizuj(QState stav, int akcia, float odmena, float buducaQhodnota)
        {
            Qtable[stav][akcia] = (1f - learning_rate) * Qtable[stav][akcia] + learning_rate * (odmena + gamma * buducaQhodnota);
        }
    }
}