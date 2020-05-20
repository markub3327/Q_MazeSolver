

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AI.QLearning
{
    public class QState
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int[] stateRadar { get; set; }

        public override string ToString()
        {
            return $"{PositionX},{PositionY}\n" +
                   $"{stateRadar[0]},{stateRadar[1]},{stateRadar[2]},{stateRadar[3]},{stateRadar[4]},{stateRadar[5]},{stateRadar[6]},{stateRadar[7]}";
        }

        public override int GetHashCode()
        {
            return ((this.ToString()).GetHashCode());            
        }

        public override bool Equals(object obj)
        {
            var y = obj as QState;

            // Poloha agentov sa nezhoduje
            if ((this.PositionX != y.PositionX) || (this.PositionY != y.PositionY))
                return false;

            // Prehladaj pole radaru pri nezhode vrat false z funkcieß
            for (int i = 0; i < 8; i++)
            {
                if (this.stateRadar[i] != y.stateRadar[i])
                    return false;                    
            }

            // ziadne predchadzajuce return nezafungovalo nasiel zhodu
            return true;
        }
    }
}