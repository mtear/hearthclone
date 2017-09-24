using System.Collections.Generic;

namespace HS_Lib
{
    public class HS_Battlefield
    {

        private static byte MAX = 7;
        private List<List<HS_CreatureInstance>> fields;
        private Dictionary<HS_PlayerInstance, byte> playerFieldIndicies;

        public byte PlayerCount
        {
            get
            {
                return (byte)playerFieldIndicies.Keys.Count;
            }
        }

        public HS_Battlefield()
        {
            fields = new List<List<HS_CreatureInstance>>();
            playerFieldIndicies = new Dictionary<HS_PlayerInstance, byte>();
        }

        public void AddPlayer(HS_PlayerInstance player)
        {
            if (!playerFieldIndicies.ContainsKey(player))
            {
                playerFieldIndicies.Add(player, PlayerCount);
                fields.Add(new List<HS_CreatureInstance>());
            }
        }

        public void AddCreature(HS_PlayerInstance player, HS_CreatureInstance ci)
        {
            List<HS_CreatureInstance> field = GetField(player);
            if(field.Count < MAX)
            {
                field.Add(ci);
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

        public void Attack(HS_PlayerInstance fromPlayer, sbyte attackerIndex, HS_PlayerInstance targetPlayer, sbyte defenderIndex)
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
                    targetPlayer.TakeDamage(fromField[attackerIndex].CreatureStats.Power);
                }
            }
            if(attackerIndex >= 0 && fromField[attackerIndex].CreatureStats.Health <= 0)
            {
                fromField.RemoveAt(attackerIndex);
            }
            if(defenderIndex >= 0 && toField[defenderIndex].CreatureStats.Health <= 0)
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
