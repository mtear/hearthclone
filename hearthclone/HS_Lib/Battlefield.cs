using System.Collections.Generic;

namespace HS_Lib
{
    public class HS_Battlefield
    {

        private int max = 7;
        private List<List<HS_CreatureInstance>> fields;
        private Dictionary<HS_PlayerInstance, char> playerFieldIndicies;

        public char PlayerCount
        {
            get
            {
                return (char)playerFieldIndicies.Keys.Count;
            }
        }

        public HS_Battlefield()
        {
            fields = new List<List<HS_CreatureInstance>>();
            playerFieldIndicies = new Dictionary<HS_PlayerInstance, char>();
        }

        public void AddPlayer(HS_PlayerInstance player)
        {
            if (!playerFieldIndicies.ContainsKey(player))
            {
                playerFieldIndicies.Add(player, PlayerCount);
                fields.Add(new List<HS_CreatureInstance>());
            }
        }

        public List<HS_CreatureInstance> GetField(HS_PlayerInstance player)
        {
            try
            {
                return fields[(int)playerFieldIndicies[player]];
            }
            catch {
                return null;
            }
        }

        public void Attack(HS_PlayerInstance fromPlayer, int attackerIndex, HS_PlayerInstance targetPlayer, int defenderIndex)
        {
            List<HS_CreatureInstance> fromField = GetField(fromPlayer);
            List<HS_CreatureInstance> toField = GetField(targetPlayer);
            if (attackerIndex >= 0 && defenderIndex >= 0)
            {
                Combat(fromField[attackerIndex], toField[defenderIndex]);
            }
            else
            {
                if(defenderIndex == -1)
                {
                    //targetPlayer.Life -= fromField[attackerIndex].CreatureStats.Power;
                }
            }
            if(fromField[attackerIndex].CreatureStats.Health <= 0)
            {
                fromField.RemoveAt(attackerIndex);
            }
            if(toField[defenderIndex].CreatureStats.Health <= 0)
            {
                toField.RemoveAt(defenderIndex);
            }
        }

        public void Combat(HS_CreatureInstance attacker, HS_CreatureInstance defender)
        {
            attacker.CreatureStats.Health -= defender.CreatureStats.Power;
            defender.CreatureStats.Health -= attacker.CreatureStats.Power;
        }

    }
}
